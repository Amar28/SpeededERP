using MediatR;
using SpeededERP.Modules.Core.Documents.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Documents.Application.Documents.Queries.GetDocumentById;

public sealed class GetDocumentByIdQueryHandler(IDocumentRepository repository)
    : IRequestHandler<GetDocumentByIdQuery, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await repository.GetByIdAsync(request.DocumentId, cancellationToken).ConfigureAwait(false);
        if (document is null)
        {
            return Result.Failure<DocumentDto>(Error.NotFound("Document.NotFound", "Document introuvable."));
        }

        return Result.Success(new DocumentDto(
            document.Id,
            document.FileName,
            document.StorageKey,
            document.ContentType,
            document.SizeBytes,
            document.Version,
            document.EntityType,
            document.EntityId));
    }
}
