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
# Reuse the pre-existing pools: their ApplicationPoolIdentity accounts already have
# SQL logins and db_owner on PropertyManagerDB. New pools would fail Integrated Security.
$apiPool = 'PropertyManagerApi'
$uiPool = 'PropertyManagerFL'
# Pools created by earlier runs can keep w3wp processes (and DLL file locks) alive
# even after the sites have been reassigned to the pools above.
$legacyPools = @('PropertyManagerFL.Api', 'PropertyManagerFL.UI')
$apiPath = Join-Path $root 'Api'
$uiPath = Join-Path $root 'UI'
$apiPort = 4300
$uiPort = 4310

function Ensure-Directory([string] $Path) {
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Wait-AppPoolState([string] $Name, [string] $Desired) {
    for ($i = 0; $i -lt 40; $i++) {
        if ((Get-WebAppPoolState -Name $Name -ErrorAction Stop).Value -eq $Desired) {
            return
        }
        Start-Sleep -Milliseconds 250
    }
    throw "App pool $Name did not reach state $Desired"
}

function Stop-AppPool-WithRetry([string] $Name) {
    for ($attempt = 0; $attempt -lt 10; $attempt++) {
        if ((Get-WebAppPoolState -Name $Name -ErrorAction Stop).Value -eq 'Stopped') {
            return
        }
        try {
            Stop-WebAppPool -Name $Name -ErrorAction Stop
        }
        catch {
            if ($attempt -eq 9) { throw }
            # IIS can briefly reject control messages right after a pool stop.
            Start-Sleep -Milliseconds 500
            continue
        }
        try {
            Wait-AppPoolState $Name 'Stopped'
            return
        }
        catch {
            if ($attempt -eq 9) { throw }
            Start-Sleep -Milliseconds 500
        }
    }
}

function Start-AppPool-WithRetry([string] $Name) {
    for ($attempt = 0; $attempt -lt 10; $attempt++) {
        if ((Get-WebAppPoolState -Name $Name -ErrorAction Stop).Value -eq 'Started') {
            return
        }
        try {
            Start-WebAppPool -Name $Name -ErrorAction Stop
            Wait-AppPoolState $Name 'Started'
            return
        }
        catch {
            if ($attempt -eq 9) { throw }
            Start-Sleep -Milliseconds 500
        }
    }
}

function Start-Site-WithRetry([string] $Name) {
    for ($attempt = 0; $attempt -lt 10; $attempt++) {
        if ((Get-Website -Name $Name -ErrorAction Stop).State -eq 'Started') {
            return
        }
        try {
            Start-Website -Name $Name -ErrorAction Stop
            return
        }
        catch {
            if ($attempt -eq 9) { throw }
            Start-Sleep -Milliseconds 500
        }
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

    # Recreate the port binding without SNI: 4310 is dedicated to this site, and a
    # non-SNI binding is the default SSL site, so IIS does not warn about SNI-only SSL.
    Get-WebBinding -Name $Site |
        Where-Object { $_.bindingInformation -like "*:$($Port):*" -or $_.bindingInformation -like "*:$($Port)" } |
        Remove-WebBinding

    $bindings = @(Get-WebBinding -Name $Site -Protocol https -Port $Port)
    if ($bindings.Count -eq 0) {
        New-WebBinding -Name $Site -Protocol https -Port $Port -SslFlags 0 | Out-Null
        $bindings = @(Get-WebBinding -Name $Site -Protocol https -Port $Port)
    }
    foreach ($binding in $bindings) {
        $binding.AddSslCertificate($cert.Thumbprint, 'my')
    }
}

function Set-WebConfigEnvironment([string] $SitePath, [hashtable] $Values) {
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

    foreach ($key in $Values.Keys) {
        $value = [string] $Values[$key]
        if ([string]::IsNullOrWhiteSpace($value)) {
            throw "Missing deployment value for $key"
        }
        # ANCM schema uses <environmentVariable>, not <add>.
        $node = $variables.SelectSingleNode("environmentVariable[@name='$key']")
        if ($null -eq $node) {
            $node = $xml.CreateElement('environmentVariable')
            [void] $variables.AppendChild($node)
        }
        $node.SetAttribute('name', $key)
        $node.SetAttribute('value', $value)
    }

    $xml.Save($webConfig)
}

function Wait-NoW3wpForPools([string[]] $Pools) {
    for ($i = 0; $i -lt 40; $i++) {
        $procs = @(Get-CimInstance Win32_Process -Filter "Name='w3wp.exe'" | Where-Object {
            $cmdLine = [string] $_.CommandLine
            foreach ($pool in $Pools) {
                if ($cmdLine -match ('-ap\s+"?' + [regex]::Escape($pool) + '"?')) { return $true }
            }
            return $false
        })
        if ($procs.Count -eq 0) { return }
        Start-Sleep -Milliseconds 500
    }
}

function Copy-PublishedFiles([string] $Source, [string] $Destination) {
    if (-not (Test-Path $Source)) {
        throw "Publish folder not found: $Source"
    }
    Ensure-Directory $Destination
    # Overwrite only: never delete the destination (uploaded files may live there).
    # A just-stopped w3wp can briefly hold a DLL, so retry the (idempotent) copy.
    $lastError = $null
    for ($attempt = 0; $attempt -lt 10; $attempt++) {
        $before = $Error.Count
        Copy-Item -Path (Join-Path $Source '*') -Destination $Destination -Recurse -Force -ErrorAction SilentlyContinue
        if ($? -and $Error.Count -eq $before) { return }
        if ($Error.Count -gt $before) { $lastError = $Error[0] }
        Start-Sleep -Seconds 1
    }
    throw "Failed to copy publish files to $Destination : $lastError"
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

# Removing/recreating bindings can leave a site stopped even when its pool is running.
foreach ($site in @($apiSite, $uiSite)) {
    Start-Site-WithRetry $site
}

try {
    $poolsToStop = @($apiPool, $uiPool) + @($legacyPools | Where-Object { Test-Path "IIS:\AppPools\$_" })
    foreach ($pool in $poolsToStop) {
        Stop-AppPool-WithRetry $pool
    }

    Wait-NoW3wpForPools $poolsToStop
    Copy-PublishedFiles $ApiPublishDir $apiPath
    Copy-PublishedFiles $UiPublishDir $uiPath

    # Letter templates are essential to the letter editor and mail merge. Assert they
    # reached the IIS content root; a silent miss would leave Dev/Prod with no templates.
    $expectedTemplates = @(Get-ChildItem -Path (Join-Path $ApiPublishDir 'Reports\Docs') -Filter '*.dotx' -File -ErrorAction SilentlyContinue)
    $deployedTemplates = @(Get-ChildItem -Path (Join-Path $apiPath 'Reports\Docs') -Filter '*.dotx' -File -ErrorAction SilentlyContinue)
    if ($expectedTemplates.Count -eq 0 -or $deployedTemplates.Count -ne $expectedTemplates.Count) {
        throw "Letter templates missing from IIS API folder: expected $($expectedTemplates.Count), found $($deployedTemplates.Count) in $apiPath\Reports\Docs"
    }
    Write-Host "Verified $($deployedTemplates.Count) letter templates in $apiPath\Reports\Docs"

    # Production does not load user-secrets: inject every value the apps read at startup.
    $commonEnv = @{
        ASPNETCORE_ENVIRONMENT          = 'Production'
        ApiKey                          = $env:PM_API_KEY
        ConnectionStrings__PMConnection = $env:PM_PMCONNECTION
    }
    # The UI and the API have different Syncfusion keys in Development; keep them separate.
    $apiEnv = $commonEnv + @{
        'SyncfusionLicenseKey' = $env:PM_SYNCFUSION_LICENSE_API
        'JWT:Secret'          = $env:PM_JWT_SECRET
        'JWT:Issuer'          = $env:PM_JWT_ISSUER
        'JWT:Audience'        = $env:PM_JWT_AUDIENCE
        'JWT:ExpiraEmMinutos' = $env:PM_JWT_EXPIRA_EM_MINUTOS
    }
    $uiEnv = $commonEnv + @{
        'SyncfusionLicenseKey'          = $env:PM_SYNCFUSION_LICENSE
        'EmailConfiguration:Username'   = $env:PM_EMAIL_USERNAME
        'EmailConfiguration:SmtpServer' = $env:PM_EMAIL_SMTPSERVER
        'EmailConfiguration:Port'       = $env:PM_EMAIL_PORT
        'EmailConfiguration:Password'   = $env:PM_EMAIL_PASSWORD
        'EmailConfiguration:From'       = $env:PM_EMAIL_FROM
    }
    Set-WebConfigEnvironment $apiPath $apiEnv
    Set-WebConfigEnvironment $uiPath $uiEnv

    Write-Host "Deployed API -> $apiPath (http://localhost:$apiPort)"
    Write-Host "Deployed UI  -> $uiPath (https://localhost:$uiPort)"
}
finally {
    # Always leave both pools running, including when a previous run left them stopped.
    foreach ($pool in @($apiPool, $uiPool)) {
        Start-AppPool-WithRetry $pool
    }
}
