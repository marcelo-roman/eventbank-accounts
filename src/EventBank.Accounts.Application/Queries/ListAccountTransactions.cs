using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed record ListAccountTransactions(Guid AccountId) : IQuery<IReadOnlyList<AccountTransactionView>>;
