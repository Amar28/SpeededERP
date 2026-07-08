using SpeededERP.BuildingBlocks.Application.Messaging;
using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.Modules.Core.Documents.Application.Documents.Commands.UploadDocument;

/// <summary>Registers a new document (or a new version, once a companion "re-upload" command is added) pointing at an already-stored blob.</summary>
[RequirePermission("documents:upload")]
public sealed record UploadDocumentCommand(
    string FileName,
    string StorageKey,
    string ContentType,
    long SizeBytes,
    string? EntityType,
    Guid? EntityId) : ICommand<Guid>;
