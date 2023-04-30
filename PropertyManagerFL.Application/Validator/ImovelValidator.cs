using FluentValidation;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.ViewModels.Imoveis;

namespace PropertyManagerFL.Application.Validator
{
    public class ImovelValidator : AbstractValidator<ImovelVM>
    {
        public ImovelValidator()
        {
            RuleFor(p => p.Descricao)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Descrição do Imóvel")
                .Length(4, 60).WithMessage("Tamanho do campo Descrição ({MinLength}, {MaxLength}) inválido.");
            RuleFor(p => p.Morada)
                .NotNull()
                .NotEmpty().WithMessage("Preencha campo Morada")
                .MaximumLength(60).WithMessage("Tamanho do campo Morada ({MaxLength}) inválido.");
            RuleFor(p => p.AnoConstrucao)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Ano de Construção do Imóvel")
                .Length(4).WithMessage("Ano deve conter 4 caracteres...")
                .Must(BeAValidYear).WithMessage("Ano de avaliação inválido");
            RuleFor(p => p.Numero)
                .NotNull()
                .NotEmpty().WithMessage("Preencha nº da porta, p.f.");
            RuleFor(p => p.CodPst)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Código Postal")
                .Length(4).WithMessage("Cod. Pst. deve conter 4 caracteres...")
                .Must(BeANumber).WithMessage("Código Postal inválido (não numérico)");
            RuleFor(p => p.CodPstEx)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Sub Código Postal")
                .Length(3).WithMessage("Cod. Pst. deve conter 3 caracteres...")
                .Must(BeANumber).WithMessage("Código Postal inválido (não numérico)");
            RuleFor(p => p.FreguesiaImovel)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Freguesia, p.f.");
            RuleFor(p => p.ConcelhoImovel)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Concelho, p.f.");
            RuleFor(p => p.Conservacao)
                .NotNull().WithMessage("Estado de Conservação requerido");
            RuleFor(p => p.DataUltimaInspecaoGas)
                .Must(BeAValidDate).WithMessage("Data da última inspeção do gás inválida.")
                .LessThan(x => DateTime.Today)
                .WithMessage("Data da última inspeção do gás inválida.");

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

        protected bool BeANumber(string descricao)
        {
            descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
            return descricao.All(char.IsDigit);
        }

        protected bool BeAValidDescription(string descricao)
        {
            descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
            return descricao.All(char.IsLetterOrDigit);
        }

        #endregion
    }
}
