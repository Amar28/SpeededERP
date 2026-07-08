using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Business.Immobilier.Application.Biens.Commands.MettreEnLocation;

public sealed record MettreEnLocationCommand(Guid BienId) : ICommand;
