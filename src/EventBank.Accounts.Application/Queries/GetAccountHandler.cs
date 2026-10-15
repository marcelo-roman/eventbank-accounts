using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed class GetAccountHandler(IAccountReadStore readStore) : IQueryHandler<GetAccount, AccountSummaryView?>
{
    public Task<AccountSummaryView?> Handle(GetAccount query, CancellationToken cancellationToken) =>
        readStore.FindAccount(query.AccountId, cancellationToken);
}
