using FluentValidation;

using System.Linq;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Validator
{
    public class TipoPropriedadeValidator : AbstractValidator<TipoPropriedade>
    {
        public TipoPropriedadeValidator()
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
