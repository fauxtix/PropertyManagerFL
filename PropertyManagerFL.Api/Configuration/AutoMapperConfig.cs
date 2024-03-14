using PropertyManagerFL.Application.Mappings;

namespace PropertyManagerFL.Api.Configuration;

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
            typeof(AppointmentsMappingProfile),
            typeof(AppLogMappingProfile),
            typeof(ApplicationSettingsMappingProfile),
            typeof(ProprietarioMappingProfile),
            typeof(InquilinoMappingProfile),
            typeof(FracaoMappingProfile),
            typeof(ContactoMappingProfile),
            typeof(ArrendamentoMappingProfile),
            typeof(DespesaMappingProfile),
            typeof(RecebimentoMappingProfile),
            typeof(DocumentosMappingProfile),
            typeof(TipoDespesaMappingProfile),
            typeof(HistoricoEnvioCartasMappingProfile),
            typeof(ImovelMappingProfile));
    }
}