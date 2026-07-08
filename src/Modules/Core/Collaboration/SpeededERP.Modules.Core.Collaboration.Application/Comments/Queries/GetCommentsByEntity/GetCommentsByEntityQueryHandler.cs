using MediatR;
using SpeededERP.Modules.Core.Collaboration.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Collaboration.Application.Comments.Queries.GetCommentsByEntity;

public sealed class GetCommentsByEntityQueryHandler(ICommentRepository repository)
    : IRequestHandler<GetCommentsByEntityQuery, Result<IReadOnlyCollection<CommentDto>>>
{
    public async Task<Result<IReadOnlyCollection<CommentDto>>> Handle(GetCommentsByEntityQuery request, CancellationToken cancellationToken)
    {
        var comments = await repository
            .GetByEntityAsync(request.EntityType, request.EntityId, request.TenantId, cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyCollection<CommentDto> dtos = comments
            .Select(c => new CommentDto(c.Id, c.EntityType, c.EntityId, c.AuthorUserId, c.Body, c.IsEdited, c.CreatedOnUtc))
            .ToList();

        return Result.Success(dtos);
    }
}
