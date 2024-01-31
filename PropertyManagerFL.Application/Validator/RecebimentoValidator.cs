using FluentValidation;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.Application.Validator
{
    public class RecebimentoValidator : AbstractValidator<RecebimentoVM>
    {
        public RecebimentoValidator()
        {
            RuleFor(p => p.ID_TipoRecebimento)
                .NotNull().WithMessage("Deve selecionar Tipo de Recebimento");
            RuleFor(p => p.ID_Propriedade)
                .NotNull().NotEmpty()
                .GreaterThan(0).WithMessage("Deve selecionar Fração");
            RuleFor(p => p.ValorRecebido)
                .NotNull()
                .NotEmpty().WithMessage("Deve preencher Valor Recebido")
                .GreaterThanOrEqualTo(0).WithMessage("Valor Recebido deve ser um valor positivo");
            RuleFor(p => p.ValorRecebido)
                .NotNull()
                .NotEmpty().WithMessage("Deve preencher Valor Recebido")
                .LessThanOrEqualTo(p=>p.ValorPrevisto).WithMessage("Valor Recebido deve ser inferior ao valor da renda")
                .When(p=>p.Renda);
        }


        #region Custom Validators

        #endregion
    }
}
