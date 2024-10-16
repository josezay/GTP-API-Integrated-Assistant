﻿using FluentValidation;
using CompanionAPI.Common.Validators;

namespace CompanionAPI.Contracts.ExerciseContracts;

public class ExerciseRequestValidator : AbstractValidator<ExerciseRequest>
{
    public ExerciseRequestValidator()
    {
        RuleFor(x => x.ActivityName).NotEmpty().WithMessage("Activity name is required.");
        RuleFor(x => x.WeeklyFrequency).GreaterThan(0).WithMessage("Weekly frequency must be greater than 0.");
        RuleFor(x => x.DurationInMinutes).GreaterThan(0).WithMessage("Duration in minutes must be greater than 0.");
    }
}
