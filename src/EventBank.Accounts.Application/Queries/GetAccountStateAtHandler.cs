using EventBank.Accounts.Application.Access;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Queries;

public sealed class GetAccountStateAtHandler(IAggregateHistory<BankAccount, AccountId> history, AccountAccess access)
    : IQueryHandler<GetAccountStateAt, AccountStateView?>
{
    public async Task<AccountStateView?> Handle(GetAccountStateAt query, CancellationToken cancellationToken)
    {
        await access.EnsureCanRead(query.AccountId, cancellationToken);
        var account = await history.LoadAtVersion(new AccountId(query.AccountId), query.Version, cancellationToken);
        return account is null ? null : AccountStateView.From(account);
    }
}
