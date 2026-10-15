using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed record ListAccounts : IQuery<IReadOnlyList<AccountSummaryView>>;
