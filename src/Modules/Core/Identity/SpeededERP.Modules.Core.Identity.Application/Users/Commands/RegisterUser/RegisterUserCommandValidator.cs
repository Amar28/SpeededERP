using FluentValidation;

namespace SpeededERP.Modules.Core.Identity.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(128);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(128);
        RuleFor(c => c.InitialPassword).NotEmpty().MinimumLength(12);
    }
}
