using PropertyManagerFL.Application.Mappings;
using System.Reflection;

namespace PropertyManagerFL.UI;

/// <summary>
/// Configura automapper para DI
/// </summary>
public static class AutoMapperConfig
{
    /// <summary>
    /// Configura automapper para DI
    /// </summary>
    /// <param name="services"></param>
    public static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(AppLogMappingProfile),
            typeof(InquilinoMappingProfile),
            typeof(ProprietarioMappingProfile),
            typeof(FracaoMappingProfile));
    }
}