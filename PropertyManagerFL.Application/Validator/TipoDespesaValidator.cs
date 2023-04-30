using FluentValidation;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Validator
{
    public class TipoDespesaValidator : AbstractValidator<TipoDespesaVM>
    {
        public TipoDespesaValidator()
        {
            RuleFor(p => p.Descricao)
                .NotNull()
                .NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
                .Length(2, 256).WithMessage("Tamanho ({TotalLength}) inválido na {PropertyName}")
                .Must(BeAValidDescricao).WithMessage("{PropertyName} contém carateres inválidos");

        }

        #region Custom Validators
        protected bool BeAValidDescricao(string descricao)
        {
            descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
            return descricao.All(char.IsLetter);
        }
        #endregion
    }
}
