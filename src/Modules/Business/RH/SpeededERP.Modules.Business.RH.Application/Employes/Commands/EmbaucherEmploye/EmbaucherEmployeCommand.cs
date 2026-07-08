using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.RH.Application.Employes.Commands.EmbaucherEmploye;

public sealed record EmbaucherEmployeCommand(string Nom, string Prenom, string Email, string Poste, DateTimeOffset DateEmbaucheUtc) : ICommand<Guid>;
