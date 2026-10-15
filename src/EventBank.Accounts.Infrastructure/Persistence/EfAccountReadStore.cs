using EventBank.Accounts.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class EfAccountReadStore(AccountsDbContext dbContext) : IAccountReadStore
{
    public async Task<IReadOnlyList<AccountSummaryView>> ListAccounts(CancellationToken cancellationToken) =>
        await Views(dbContext.AccountSummaries.AsNoTracking().OrderBy(account => account.OpenedAt))
            .ToListAsync(cancellationToken);

    public Task<AccountSummaryView?> FindAccount(Guid accountId, CancellationToken cancellationToken) =>
        Views(dbContext.AccountSummaries.AsNoTracking().Where(account => account.Id == accountId))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<AccountTransactionView>> ListTransactions(Guid accountId, CancellationToken cancellationToken) =>
        await dbContext.AccountTransactions
            .AsNoTracking()
            .Where(transaction => transaction.AccountId == accountId)
            .OrderByDescending(transaction => transaction.Version)
            .Select(transaction => new AccountTransactionView(
                transaction.EventId,
                transaction.Version,
                transaction.Kind,
                transaction.Amount,
                transaction.BalanceAfter,
                transaction.TransferId,
                transaction.Detail,
                transaction.OccurredAt))
            .ToListAsync(cancellationToken);

    private static IQueryable<AccountSummaryView> Views(IQueryable<AccountSummary> accounts) =>
        accounts.Select(account => new AccountSummaryView(
                account.Id,
                account.Holder,
                account.Currency,
                account.Balance,
                account.Status,
                account.Version,
                account.OpenedAt,
                account.UpdatedAt));
}
