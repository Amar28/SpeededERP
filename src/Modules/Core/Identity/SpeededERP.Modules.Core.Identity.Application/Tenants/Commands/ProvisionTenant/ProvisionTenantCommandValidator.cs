using FluentValidation;

namespace SpeededERP.Modules.Core.Identity.Application.Tenants.Commands.ProvisionTenant;

public sealed class ProvisionTenantCommandValidator : AbstractValidator<ProvisionTenantCommand>
{
    public ProvisionTenantCommandValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty().MaximumLength(256);
        RuleFor(c => c.Slug).NotEmpty().Matches("^[a-z0-9-]{3,63}$")
            .WithMessage("Le slug doit être composé de minuscules, chiffres et tirets (3 à 63 caractères).");
    }
}
