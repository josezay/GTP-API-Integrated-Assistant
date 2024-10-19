using ErrorOr;

namespace CompanionAPI.Errors;

public static class UserErrors
{
    public static Error UserAlreadyExists => Error.Validation(
        code: "User.Duplicate",
        description: "User already registred");

    public static Error UserNotExists => Error.NotFound(
        code: "User.NotFound",
        description: "User not found");
}