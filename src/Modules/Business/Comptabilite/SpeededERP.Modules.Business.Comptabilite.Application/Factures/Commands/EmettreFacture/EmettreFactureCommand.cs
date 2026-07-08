using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Commands.EmettreFacture;

public sealed record EmettreFactureCommand(Guid FactureId) : ICommand;
