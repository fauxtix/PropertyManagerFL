using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PropertyManagerFL.Application.Validator;
using System.Globalization;
using System.Text.Json.Serialization;

namespace PropertyManagerFL.UI;

public static class FluentValidationConfig
{
    public static void AddFluentValidationConfiguration(this IServiceCollection services)
    {
        //services.AddFluentValidationAutoValidation()
        //    .AddFluentValidationClientsideAdapters()
        //    .AddFluentValidationRulesToSwagger()
        //    .AddValidatorsFromAssemblyContaining<InquilinoValidator>();

        services.AddControllers()
            .AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                x.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            .AddJsonOptions(p => p.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .AddFluentValidation(p =>
           {
               p.RegisterValidatorsFromAssemblyContaining<InquilinoValidator>();
               p.RegisterValidatorsFromAssemblyContaining<FiadorValidator>();
               p.RegisterValidatorsFromAssemblyContaining< ArrendamentoValidator> ();
               p.RegisterValidatorsFromAssemblyContaining<CategoriaDespesaValidator>();
               p.RegisterValidatorsFromAssemblyContaining<ContactoValidator>();
               p.RegisterValidatorsFromAssemblyContaining< DespesaValidator> ();
               p.RegisterValidatorsFromAssemblyContaining<FracaoValidator>();
               p.RegisterValidatorsFromAssemblyContaining<ImovelValidator>();
               p.RegisterValidatorsFromAssemblyContaining<ProprietarioValidator>();
               p.RegisterValidatorsFromAssemblyContaining<RecebimentoValidator>();
               p.RegisterValidatorsFromAssemblyContaining<TipoContactoValidator>();
               p.RegisterValidatorsFromAssemblyContaining<TipoDespesaValidator>();
               p.RegisterValidatorsFromAssemblyContaining<TipoPropriedadeValidator>();
               p.RegisterValidatorsFromAssemblyContaining<ProprietarioValidator>();
               p.RegisterValidatorsFromAssemblyContaining<TipoRecebimentoValidator>();
               p.ValidatorOptions.LanguageManager.Culture = new CultureInfo("pt-PT");
           });

        services.AddFluentValidationRulesToSwagger();
    }
}