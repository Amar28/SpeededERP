using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.CreateDevis;

public sealed record CreateDevisCommand(Guid ClientId, string Devise) : ICommand<Guid>;
