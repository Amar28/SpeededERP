using MediatR;
using SpeededERP.SharedKernel;

namespace SpeededERP.BuildingBlocks.Application.Messaging;

/// <summary>A command that mutates state and returns no data beyond success/failure.</summary>
public interface ICommand : IRequest<Result>, IBaseCommand;

/// <summary>A command that mutates state and returns <typeparamref name="TResponse"/>.</summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
