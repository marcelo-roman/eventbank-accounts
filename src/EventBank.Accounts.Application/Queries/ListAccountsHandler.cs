using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Security;

namespace EventBank.Accounts.Application.Queries;

public sealed class ListAccountsHandler(IAccountReadStore readStore, ICurrentUser currentUser)
    : IQueryHandler<ListAccounts, IReadOnlyList<AccountSummaryView>>
{
    public Task<IReadOnlyList<AccountSummaryView>> Handle(ListAccounts query, CancellationToken cancellationToken) =>
        readStore.ListAccounts(currentUser.IsOperator ? null : currentUser.Id, cancellationToken);
}
