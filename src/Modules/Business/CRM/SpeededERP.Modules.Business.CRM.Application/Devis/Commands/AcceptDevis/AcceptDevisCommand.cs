using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.AcceptDevis;

public sealed record AcceptDevisCommand(Guid DevisId) : ICommand;
