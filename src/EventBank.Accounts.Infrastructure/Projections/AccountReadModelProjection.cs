using EventBank.Accounts.Domain;
using EventBank.Accounts.Domain.Events;
using EventBank.Accounts.Infrastructure.Persistence;
using EventBank.BuildingBlocks.EventSourcing.Projections;
using EventBank.BuildingBlocks.EventSourcing.Store;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Projections;

public sealed class AccountReadModelProjection : IProjection<AccountsDbContext>
{
    public async Task Project(EventEnvelope envelope, AccountsDbContext dbContext, CancellationToken cancellationToken)
    {
        if (envelope.Event is AccountOpened opened)
        {
            dbContext.AccountSummaries.Add(new AccountSummary
            {
                Id = opened.AccountId,
                OwnerId = opened.OwnerId,
                Holder = opened.Holder,
                Branch = opened.Branch,
                Number = opened.Number,
                Currency = opened.Currency,
                Status = nameof(AccountStatus.Open),
                Version = envelope.StreamVersion,
                OpenedAt = envelope.RecordedAt,
                UpdatedAt = envelope.RecordedAt,
            });
            return;
        }

        if (envelope.Event is not IAccountEvent accountEvent)
        {
            return;
        }

        var summary = await dbContext.AccountSummaries.FindAsync([accountEvent.AccountId], cancellationToken)
            ?? throw new InvalidOperationException($"Account summary {accountEvent.AccountId} is missing.");

        summary.Balance += BalanceChange(accountEvent);
        summary.Version = envelope.StreamVersion;
        summary.UpdatedAt = envelope.RecordedAt;
        if (accountEvent is AccountClosed)
        {
            summary.Status = nameof(AccountStatus.Closed);
        }

        var transaction = Transaction(envelope, summary.Balance);
        if (transaction is not null)
        {
            dbContext.AccountTransactions.Add(transaction);
        }
    }

    public async Task Reset(AccountsDbContext dbContext, CancellationToken cancellationToken)
    {
        await dbContext.AccountTransactions.ExecuteDeleteAsync(cancellationToken);
        await dbContext.AccountSummaries.ExecuteDeleteAsync(cancellationToken);
    }

    private static decimal BalanceChange(IAccountEvent accountEvent) => accountEvent switch
    {
        FundsDeposited deposited => deposited.Amount,
        FundsWithdrawn withdrawn => -withdrawn.Amount,
        TransferDebited debited => -debited.Amount,
        TransferCredited credited => credited.Amount,
        TransferRefunded refunded => refunded.Amount,
        AccountCharged charged => -charged.Amount,
        _ => 0,
    };

    private static AccountTransaction? Transaction(EventEnvelope envelope, decimal balanceAfter) => envelope.Event switch
    {
        FundsDeposited deposited => Entry(envelope, "Deposit", deposited.Amount, balanceAfter),
        FundsWithdrawn withdrawn => Entry(envelope, "Withdrawal", -withdrawn.Amount, balanceAfter),
        TransferDebited debited => Entry(envelope, "TransferOut", -debited.Amount, balanceAfter, debited.TransferId),
        TransferDebitDeclined declined => Entry(envelope, "TransferOutDeclined", -declined.Amount, balanceAfter, declined.TransferId, declined.Reason),
        TransferCredited credited => Entry(envelope, "TransferIn", credited.Amount, balanceAfter, credited.TransferId),
        TransferCreditDeclined declined => Entry(envelope, "TransferInDeclined", declined.Amount, balanceAfter, declined.TransferId, declined.Reason),
        TransferRefunded refunded => Entry(envelope, "TransferRefund", refunded.Amount, balanceAfter, refunded.TransferId),
        AccountCharged charged => Entry(envelope, "CardPurchase", -charged.Amount, balanceAfter, description: charged.Description),
        AccountChargeDeclined declined => Entry(envelope, "CardPurchaseDeclined", -declined.Amount, balanceAfter, description: declined.Description, detail: declined.Reason),
        _ => null,
    };

    private static AccountTransaction Entry(
        EventEnvelope envelope,
        string kind,
        decimal amount,
        decimal balanceAfter,
        Guid? transferId = null,
        string? detail = null,
        string? description = null) => new()
        {
            EventId = envelope.EventId,
            AccountId = ((IAccountEvent)envelope.Event).AccountId,
            Version = envelope.StreamVersion,
            Kind = kind,
            Amount = amount,
            BalanceAfter = balanceAfter,
            TransferId = transferId,
            Description = description,
            Detail = detail,
            OccurredAt = envelope.RecordedAt,
        };
}
