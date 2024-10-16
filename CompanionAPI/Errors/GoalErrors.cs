using ErrorOr;

namespace CompanionAPI.Errors;

public static class GoalErrors
{
    public static Error CalcError => Error.Failure(
        code: "Goal.CalcError",
        description: "An error occurred while calculating the goal.");
}