using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed record GetAccount(Guid AccountId) : IQuery<AccountSummaryView?>;
