using EmailService;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Repositories.Data_Operations;
using PropertyManagerFL.Application.Interfaces.Repositories.Email;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Infrastructure.Context;
using PropertyManagerFL.Infrastructure.Repositories;
using PropertyManagerFL.Infrastructure.Repositories.Data_Operations;

namespace PropertyManagerFL.Api.Configuration;

/// <summary>
/// Dependency Injection
/// </summary>
public static class DependencyInjectionConfig
{
    /// <summary>
    /// Add Dependency Injection Configuration
    /// </summary>
    /// <param name="services"></param>
    public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IDapperContext, DapperContext>();
        services.AddTransient<IValidationService, ValidationService>();

        services.AddScoped<IProprietarioRepository, ProprietarioRepository>();
        services.AddScoped<IImovelRepository, ImovelRepository>();
        services.AddScoped<IFracaoRepository, FracaoRepository>();
        services.AddScoped<IInquilinoRepository, InquilinoRepository>();
        services.AddScoped<IFiadorRepository, FiadorRepository>();
        services.AddScoped<IArrendamentoRepository, ArrendamentoRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ITipoDespesaRepository, TipoDespesaRepository>();
        services.AddScoped<ICC_InquilinoRepository, CC_InquilinoRepository>();
        services.AddScoped<IDespesaRepository, DespesaRepository>();
        services.AddScoped<IHelpManagerRepository, HelpManagerRepository>();
        services.AddScoped<IHistoricoAtualizacaoRendaRepository, HistoricoAtualizacaoRendaRepository>();
        services.AddScoped<IHistoricoEnvioCartasRepository, HistoricoEnvioCartasRepository>(); // 01-2024
        services.AddScoped<IMediadorRepository, MediadorRepository>();
        services.AddScoped<IRecebimentoRepository, RecebimentoRepository>();
        services.AddScoped<ITipoContactoRepository, TipoContactoRepository>();
        services.AddScoped<ITipoDespesaRepository, TipoDespesaRepository>();
        services.AddScoped<ITipoPropriedadeRepository, TipoPropriedadeRepository>();
        services.AddScoped<ITipoRecebimentoRepository, TipoRecebimentoRepository>();

        services.AddScoped<IDocumentsRepository, DocumentsRepository>();

        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
        services.AddScoped<ILookupTableRepository, LookupTableRepository>();
        services.AddScoped<IBackupDBRepository, BackupDBRepository>();

        services.AddScoped<IStatsRepository, StatsRepository>();

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ILetterTemplatesRepository, LetterTemplatesRepository>();
        services.AddScoped<IDistritosConcelhosRepository, DistritosConcelhosRepository>();

        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();

    }
}