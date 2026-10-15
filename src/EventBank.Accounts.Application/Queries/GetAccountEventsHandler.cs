using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Queries;

public sealed class GetAccountEventsHandler(IAggregateHistory<BankAccount, AccountId> history)
    : IQueryHandler<GetAccountEvents, IReadOnlyList<RecordedEvent>>
{
    public Task<IReadOnlyList<RecordedEvent>> Handle(GetAccountEvents query, CancellationToken cancellationToken) =>
        history.ReadEvents(new AccountId(query.AccountId), cancellationToken);
}
