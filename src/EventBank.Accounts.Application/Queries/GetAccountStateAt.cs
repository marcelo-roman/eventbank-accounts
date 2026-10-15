using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed record GetAccountStateAt(Guid AccountId, long Version) : IQuery<AccountStateView?>;
