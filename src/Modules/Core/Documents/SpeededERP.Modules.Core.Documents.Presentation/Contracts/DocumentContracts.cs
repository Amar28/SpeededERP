namespace SpeededERP.Modules.Core.Documents.Presentation.Contracts;

public sealed record UploadDocumentRequest(
    string FileName,
    string StorageKey,
    string ContentType,
    long SizeBytes,
    string? EntityType,
    Guid? EntityId);
