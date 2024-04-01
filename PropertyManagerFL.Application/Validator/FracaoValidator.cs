using FluentValidation;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.ViewModels.Fracoes;

namespace PropertyManagerFL.Application.Validator;

public class FracaoValidator : AbstractValidator<FracaoVM>
{
    public FracaoValidator()
    {
        RuleFor(p => p.Descricao)
            .NotNull().NotEmpty().WithMessage("Preencha descrição")
            .Length(5, 60).WithMessage("Tamanho do campo Descrição ({MinLength}, {MaxLength}) inválido.");
        RuleFor(p => p.Andar)
            .NotNull()
            .NotEmpty().WithMessage("Campo 'Andar' requerido");
        RuleFor(p => p.Lado)
            .NotNull()
            .NotEmpty().WithMessage("Campo 'Lado' requerido");
        RuleFor(p => p.Matriz)
            .NotNull()
            .NotEmpty().WithMessage("Campo 'Matriz' requerido")
            .Length(4, 8).WithMessage("Tamanho do campo Matriz ({TotalLength}) inválido.");
        RuleFor(p => p.AnoUltAvaliacao)
            .NotNull()
            .NotEmpty().WithMessage("Preencha Ano da última avaliação")
            .Length(4).WithMessage("Ano deve conter 4 caracteres...")
            .Must(BeAValidYear).WithMessage("Ano de avaliação inválido");
        RuleFor(p => p.LicencaHabitacao)
            .NotNull()
            .NotEmpty().WithMessage("Preencha Licença de Habitação, p.f.");
        RuleFor(p => p.DataEmissaoLicencaHabitacao)
            .Must(BeAValidDate).WithMessage("Data de emissão da Licença  inválida.")
            .LessThan(x => DateTime.Today)
            .WithMessage("Data de emissão da Licença inválida.");
        RuleFor(p => p.Id_TipoPropriedade)
            .NotNull()
            .GreaterThan(0).WithMessage("Deve selecionar Tipo de Propriedade");
        RuleFor(p => p.Tipologia)
            .NotNull()
            .GreaterThan(0).WithMessage("Deve selecionar Tipologia");
        RuleFor(p => p.Id_Imovel)
            .NotNull()
            .GreaterThan(0).WithMessage("Deve selecionar Imóvel");
        RuleFor(p => p.Situacao)
            .NotNull()
            .GreaterThan(0).WithMessage("Deve selecionar Situação");
        RuleFor(p => p.Conservacao)
            .NotNull()
            .GreaterThan(0).WithMessage("Deve selecionar Conservação");
        RuleFor(p => p.AreaBrutaPrivativa) // area bruta privativa
            .NotNull()
            .NotEmpty().WithMessage("Deve preencher Área Bruta Privativa")
            .GreaterThan(0).WithMessage("Área Bruta Privativa deve ter um valor positivo")
            .GreaterThan(p => p.AreaBrutaDependente).WithMessage("Área Bruta Privativa deverá ser maior que a Área Bruta Dependente");
        RuleFor(p => p.AreaBrutaDependente) // area bruta dependente
            .NotNull()
            .GreaterThanOrEqualTo(0).WithMessage("Área Bruta Dependente não pode ter valor negativo")
            .LessThan(p => p.AreaBrutaPrivativa).WithMessage("Área Bruta Dependente deverá ser menor que a Área Bruta Privativa");
        RuleFor(p => p.ValorUltAvaliacao)
            .NotNull().NotEmpty().GreaterThan(0).WithMessage("Valor da Avaliação inválido");
        RuleFor(p => p.ID_CertificadoEnergetico)
            .NotNull().WithMessage("Certificado Energético requerido");

        // insurance data
            RuleFor(p => p.Apolice.Apolice)
                .NotEmpty()
                .NotNull().NotEmpty().WithMessage("Preencha número da apólice");
            RuleFor(p => p.Apolice.Premio)
                .GreaterThan(0).WithMessage("Valor do prémio da apólice requerido");
    }


    #region Custom Validators
    protected bool BeAValidYear(string year)
    {
        if (DataFormat.IsInteger(year))
        {
            int iAno = DataFormat.GetInteger(year);
            return iAno > 1900 && iAno <= DateTime.Today.Year;
        }

        return false;
    }
    protected bool BeAValidDate(DateTime date)
    {
        return DataFormat.IsValidDate(date);
    }

    protected bool BeAValidDescription(string descricao)
    {
        descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
        return descricao.All(char.IsDigit);
    }

    #endregion
}
