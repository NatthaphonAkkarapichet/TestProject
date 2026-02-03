using FluentValidation;

namespace Backend.Features.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username must contain only letters, numbers, or underscore");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain number");

        RuleFor(x => x.PasswordConfirm)
            .NotEmpty().WithMessage("Password confirmation is required")
            .Equal(x => x.Password)
            .WithMessage("Password confirmation does not match");
    }
}
