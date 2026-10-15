using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain;

public sealed class BankAccount : AggregateRoot<AccountId>
{
    private readonly HashSet<TransferId> _answeredDebits = [];
    private readonly HashSet<TransferId> _answeredCredits = [];
    private readonly Dictionary<TransferId, decimal> _debitedTransfers = [];
    private readonly HashSet<TransferId> _refundedTransfers = [];
    private readonly HashSet<ChargeId> _answeredCharges = [];

    private BankAccount()
    {
    }

    public OwnerId Owner { get; private set; }

    public AccountNumber Number { get; private set; } = null!;

    public AccountHolder Holder { get; private set; } = null!;

    public Currency Currency { get; private set; } = null!;

    public Money Balance { get; private set; } = null!;

    public AccountStatus Status { get; private set; }

    public static BankAccount Open(AccountId id, OwnerId owner, AccountHolder holder, Currency currency, AccountNumber number)
    {
        var account = new BankAccount();
        account.Raise(new AccountOpened(id.Value, owner.Value, holder.Name, currency.Code, number.Branch, number.Number));
        return account;
    }

    public bool IsOwnedBy(Guid customerId) => Owner.Value == customerId;

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

    public void Charge(ChargeId chargeId, Money amount, string description)
    {
        if (_answeredCharges.Contains(chargeId))
        {
            return;
        }

        var declineReason = DebitDeclineReason(amount);
        if (declineReason is not null)
        {
            Raise(new AccountChargeDeclined(Id.Value, chargeId.Value, amount.Amount, description, declineReason));
            return;
        }

        Raise(new AccountCharged(Id.Value, chargeId.Value, amount.Amount, description));
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
                Owner = new OwnerId(opened.OwnerId);
                Number = AccountNumber.Restore(opened.Branch, opened.Number);
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
            case AccountCharged charged:
                Balance = Balance.Subtract(charged.Amount);
                _answeredCharges.Add(new ChargeId(charged.ChargeId));
                break;
            case AccountChargeDeclined declined:
                _answeredCharges.Add(new ChargeId(declined.ChargeId));
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
