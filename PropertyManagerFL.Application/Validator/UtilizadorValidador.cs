using FluentValidation;
using PropertyManagerFL.Application.ViewModels.Users;

namespace PropertyManagerFL.Application.Validator
{
	public class UtilizadorValidator : AbstractValidator<UserWithConfirmPwd>
	{
		public UtilizadorValidator()
		{
			//CascadeMode = CascadeMode.StopOnFirstFailure;
			RuleFor(p => p.RoleId)
				.NotNull()
				.GreaterThanOrEqualTo(0).WithMessage("Preencha 'Role', p.f.");

			RuleFor(p => p.User_Name)
				.NotNull()
				.NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
				.Length(5, 50).WithMessage("Tamanho ({TotalLength}) inválido na {PropertyName}")
				.Must(BeAValidString).WithMessage("{PropertyName} contém carateres inválidos");

			RuleFor(p => p.First_Name)
				.NotNull()
				.NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
				.Length(8, 50).WithMessage("Tamanho ({TotalLength}) inválido na {PropertyName}")
				.Must(BeAValidString).WithMessage("{PropertyName} contém carateres inválidos");

			RuleFor(p => p.Pwd)
				.NotNull()
				.NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
				.Length(8, 50).WithMessage("Tamanho ({TotalLength}) inválido na {PropertyName}")
				.Must(BeAValidString).WithMessage("{PropertyName} contém carateres inválidos");

			RuleFor(p => p.ConfirmPwd)
				.NotNull()
				.NotEmpty().WithMessage("Preencha {PropertyName}, p.f.")
				.Length(8, 50).WithMessage("Tamanho ({TotalLength}) inválido na {PropertyName}")
				.Must(BeAValidString).WithMessage("{PropertyName} contém carateres inválidos");

			RuleFor(p => p).Custom((p, contexto) =>
			{
				if (p.Pwd != p.ConfirmPwd)
				{
					contexto.AddFailure(nameof(p.Pwd), "Passwords devem ser iguais");
				}
			});

		}

		#region Custom Validators
		protected bool BeAValidString(string name)
		{
			name = name.Replace("'", " ").Replace("-", "").Replace(" ", "");
			return name.All(char.IsLetterOrDigit);
		}

		#endregion
	}
}
