using EmailService;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.Interfaces.Services.Contract;
using PropertyManagerFL.Application.Interfaces.Services.Email;
using PropertyManagerFL.Application.Interfaces.Services.Stats;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.UI.ApiWrappers;

namespace PropertyManagerFL.UI;

/// <summary>
/// Dependency Injection
/// </summary>
public static class DependencyInjectionConfig
{
    /// <summary>
    /// Add Dependency Injection Configuration
    /// </summary>
    /// <param name="services"></param>
    public static void AddServicesDependencyInjectionConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IValidationService, ValidationService>();
        services.AddTransient<ILookupTableService, WrapperLookupTables>();

        services.AddTransient<IInquilinoService, WrapperInquilinos>();
        services.AddTransient<IFiadorService, WrapperFiadores>();

        services.AddTransient<IProprietarioService, WrapperProprietario>();
        services.AddTransient<IFracaoService, WrapperFracoes>();
        services.AddTransient<IArrendamentoService, WrapperArrendamentos>();
        services.AddTransient<IContratoService, WrapperContratos>();
        services.AddTransient<IImovelService, WrapperImoveis>();
        services.AddTransient<IDocumentosService, WrapperDocumentos>();
        services.AddTransient<IContactosService, WrapperContactos>();
        services.AddTransient<IDespesaService, WrapperDespesas>();
        services.AddTransient<IRecebimentoService, WrapperRecebimentos>();
        services.AddTransient<ICC_InquilinoService, WrapperCC_Inquilinos>();
        services.AddTransient<ITipoDespesaService, WrapperTipoDespesa>();
        services.AddTransient<ITipoContactoService, WrapperTipoContacto>();
        services.AddTransient<ITipoDespesaService, WrapperTipoDespesa>();
        services.AddTransient<ITipoPropriedadeService, WrapperTipoPropriedade>();
        services.AddTransient<ITipoRecebimentoService, WrapperTipoRecebimento>();
        services.AddTransient<IUtilizadorService, WrapperUtilizadores>();
        services.AddTransient<iHelpManagerService, WrapperHelpManager>();
        services.AddTransient<ILogService, WrapperAppLogs>();

        services.AddTransient<ICodigosPostais, WapperCodigosPostais>();

        services.AddTransient<IMailMergeService, WrapperMailMerge>();
        services.AddTransient<IBackupDatabaseService, WrapperBackupDatabase>();
        services.AddTransient<IStatsService, WrapperStats>();
        services.AddTransient<IMessagesService, WrapperMessages>();
        services.AddTransient<IClientEmailService, WrapperEMail>();

        services.AddTransient<ILetterTemplatesService, WrapperLetterTemplates>();
        services.AddTransient<IDistritosConcelhosService, WrapperDistritosConcelhos>();

        services.AddScoped<IEmailSender, EmailSender>();
    }
}