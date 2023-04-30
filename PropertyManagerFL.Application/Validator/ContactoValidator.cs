using FluentValidation;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Contactos;

namespace PropertyManagerFL.Application.Validator
{
    public class ContactoValidator : AbstractValidator<ContactoVM>
    {
        public ContactoValidator()
        {
            RuleFor(p => p.Nome)
                .NotNull()
                .NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
                .When(x => x.Nome!= "")
                    .Length(5, 60).WithMessage("Tamanho ({TotalLength}) inválido no Nome")
                // implementar em validação à parte
                //.Must(NotExistInDatabase).WithMessage("{PropertyName} já existe na base de dados")
                .Must(BeAValidDescription).WithMessage("'Nome' contém carateres inválidos");

            RuleFor(p => p.Morada)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Morada, p.f.")
                .When(x => x.Morada != "")
                    .Length(5, 60).WithMessage("Tamanho ({TotalLength}) inválido na Morada")
                // implementar em validação à parte
                //.Must(NotExistInDatabase).WithMessage("{PropertyName} já existe na base de dados")
                .Must(BeAValidDescription).WithMessage("'Morada' contém carateres inválidos");

            RuleFor(p => p.Contacto)
                .NotNull()
                .NotEmpty().WithMessage("Preencha 'Contacto', p.f.")
                .When(p=>p.Contacto != "")
                    .Length(9).WithMessage("'Contacto' deve conter 9 caracteres")
                .Must(BeAValidContact).WithMessage("'Contacto' deve ser numérico");

            RuleFor(p => p.eMail)
                .EmailAddress()
                .When(p => !string.IsNullOrEmpty(p.eMail))
                    .WithMessage("e-mail inválido.");
        }

        #region Custom Validators

        protected bool BeAValidContact(string contacto)
        {
            return contacto.All(char.IsDigit);
        }

        protected bool BeAValidDescription(string descricao)
        {
            descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
            return descricao.All(char.IsLetterOrDigit);
        }

        #endregion
    }
}
