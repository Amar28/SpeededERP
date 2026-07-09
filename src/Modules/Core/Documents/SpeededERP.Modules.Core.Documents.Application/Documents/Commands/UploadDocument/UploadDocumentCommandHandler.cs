using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Documents.Domain.Aggregates.Documents;
using SpeededERP.Modules.Core.Documents.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Documents.Application.Documents.Commands.UploadDocument;

public sealed class UploadDocumentCommandHandler(IDocumentRepository repository, [FromKeyedServices("documents")] IUnitOfWork unitOfWork)
    : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var uploadResult = Document.Upload(
            request.FileName,
            request.StorageKey,
            request.ContentType,
            request.SizeBytes,
            request.EntityType,
            request.EntityId);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<Guid>(uploadResult.Error);
        }

        repository.Add(uploadResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(uploadResult.Value.Id);
    }
}
