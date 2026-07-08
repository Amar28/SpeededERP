using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Commands.ConfirmerLivraison;

public sealed record ConfirmerLivraisonCommand(Guid ExpeditionId) : ICommand;
