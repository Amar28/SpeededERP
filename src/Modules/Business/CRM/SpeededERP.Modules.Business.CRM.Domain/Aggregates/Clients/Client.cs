using Ardalis.GuardClauses;
using SpeededERP.Modules.Business.CRM.Domain.Events;
using SpeededERP.Modules.Business.CRM.Domain.ValueObjects;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Business.CRM.Domain.Aggregates.Clients;

public enum ClientKind
{
    Prospect,
    Client,
}

/// <summary>
/// The textbook illustration of architecture section 10's hybrid model: <see cref="Name"/>,
/// <see cref="Address"/> and <see cref="Phone"/> are native, code-owned columns with real
/// invariants; <see cref="CustomFieldsJson"/> is the tenant-extensible JSONB bag (e.g.
/// "Numéro fiscal", "Secteur" for one tenant, nothing for another — same entity, same
/// code, zero redeployment). Validated before assignment by
/// <c>BuildingBlocks.Application.Validation.ICustomFieldsValidator</c>, never inside
/// the aggregate itself (the aggregate doesn't know about FieldDefinition at all).
/// </summary>
public sealed class Client : AggregateRoot<Guid>, IAuditable, IMustHaveTenant, ISoftDeletable
{
    public string Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public string? Phone { get; private set; }

    public ClientKind Kind { get; private set; }

    /// <summary>Raw JSON — validated at the Application boundary, stored as-is (architecture section 10).</summary>
    public string CustomFieldsJson { get; private set; } = "{}";

    public Guid TenantId { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? DeletedOnUtc { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset? LastModifiedOnUtc { get; private set; }

    public string? LastModifiedBy { get; private set; }

    private Client()
    {
    }

    private Client(Guid id, string name, Address address, string? phone, ClientKind kind)
        : base(id)
    {
        Name = name;
        Address = address;
        Phone = phone;
        Kind = kind;
    }

    public static Result<Client> Register(string name, Address address, string? phone, ClientKind kind)
    {
        Guard.Against.NullOrWhiteSpace(name);

        var client = new Client(Guid.NewGuid(), name.Trim(), address, phone, kind);
        client.Raise(new ClientRegisteredDomainEvent(client.Id, client.TenantId));

        return Result.Success(client);
    }

    public void Rename(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void Relocate(Address address) => Address = address;

    public void UpdatePhone(string? phone) => Phone = phone;

    public void PromoteToClient() => Kind = ClientKind.Client;

    /// <summary>Caller must have already validated <paramref name="customFieldsJson"/> via ICustomFieldsValidator.</summary>
    public void SetCustomFields(string customFieldsJson) => CustomFieldsJson = customFieldsJson;

    public void AssignTenant(Guid tenantId) => TenantId = tenantId;

    public void MarkDeleted(DateTimeOffset onUtc)
    {
        IsDeleted = true;
        DeletedOnUtc = onUtc;
    }

    public void SetCreated(DateTimeOffset onUtc, string? by)
    {
        CreatedOnUtc = onUtc;
        CreatedBy = by;
    }

    public void SetModified(DateTimeOffset onUtc, string? by)
    {
        LastModifiedOnUtc = onUtc;
        LastModifiedBy = by;
    }
}
