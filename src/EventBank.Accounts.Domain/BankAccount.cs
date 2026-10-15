using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain;

public sealed class BankAccount : AggregateRoot<AccountId>
{
    private readonly HashSet<TransferId> _answeredDebits = [];
    private readonly HashSet<TransferId> _answeredCredits = [];
    private readonly Dictionary<TransferId, decimal> _debitedTransfers = [];
    private readonly HashSet<TransferId> _refundedTransfers = [];

    private BankAccount()
    {
    }

    public AccountHolder Holder { get; private set; } = null!;

    public Currency Currency { get; private set; } = null!;

    public Money Balance { get; private set; } = null!;

    public AccountStatus Status { get; private set; }

    public static BankAccount Open(AccountId id, AccountHolder holder, Currency currency)
    {
        var account = new BankAccount();
        account.Raise(new AccountOpened(id.Value, holder.Name, currency.Code));
        return account;
    }

    public void Deposit(Money amount)
    {
        EnsureCanMove(amount);
        Raise(new FundsDeposited(Id.Value, amount.Amount));
    }

    public void Withdraw(Money amount)
    {
        EnsureCanMove(amount);
        if (Balance.IsLessThan(amount))
        {
            throw AccountErrors.InsufficientFunds();
        }

        Raise(new FundsWithdrawn(Id.Value, amount.Amount));
    }

    public void DebitForTransfer(TransferId transferId, Money amount)
    {
        if (_answeredDebits.Contains(transferId))
        {
            return;
        }

        var declineReason = DebitDeclineReason(amount);
        if (declineReason is not null)
        {
            Raise(new TransferDebitDeclined(Id.Value, transferId.Value, amount.Amount, declineReason));
            return;
        }

        Raise(new TransferDebited(Id.Value, transferId.Value, amount.Amount));
    }

    public void CreditForTransfer(TransferId transferId, Money amount)
    {
        if (_answeredCredits.Contains(transferId))
        {
            return;
        }

        var declineReason = CreditDeclineReason(amount);
        if (declineReason is not null)
        {
            Raise(new TransferCreditDeclined(Id.Value, transferId.Value, amount.Amount, declineReason));
            return;
        }

        Raise(new TransferCredited(Id.Value, transferId.Value, amount.Amount));
    }

    public void RefundTransfer(TransferId transferId)
    {
        if (_refundedTransfers.Contains(transferId))
        {
            return;
        }

        if (!_debitedTransfers.TryGetValue(transferId, out var amount))
        {
            throw AccountErrors.TransferNotDebited(transferId);
        }

        Raise(new TransferRefunded(Id.Value, transferId.Value, amount));
    }

    public void Close()
    {
        EnsureOpen();
        if (!Balance.IsZero)
        {
            throw AccountErrors.BalanceNotZero();
        }

        Raise(new AccountClosed(Id.Value));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case AccountOpened opened:
                Id = new AccountId(opened.AccountId);
                Holder = AccountHolder.Restore(opened.Holder);
                Currency = Currency.Restore(opened.Currency);
                Balance = Money.Zero(Currency);
                Status = AccountStatus.Open;
                break;
            case FundsDeposited deposited:
                Balance = Balance.Add(deposited.Amount);
                break;
            case FundsWithdrawn withdrawn:
                Balance = Balance.Subtract(withdrawn.Amount);
                break;
            case TransferDebited debited:
                Balance = Balance.Subtract(debited.Amount);
                _answeredDebits.Add(new TransferId(debited.TransferId));
                _debitedTransfers[new TransferId(debited.TransferId)] = debited.Amount;
                break;
            case TransferDebitDeclined declined:
                _answeredDebits.Add(new TransferId(declined.TransferId));
                break;
            case TransferCredited credited:
                Balance = Balance.Add(credited.Amount);
                _answeredCredits.Add(new TransferId(credited.TransferId));
                break;
            case TransferCreditDeclined declined:
                _answeredCredits.Add(new TransferId(declined.TransferId));
                break;
            case TransferRefunded refunded:
                Balance = Balance.Add(refunded.Amount);
                _refundedTransfers.Add(new TransferId(refunded.TransferId));
                break;
            case AccountClosed:
                Status = AccountStatus.Closed;
                break;
            default:
                throw new InvalidOperationException($"{nameof(BankAccount)} cannot apply {domainEvent.GetType().Name}.");
        }
    }

    private string? DebitDeclineReason(Money amount)
    {
        if (Status == AccountStatus.Closed)
        {
            return DeclineReasons.AccountClosed;
        }

        if (amount.Currency != Currency)
        {
            return DeclineReasons.CurrencyMismatch;
        }

        return Balance.IsLessThan(amount) ? DeclineReasons.InsufficientFunds : null;
    }

    private string? CreditDeclineReason(Money amount)
    {
        if (Status == AccountStatus.Closed)
        {
            return DeclineReasons.AccountClosed;
        }

        return amount.Currency != Currency ? DeclineReasons.CurrencyMismatch : null;
    }

    private void EnsureCanMove(Money amount)
    {
        EnsureOpen();
        if (amount.Currency != Currency)
        {
            throw AccountErrors.CurrencyMismatch(Currency, amount.Currency);
        }
    }

    private void EnsureOpen()
    {
        if (Status == AccountStatus.Closed)
        {
            throw AccountErrors.AccountClosed();
        }
    }
}
