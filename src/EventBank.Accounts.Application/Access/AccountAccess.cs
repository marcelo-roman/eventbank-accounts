using EventBank.Accounts.Application.Queries;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Persistence;
using EventBank.BuildingBlocks.Application.Security;

namespace EventBank.Accounts.Application.Access;

public sealed class AccountAccess(ICurrentUser currentUser, IAccountReadStore readStore)
{
    public void EnsureCanMove(BankAccount account)
    {
        if (!account.IsOwnedBy(currentUser.Id))
        {
            throw new ForbiddenException("Only the account owner can move its money.");
        }
    }

    public async Task<AccountSummaryView> EnsureCanRead(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await readStore.FindAccount(accountId, cancellationToken)
            ?? throw new NotFoundException(nameof(BankAccount), accountId);

        if (!currentUser.IsOperator && account.OwnerId != currentUser.Id)
        {
            throw new ForbiddenException("This account belongs to another customer.");
        }

        return account;
    }
}
