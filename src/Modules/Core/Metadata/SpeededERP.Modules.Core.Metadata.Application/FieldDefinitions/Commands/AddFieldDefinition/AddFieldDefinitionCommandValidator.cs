using FluentValidation;

namespace SpeededERP.Modules.Core.Metadata.Application.FieldDefinitions.Commands.AddFieldDefinition;

public sealed class AddFieldDefinitionCommandValidator : AbstractValidator<AddFieldDefinitionCommand>
{
    public AddFieldDefinitionCommandValidator()
    {
        RuleFor(c => c.EntityDefinitionId).NotEmpty();
        RuleFor(c => c.Key).NotEmpty().MaximumLength(64);
        RuleFor(c => c.LabelKey).NotEmpty().MaximumLength(256);
        RuleFor(c => c.Order).GreaterThanOrEqualTo(0);
    }
}
