using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed class ListAccountTransactionsHandler(IAccountReadStore readStore)
    : IQueryHandler<ListAccountTransactions, IReadOnlyList<AccountTransactionView>>
{
    public Task<IReadOnlyList<AccountTransactionView>> Handle(ListAccountTransactions query, CancellationToken cancellationToken) =>
        readStore.ListTransactions(query.AccountId, cancellationToken);
}
