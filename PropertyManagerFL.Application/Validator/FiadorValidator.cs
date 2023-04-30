using FluentValidation;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.ViewModels.Fiadores;

namespace PropertyManagerFL.Application.Validator
{
    public class FiadorValidator : AbstractValidator<FiadorVM>
    {
        public FiadorValidator()
        {
            RuleFor(p => p.Nome)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Nome, p.f.")
                .Length(5, 60).WithMessage("Tamanho do Nome {TotalLength}) inválido")
                // implementar em validação à parte
                .Must(BeAValidDescription).WithMessage("Nome contém carateres inválidos");

            RuleFor(m => m.Contacto1)
                .NotEmpty()
                .Unless(m => !string.IsNullOrEmpty(m.Contacto2));
            RuleFor(m => m.Contacto2).
                NotEmpty()
                .Unless(m => !string.IsNullOrEmpty(m.Contacto1));

            RuleFor(m => m.NIF)
                .NotEmpty()
                .Length(9).WithMessage("NIF deve ter 9 dígitos...")
                .Must(BeAValidNIF).WithMessage("NIF inválido.");

            RuleFor(m => m.Identificacao)
                .NotNull()
                .NotEmpty().WithMessage("Preencha nº de Identificação, p.f.");

            RuleFor(p => p.eMail)
                .EmailAddress()
                .When(p => !string.IsNullOrEmpty(p.eMail))
                .WithMessage("e-mail inválido.");

            RuleFor(p => p.IRSAnual)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Valor do IRS anual, p.f.")
                .GreaterThan(15000).WithMessage("Valor do IRS anual deverá ser superior a 15.000€");
            RuleFor(p => p.Vencimento)
                .NotNull()
                .NotEmpty().WithMessage("Preencha Valor do vencimento mensal, p.f.")
                .GreaterThan(750).WithMessage("Valor do vencimento mensal deverá ser superior a 750€");

        }


        #region Custom Validators

        protected bool BeAValidDate(DateTime date)
        {
            return DataFormat.IsValidDate(date);
        }

        private bool BeAValidNIF(string sNIF)
        {
            if (sNIF.Length != 9)
                return false;

            bool bRet = false;
            string[] s = new string[9];
            string Ss = null;
            string C = null;
            int i = 0;
            long checkDigit = 0;

            s[0] = Convert.ToString(sNIF[0]);
            s[1] = Convert.ToString(sNIF[1]);
            s[2] = Convert.ToString(sNIF[2]);
            s[3] = Convert.ToString(sNIF[3]);
            s[4] = Convert.ToString(sNIF[4]);
            s[5] = Convert.ToString(sNIF[5]);
            s[6] = Convert.ToString(sNIF[6]);
            s[7] = Convert.ToString(sNIF[7]);
            s[8] = Convert.ToString(sNIF[8]);

            C = s[0];
            if (s[0] == "1" || s[0] == "2" || s[0] == "5" || s[0] == "6" || s[0] == "9")
            {
                checkDigit = Convert.ToInt32(C) * 9;
                for (i = 2; i <= 8; i++)
                {
                    checkDigit = checkDigit + (Convert.ToInt32(s[i - 1]) * (10 - i));
                }
                checkDigit = 11 - (checkDigit % 11);
                if ((checkDigit >= 10))
                    checkDigit = 0;
                Ss = s[0] + s[1] + s[2] + s[3] + s[4] + s[5] + s[6] + s[7] + s[8];
                if ((checkDigit == Convert.ToInt32(s[8])))
                    bRet = true;
            }

            return bRet;
        }

        protected bool BeAValidDescription(string descricao)
        {
            descricao = descricao.Replace("'", " ").Replace("-", "").Replace(" ", "");
            return descricao.All(char.IsLetterOrDigit);
        }

        #endregion
    }
}
