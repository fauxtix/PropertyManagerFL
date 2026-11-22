param(
    [Parameter(Mandatory = $true)][string] $ApiPublishDir,
    [Parameter(Mandatory = $true)][string] $UiPublishDir
)

$ErrorActionPreference = 'Stop'

# The runner service runs as SYSTEM, so it can start IIS if it is stopped.
$w3svc = Get-Service W3SVC -ErrorAction Stop
if ($w3svc.Status -ne 'Running') {
    Start-Service W3SVC -ErrorAction Stop
}

Import-Module WebAdministration

$root = 'C:\inetpub\wwwroot\PropertyManagerFL'
$apiSite = 'PropertyManagerFL.Api'
$uiSite = 'PropertyManagerFL.UI'
$apiPool = 'PropertyManagerFL.Api'
$uiPool = 'PropertyManagerFL.UI'
$apiPath = Join-Path $root 'Api'
$uiPath = Join-Path $root 'UI'
$apiPort = 4300
$uiPort = 4310

function Ensure-Directory([string] $Path) {
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Ensure-AppPool([string] $Name) {
    if (-not (Test-Path "IIS:\AppPools\$Name")) {
        New-WebAppPool -Name $Name | Out-Null
    }
    # .NET Framework CLR version must be empty for ASP.NET Core apps.
    Set-ItemProperty -Path "IIS:\AppPools\$Name" -Name managedRuntimeVersion -Value ''
}

function Ensure-Site([string] $Name, [string] $Pool, [string] $Path, [int] $Port) {
    if (-not (Test-Path "IIS:\Sites\$Name")) {
        New-Website -Name $Name -Port $Port -PhysicalPath $Path -ApplicationPool $Pool | Out-Null
    }
    else {
        Set-ItemProperty -Path "IIS:\Sites\$Name" -Name applicationPool -Value $Pool
        Set-ItemProperty -Path "IIS:\Sites\$Name" -Name physicalPath -Value $Path
    }
}

function Ensure-HttpBinding([string] $Site, [int] $Port) {
    if (-not (Get-WebBinding -Name $Site -Protocol http -Port $Port)) {
        New-WebBinding -Name $Site -Protocol http -Port $Port | Out-Null
    }
}

function Ensure-HttpsBinding([string] $Site, [int] $Port, [string] $Thumbprint) {
    if ($Thumbprint) {
        $cert = Get-Item -Path "Cert:\LocalMachine\My\$Thumbprint" -ErrorAction Stop
    }
    else {
        $cert = Get-ChildItem -Path Cert:\LocalMachine\My |
            Where-Object { $_.Subject -eq 'CN=localhost' -and $_.NotAfter -gt (Get-Date) } |
            Sort-Object NotAfter -Descending |
            Select-Object -First 1
        if (-not $cert) {
            $cert = New-SelfSignedCertificate -DnsName localhost -CertStoreLocation Cert:\LocalMachine\My -FriendlyName 'PropertyManagerFL UI (local)'
        }
    }

    $bindings = @(Get-WebBinding -Name $Site -Protocol https -Port $Port)
    if ($bindings.Count -eq 0) {
        # SNI (SslFlags=1) requires a host header in bindingInformation.
        New-WebBinding -Name $Site -Protocol https -Port $Port -HostHeader localhost -SslFlags 1 | Out-Null
        $bindings = @(Get-WebBinding -Name $Site -Protocol https -Port $Port)
    }
    foreach ($binding in $bindings) {
        $binding.AddSslCertificate($cert.Thumbprint, 'my')
    }

    # The site may have been bootstrapped with an HTTP binding on the same port.
    Get-WebBinding -Name $Site -Protocol http -Port $Port | Remove-WebBinding
}

function Set-WebConfigEnvironment([string] $SitePath) {
    $webConfig = Join-Path $SitePath 'web.config'
    if (-not (Test-Path $webConfig)) {
        throw "web.config was not published to $SitePath"
    }

    [xml] $xml = Get-Content $webConfig -Raw
    # publish puts <aspNetCore> inside <location path=".">, not directly under <configuration>.
    $aspNetCore = $xml.SelectSingleNode('//system.webServer/aspNetCore')
    if ($null -eq $aspNetCore) {
        throw "No system.webServer/aspNetCore element in $webConfig"
    }

    $variables = $aspNetCore.SelectSingleNode('environmentVariables')
    if ($null -eq $variables) {
        $variables = $xml.CreateElement('environmentVariables')
        [void] $aspNetCore.AppendChild($variables)
    }

    $values = [ordered] @{
        ASPNETCORE_ENVIRONMENT          = 'Production'
        ApiKey                          = $env:PM_API_KEY
        SyncfusionLicenseKey            = $env:PM_SYNCFUSION_LICENSE
        ConnectionStrings__PMConnection = $env:PM_PMCONNECTION
    }

    foreach ($key in $values.Keys) {
        $value = [string] $values[$key]
        if ([string]::IsNullOrWhiteSpace($value)) {
            throw "Missing deployment value for $key"
        }
        $node = $variables.SelectSingleNode("add[@name='$key']")
        if ($null -eq $node) {
            $node = $xml.CreateElement('add')
            [void] $variables.AppendChild($node)
        }
        $node.SetAttribute('name', $key)
        $node.SetAttribute('value', $value)
    }

    $xml.Save($webConfig)
}

function Copy-PublishedFiles([string] $Source, [string] $Destination) {
    if (-not (Test-Path $Source)) {
        throw "Publish folder not found: $Source"
    }
    Ensure-Directory $Destination
    # Overwrite only: never delete the destination (uploaded files may live there).
    Copy-Item -Path (Join-Path $Source '*') -Destination $Destination -Recurse -Force
}

Ensure-Directory $root
# New-Website requires the physical path to exist before it is called.
Ensure-Directory $apiPath
Ensure-Directory $uiPath
Ensure-AppPool $apiPool
Ensure-AppPool $uiPool
Ensure-Site $apiSite $apiPool $apiPath $apiPort
Ensure-Site $uiSite $uiPool $uiPath $uiPort
Ensure-HttpBinding $apiSite $apiPort
Ensure-HttpsBinding $uiSite $uiPort $env:PM_UI_CERT_THUMBPRINT

$stoppedPools = @()
try {
    foreach ($pool in @($apiPool, $uiPool)) {
        if ((Get-WebAppPoolState -Name $pool).Value -ne 'Stopped') {
            Stop-WebAppPool -Name $pool
            $stoppedPools += $pool
        }
    }

    Copy-PublishedFiles $ApiPublishDir $apiPath
    Copy-PublishedFiles $UiPublishDir $uiPath
    Set-WebConfigEnvironment $apiPath
    Set-WebConfigEnvironment $uiPath

    Write-Host "Deployed API -> $apiPath (http://localhost:$apiPort)"
    Write-Host "Deployed UI  -> $uiPath (https://localhost:$uiPort)"
}
finally {
    foreach ($pool in $stoppedPools) {
        Start-WebAppPool -Name $pool
    }
}
