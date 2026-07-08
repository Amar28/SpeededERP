using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Comptabilite.Application.Factures.Commands.CreerFacture;

public sealed record CreerFactureCommand(Guid ClientId, decimal Montant, string Devise, string? ReferenceExterne) : ICommand<Guid>;
