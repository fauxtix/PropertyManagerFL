using FluentValidation;
using PropertyManagerFL.Application.ViewModels.Documentos;

namespace PropertyManagerFL.Application.Validator
{
    public class DocumentoValidator : AbstractValidator<DocumentoVM>
    {
        public DocumentoValidator()
        {
            RuleFor(p => p.Title)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Descição, p.f.")
                .When(x => x.Description != "")
                .Length(5, 256).WithMessage("Tamanho ({TotalLength}) inválido no Título");

            RuleFor(p => p.Description)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Título, p.f.")
                .When(x => x.Title != "").
                MinimumLength(5).WithMessage("Tamanho mínimo ({TotalLength}) inválido na Descrição");
            RuleFor(p => p.URL)
                .NotNull()
                .NotEmpty().WithMessage("Escolha documento, p.f.");
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
