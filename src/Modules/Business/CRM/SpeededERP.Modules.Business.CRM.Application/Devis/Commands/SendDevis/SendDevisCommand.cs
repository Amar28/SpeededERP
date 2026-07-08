using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.SendDevis;

public sealed record SendDevisCommand(Guid DevisId) : ICommand;
