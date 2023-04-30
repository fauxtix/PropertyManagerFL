using FluentValidation;
using PropertyManagerFL.Application.ViewModels.Despesas;

namespace PropertyManagerFL.Application.Validator
{
    public class DespesaValidator : AbstractValidator<DespesaVM>
    {
        public DespesaValidator()
        {
            RuleFor(p => p.ID_CategoriaDespesa)
                .NotNull()
                .GreaterThan(0).WithMessage("Deve selecionar Categoria");
            RuleFor(p => p.ID_TipoDespesa)
                .NotNull()
                .GreaterThan(0).WithMessage("Deve selecionar Tipo de Despesa");
            RuleFor(p => p.Valor_Pago)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Valor da despesa")
                .GreaterThan(0).WithMessage("Valor da despesa deve ter um valor positivo");
            RuleFor(p=>p.DataMovimento)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Data de pagamento inválida");
        }


        #region Custom Validators

        #endregion
    }
}
