using FluentValidation;

namespace CompanionAPI.Contracts.Requests.UserOnboardingRequest;

public class UserOnboardingRequestValidator : AbstractValidator<UserOnboardingRequest>
{
    public UserOnboardingRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender is required.");
        RuleFor(x => x.Age).InclusiveBetween(18, 100).WithMessage("Age must be between 18 and 100.");
        RuleFor(x => x.Weight).GreaterThan(0).WithMessage("Weight must be greater than 0.");
        RuleFor(x => x.Height).GreaterThan(0).WithMessage("Height must be greater than 0.");
        RuleFor(x => x.Goal).NotEmpty().WithMessage("Goal is required.");
    }
}
