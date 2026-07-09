using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SpeededERP.BuildingBlocks.Domain;
using SpeededERP.Modules.Core.Collaboration.Domain.Aggregates.Comments;
using SpeededERP.Modules.Core.Collaboration.Domain.Repositories;
using SpeededERP.SharedKernel;

namespace SpeededERP.Modules.Core.Collaboration.Application.Comments.Commands.PostComment;

public sealed class PostCommentCommandHandler(ICommentRepository repository, [FromKeyedServices("collaboration")] IUnitOfWork unitOfWork)
    : IRequestHandler<PostCommentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(PostCommentCommand request, CancellationToken cancellationToken)
    {
        var postResult = Comment.Post(request.EntityType, request.EntityId, request.AuthorUserId, request.Body);
        if (postResult.IsFailure)
        {
            return Result.Failure<Guid>(postResult.Error);
        }

        repository.Add(postResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(postResult.Value.Id);
    }
}
