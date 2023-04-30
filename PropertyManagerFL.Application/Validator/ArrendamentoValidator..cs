using FluentValidation;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;

namespace PropertyManagerFL.Application.Validator
{
    public class ArrendamentoValidator : AbstractValidator<ArrendamentoVM>
    {
        public ArrendamentoValidator()
        {
            RuleFor(p => p.ID_Fiador)
                .NotNull()
                .GreaterThan(0).WithMessage("Deve selecionar Fiador");
            RuleFor(p => p.ID_Inquilino)
                .NotNull()
                .GreaterThan(0).WithMessage("Deve selecionar Inquilino");
            RuleFor(p => p.ID_Fracao)
                .NotNull()
                .GreaterThan(0).WithMessage("Deve selecionar Fração");
            RuleFor(p => p.Valor_Renda)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Valor da Renda")
                .GreaterThan(0).WithMessage("Valor da renda deve ter um valor positivo");
            RuleFor(p => p.Prazo_Meses)
                .NotNull()
                .GreaterThan(0).WithMessage("Pazo do contrato deve ter um valor positivo");
            RuleFor(p => p.Data_Inicio)
                .Must(BeAValidDate).WithMessage("Data início inválida (deverá ser inferior à data fim")
                .LessThan(p => p.Data_Fim);
            RuleFor(p => p.Data_Inicio)
                .Must(BeAValidDate).WithMessage("Data início inválida para contratos novos (deverá ser no futuro...)")
                .GreaterThanOrEqualTo(DateTime.Now)
                .When(p => p.ArrendamentoNovo);
            RuleFor(p => p.Data_Inicio)
                .Must(BeAValidDate).WithMessage("Data início inválida para contratos existentes (deverá ser no passado...)")
                .LessThan(DateTime.Now)
                .When(p => p.ArrendamentoNovo == false);
            RuleFor(p => p.Data_Fim)
                .Must(BeAValidDate).WithMessage("Data fim inválida ou prazo incorreto (menos de um ano)")
                .GreaterThan(p => p.Data_Inicio.AddMonths(12));
            RuleFor(p => p.FormaPagamento)
                .NotNull()
                .GreaterThan(0).WithMessage("Deve selecionar Forma de Pagamento");
            RuleFor(p => p.LeiVigente).NotNull().NotEmpty().WithMessage("Lei vigente deverá ser preenchida"); // 03/2023
        }


        #region Custom Validators
        protected bool BeANumber(string descricao)
        {
            descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
            return descricao.All(char.IsDigit);
        }

        protected bool BeAValidDate(DateTime date)
        {
            return DataFormat.IsValidDate(date);
        }
        #endregion
    }
}
