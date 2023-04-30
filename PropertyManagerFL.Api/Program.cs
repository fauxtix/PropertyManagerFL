using log4net.Config;
using PropertyManagerFL.Api.Configuration;
using PropertyManagerFL.Api.Middlewares;
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

builder.Services.AddHttpClient();
builder.Services.AddScoped<DapperContext>();
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
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

try
{
    Log.Information("Iniciando o WebApi");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro catastrófico.");
    throw;
}
//finally
//{
//    Log.CloseAndFlush();
//}

var app = builder.Build();


Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0Z+X09EaFpEVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUViWX1fdXdRR2ddUUJx");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDatabaseConfiguration();

app.UseSwaggerConfiguration();
app.UseHttpsRedirection();
app.UseRouting();
app.UseJwtConfiguration();


app.MapControllers();

//app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();