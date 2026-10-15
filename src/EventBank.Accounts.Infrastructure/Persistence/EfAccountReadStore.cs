using EventBank.Accounts.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class EfAccountReadStore(AccountsDbContext dbContext) : IAccountReadStore
{
    public async Task<IReadOnlyList<AccountSummaryView>> ListAccounts(Guid? ownerId, CancellationToken cancellationToken)
    {
        var accounts = dbContext.AccountSummaries.AsNoTracking();
        if (ownerId is { } owner)
        {
            accounts = accounts.Where(account => account.OwnerId == owner);
        }

        return await Views(accounts.OrderBy(account => account.OpenedAt)).ToListAsync(cancellationToken);
    }

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
                transaction.Description,
                transaction.Detail,
                transaction.OccurredAt))
            .ToListAsync(cancellationToken);

    private static IQueryable<AccountSummaryView> Views(IQueryable<AccountSummary> accounts) =>
        accounts.Select(account => new AccountSummaryView(
                account.Id,
                account.OwnerId,
                account.Holder,
                account.Branch,
                account.Number,
                account.Currency,
                account.Balance,
                account.Status,
                account.Version,
                account.OpenedAt,
                account.UpdatedAt));
}
