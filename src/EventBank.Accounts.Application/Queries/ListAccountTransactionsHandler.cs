using EventBank.Accounts.Application.Access;
using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed class ListAccountTransactionsHandler(IAccountReadStore readStore, AccountAccess access)
    : IQueryHandler<ListAccountTransactions, IReadOnlyList<AccountTransactionView>>
{
    public async Task<IReadOnlyList<AccountTransactionView>> Handle(ListAccountTransactions query, CancellationToken cancellationToken)
    {
        await access.EnsureCanRead(query.AccountId, cancellationToken);
        return await readStore.ListTransactions(query.AccountId, cancellationToken);
    }
}
