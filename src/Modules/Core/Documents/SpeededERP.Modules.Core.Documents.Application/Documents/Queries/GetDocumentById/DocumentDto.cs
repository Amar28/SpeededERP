namespace SpeededERP.Modules.Core.Documents.Application.Documents.Queries.GetDocumentById;

public sealed record DocumentDto(
    Guid Id,
    string FileName,
    string StorageKey,
    string ContentType,
    long SizeBytes,
    int Version,
    string? EntityType,
    Guid? EntityId);
