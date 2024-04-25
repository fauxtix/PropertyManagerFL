using EmailService;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PropertyManagerFL.UI;
using PropertyManagerFL.UI.Areas.Identity;
using PropertyManagerFL.UI.Data;
using PropertyManagerFL.UI.Shared;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Syncfusion.Blazor;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PMConnection");

#pragma warning disable ASP0013 // Suggest switching from using Configure methods to WebApplicationBuilder.Configuration
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
        .WriteTo.MSSqlServer(connectionString,
        sinkOptions: new MSSqlServerSinkOptions()
        {
            SchemaName = "dbo",
            AutoCreateSqlTable = true,
            TableName = "PMLogs"
        })
        //        .WriteTo.Console()
        .CreateLogger();
}).UseSerilog();
#pragma warning restore ASP0013 // Suggest switching from using Configure methods to WebApplicationBuilder.Configuration


builder.Services.AddMemoryCache(options =>
{
    // Set cache size limit (in bytes)
    options.SizeLimit = 1024 * 1024 * 100; // 100 MB

    // Set cache compaction percentage
    options.CompactionPercentage = 0.25; // 25%

    // Set cache expiration scan frequency
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // 5 minutes
});

builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString!));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.Scope.Add("offline_access");
    });

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddFluentValidationConfiguration();
builder.Services.AddServicesDependencyInjectionConfiguration();



// força a abertura da página de login no arranque da aplicação
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});

#region Localization

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

// defines the list of cultures that the app will support
var supportedCultures = new[] { "en-US", "es", "fr", "pt-PT" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

#endregion

var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

if (emailConfig != null)
{
    builder.Services.AddSingleton(emailConfig);
}
else
{
    throw new InvalidOperationException("Email configuration is missing or invalid.");
}

builder.Services.AddHttpClient();
builder.Services.AddScoped<TokenProvider>();

var app = builder.Build();

#region Localization

app.UseRequestLocalization(localizationOptions);

#endregion


Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE2NjM3MEAzMjM0MmUzMDJlMzBnWFVrR2hOQjJSNUFaNjZPNkU0T1VLYnFRSTZGaEFES3Y0U0xEMGFYdGEwPQ==");
//app.UseRequestLocalization("pt");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// var port = app.Environment.IsDevelopment() ? "5001" : "4300";

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseCors(builder =>
{
    builder.WithOrigins("*")
    .AllowAnyMethod()
    .AllowAnyHeader();

});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
