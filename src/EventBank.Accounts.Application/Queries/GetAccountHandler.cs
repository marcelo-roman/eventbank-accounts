using EventBank.Accounts.Application.Access;
using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed class GetAccountHandler(AccountAccess access) : IQueryHandler<GetAccount, AccountSummaryView?>
{
    public async Task<AccountSummaryView?> Handle(GetAccount query, CancellationToken cancellationToken) =>
        await access.EnsureCanRead(query.AccountId, cancellationToken);
}
