using SpeededERP.BuildingBlocks.Application.Messaging;

namespace SpeededERP.Modules.Core.Documents.Application.Documents.Queries.GetDocumentById;

public sealed record GetDocumentByIdQuery(Guid DocumentId) : IQuery<DocumentDto>;
