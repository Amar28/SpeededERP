namespace SpeededERP.Modules.Business.Logistique.Application.Expeditions.Queries.GetExpeditionById;

public sealed record ExpeditionDto(Guid Id, string NumeroSuivi, string TransporteurNom, string Statut);
