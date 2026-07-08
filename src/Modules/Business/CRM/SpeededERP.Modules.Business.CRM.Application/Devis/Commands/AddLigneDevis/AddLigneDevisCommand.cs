using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.CRM.Application.Devis.Commands.AddLigneDevis;

public sealed record AddLigneDevisCommand(Guid DevisId, string Description, decimal Quantite, decimal PrixUnitaire) : ICommand;
