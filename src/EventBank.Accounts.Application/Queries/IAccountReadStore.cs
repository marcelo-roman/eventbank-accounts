namespace EventBank.Accounts.Application.Queries;

public interface IAccountReadStore
{
    Task<IReadOnlyList<AccountSummaryView>> ListAccounts(Guid? ownerId, CancellationToken cancellationToken);

    Task<AccountSummaryView?> FindAccount(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<AccountTransactionView>> ListTransactions(Guid accountId, CancellationToken cancellationToken);
}
