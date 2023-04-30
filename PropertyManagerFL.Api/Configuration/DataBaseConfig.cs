using Microsoft.EntityFrameworkCore;
using PropertyManagerFL.Infrastructure.Context;

namespace PropertyManagerFL.Api.Configuration;

public static class DataBaseConfig
{
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PMContext>(options => options.UseSqlServer(configuration.GetConnectionString("PMConnection")));
    }

    /// <summary>
    /// Database configuration
    /// </summary>
    /// <param name="app"></param>
    public static void UseDatabaseConfiguration(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetService<PMContext>();
        //context.Database.Migrate();
        //context.Database.EnsureCreated();
    }
}