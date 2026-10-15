using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Queries;

public sealed class ListAccountsHandler(IAccountReadStore readStore)
    : IQueryHandler<ListAccounts, IReadOnlyList<AccountSummaryView>>
{
    public Task<IReadOnlyList<AccountSummaryView>> Handle(ListAccounts query, CancellationToken cancellationToken) =>
        readStore.ListAccounts(cancellationToken);
}
