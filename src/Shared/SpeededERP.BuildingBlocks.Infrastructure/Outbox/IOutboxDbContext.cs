using Microsoft.EntityFrameworkCore;
using SpeededERP.BuildingBlocks.Application.Outbox;

namespace SpeededERP.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Implemented by every module DbContext that hosts an Outbox table — normally via
/// <see cref="Persistence.AppDbContextBase"/>, except Core.Identity's
/// <c>IdentityModuleDbContext</c> which must inherit ASP.NET Core Identity's own
/// DbContext base instead and therefore implements this interface directly.
/// Lets <see cref="OutboxProcessor{TDbContext}"/> stay agnostic of which base class
/// a given module's DbContext uses.
/// </summary>
public interface IOutboxDbContext : IOutboxWriter
{
    DbSet<OutboxMessage> OutboxMessages { get; }
}
