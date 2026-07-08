namespace SpeededERP.Modules.Business.RH.Presentation.Contracts;

public sealed record EmbaucherEmployeRequest(string Nom, string Prenom, string Email, string Poste, DateTimeOffset DateEmbaucheUtc);
