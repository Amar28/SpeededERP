using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.Modules.Business.Immobilier.Domain.Aggregates.Biens;

namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Commands.EnregistrerBien;

public sealed record EnregistrerBienCommand(
    string Street,
    string City,
    string PostalCode,
    string CountryIso2,
    TypeBien TypeBien,
    decimal LoyerMensuel,
    string Devise) : ICommand<Guid>;
