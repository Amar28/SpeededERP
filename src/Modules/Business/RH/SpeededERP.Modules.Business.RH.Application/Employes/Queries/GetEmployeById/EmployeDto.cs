namespace SpeededERP.Modules.Business.RH.Application.Employes.Queries.GetEmployeById;

public sealed record EmployeDto(
    Guid Id,
    string Nom,
    string Prenom,
    string Email,
    string Poste,
    DateTimeOffset DateEmbaucheUtc,
    string Statut);
