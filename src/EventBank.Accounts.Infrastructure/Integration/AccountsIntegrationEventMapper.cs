using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.EventSourcing.Integration;
using EventBank.BuildingBlocks.EventSourcing.Store;
using Messages = EventBank.Contracts;

namespace EventBank.Accounts.Infrastructure.Integration;

public sealed class AccountsIntegrationEventMapper : IIntegrationEventMapper
{
    public object? Map(EventEnvelope envelope) => envelope.Event switch
    {
        AccountOpened opened => new Messages.AccountOpened(opened.AccountId, opened.OwnerId, opened.Holder, opened.Currency, $"{opened.Branch}/{opened.Number}"),
        AccountCharged charged => new Messages.AccountCharged(charged.ChargeId, charged.AccountId, charged.Amount),
        AccountChargeDeclined declined => new Messages.AccountChargeDeclined(declined.ChargeId, declined.AccountId, declined.Reason),
        AccountClosed closed => new Messages.AccountClosed(closed.AccountId),
        TransferDebited debited => new Messages.AccountDebited(debited.TransferId, debited.AccountId, debited.Amount),
        TransferDebitDeclined declined => new Messages.AccountDebitDeclined(declined.TransferId, declined.AccountId, declined.Reason),
        TransferCredited credited => new Messages.AccountCredited(credited.TransferId, credited.AccountId, credited.Amount),
        TransferCreditDeclined declined => new Messages.AccountCreditDeclined(declined.TransferId, declined.AccountId, declined.Reason),
        TransferRefunded refunded => new Messages.AccountRefunded(refunded.TransferId, refunded.AccountId, refunded.Amount),
        _ => null,
    };
}
