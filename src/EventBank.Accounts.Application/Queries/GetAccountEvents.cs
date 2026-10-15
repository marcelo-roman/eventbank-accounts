using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Queries;

public sealed record GetAccountEvents(Guid AccountId) : IQuery<IReadOnlyList<RecordedEvent>>;
