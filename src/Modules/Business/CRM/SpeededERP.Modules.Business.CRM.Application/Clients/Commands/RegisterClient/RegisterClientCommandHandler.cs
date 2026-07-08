using System.Text.Json;
using MediatR;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Application.Validation;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;
using SpeededERP.Modules.Business.CRM.Domain.Repositories;
using SpeededERP.Modules.Business.CRM.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Application.Clients.Commands.RegisterClient;

public sealed class RegisterClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork,
    ICustomFieldsValidator customFieldsValidator,
    ITenantContext tenantContext)
    : IRequestHandler<RegisterClientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var addressResult = Address.Create(request.Street, request.City, request.PostalCode, request.CountryIso2);
        if (addressResult.IsFailure)
        {
            return Result.Failure<Guid>(addressResult.Error);
        }

        // The FieldValueValidator gate from section 10 — validated through the shared
        // port so CRM never references Core.Metadata's assembly directly.
        var customFieldsValidation = await customFieldsValidator
            .ValidateAsync("CRM", nameof(Client), tenantContext.TenantId, request.CustomFields, cancellationToken)
            .ConfigureAwait(false);

        if (customFieldsValidation.IsFailure)
        {
            return Result.Failure<Guid>(customFieldsValidation.Error);
        }

        var clientResult = Client.Register(request.Name, addressResult.Value, request.Phone, ClientKind.Prospect);
        if (clientResult.IsFailure)
        {
            return Result.Failure<Guid>(clientResult.Error);
        }

        var client = clientResult.Value;
        client.SetCustomFields(JsonSerializer.Serialize(request.CustomFields));

        repository.Add(client);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(client.Id);
    }
}
