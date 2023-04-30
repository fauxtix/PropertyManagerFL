using FluentValidation;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Validator
{
    public class TipoContactoValidator : AbstractValidator<TipoContacto>
    {
        public TipoContactoValidator()
        {
            RuleFor(p => p.Descricao)
                .NotNull()
                .NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
                .Length(2, 256).WithMessage("Tamanho ({TotalLength}) inválido na {PropertyName}")
                // implementar em validação à parte
                //.Must(NotExistInDatabase).WithMessage("{PropertyName} já existe na base de dados")
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
