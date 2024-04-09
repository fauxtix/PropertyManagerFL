using FluentValidation;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.ViewModels.Appointments;

namespace PropertyManagerFL.Application.Validator;
public class ApointmentValidator : AbstractValidator<AppointmentVM>
{
    public ApointmentValidator()
    {
        RuleFor(p => p.Subject)
            .NotEmpty()
            .NotNull()
            .WithMessage("Please fill in ''Subject'");
        //RuleFor(p => p.Location)
        //    .NotEmpty()
        //    .NotNull()
        //    .WithMessage("Please fill in ''Location'");
        //RuleFor(p => p.StartTime)
        //    .GreaterThanOrEqualTo(DateTime.Now)
        //    .Must(BeAValidDate)
        //    .WithMessage("Invalid start time");
        //RuleFor(p => p.EndTime)
        //    .GreaterThanOrEqualTo(p=>p.StartTime)
        //    .Must(BeAValidDate)
        //    .WithMessage("Invalid end time");
    }
    static bool BeAValidDate(DateTime date)
    {
        return DataFormat.IsValidDate(date);
    }

}
