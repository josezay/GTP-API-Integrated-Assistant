using CompanionAPI.Common.Validators;
using CompanionAPI.Contracts.ActivityContracts;
using FluentValidation;

namespace CompanionAPI.Contracts.OnboardingContracts;

public class UserOnboardingRequestValidator : AbstractValidator<UserOnboardingRequest>
{
    public UserOnboardingRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(Validators.BeAValidEmail).WithMessage("Email is not valid.");
        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(Validators.BeAValidGender).WithMessage("Gender must be 'F' or 'M'.");
        RuleFor(x => x.Age).InclusiveBetween(1, 100).WithMessage("Age must be between 1 and 100.");
        RuleFor(x => x.Weight).GreaterThan(0).WithMessage("Weight must be greater than 0.");
        RuleFor(x => x.Height).GreaterThan(0).WithMessage("Height must be greater than 0.");

        RuleForEach(x => x.Activities).SetValidator(new ActivityRequestValidator());
    }
}
