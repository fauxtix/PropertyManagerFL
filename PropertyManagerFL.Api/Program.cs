using EmailService;
using log4net.Config;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using PropertyManagerFL.Api.Configuration;
using PropertyManagerFL.Infrastructure.Context;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
//Configure Log4net.
XmlConfigurator.Configure(new FileInfo("log4net.config"));

var connectionString = builder.Configuration.GetConnectionString("PMConnection");
string? ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var emailConfig = configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddHttpClient();
builder.Services.AddDataProtection().SetApplicationName("PropertyManagerFL");
builder.Services.AddScoped<DapperContext>();
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddSignalR();
builder.Services.AddJwtTConfiguration(configuration);
builder.Services.AddFluentValidationConfiguration();
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddDatabaseConfiguration(configuration);
builder.Services.AddDependencyInjectionConfiguration();
builder.Services.AddSwaggerConfiguration();

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


var app = builder.Build();

string? SyncfusionLicenseKey = configuration["SyncfusionLicenseKey"];
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionLicenseKey);

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "text/plain; charset=utf-8";

        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (error is not null)
        {
            app.Logger.LogError(error, "Unhandled API exception");
        }

        await context.Response.WriteAsync("Algo de errado ocorreu. Contacte o Administrador");
    });
});

app.UseDatabaseConfiguration();
app.UseHttpsRedirection();
app.UseRouting();

app.UseJwtConfiguration();

// Swagger is served behind the API-key gate.
app.UseSwaggerConfiguration();

app.MapControllers();
app.MapHub<PropertyManagerFL.Api.Hubs.NotificationHub>("/hubs/notifications");

//app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();