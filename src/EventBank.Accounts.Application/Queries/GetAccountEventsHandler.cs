using EventBank.Accounts.Application.Access;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Queries;

public sealed class GetAccountEventsHandler(IAggregateHistory<BankAccount, AccountId> history, AccountAccess access)
    : IQueryHandler<GetAccountEvents, IReadOnlyList<RecordedEvent>>
{
    public async Task<IReadOnlyList<RecordedEvent>> Handle(GetAccountEvents query, CancellationToken cancellationToken)
    {
        await access.EnsureCanRead(query.AccountId, cancellationToken);
        return await history.ReadEvents(new AccountId(query.AccountId), cancellationToken);
    }
}
