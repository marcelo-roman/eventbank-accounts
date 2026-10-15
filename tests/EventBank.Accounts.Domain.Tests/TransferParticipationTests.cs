using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.Domain;
using static EventBank.Accounts.Domain.Tests.AccountHistory;

namespace EventBank.Accounts.Domain.Tests;

public sealed class TransferParticipationTests
{
    [Fact]
    public void Debiting_for_a_transfer_raises_TransferDebited_when_funds_are_available()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 100m));

        account.DebitForTransfer(Payout, Reais(40m));

        account.PendingEvents.ShouldBe([new TransferDebited(Alice.Value, Payout.Value, 40m)]);
        account.Balance.ShouldBe(Reais(60m));
    }

    [Fact]
    public void Debiting_without_funds_records_a_decline_instead_of_throwing()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 10m));

        account.DebitForTransfer(Payout, Reais(40m));

        account.PendingEvents.ShouldBe([
            new TransferDebitDeclined(Alice.Value, Payout.Value, 40m, DeclineReasons.InsufficientFunds),
        ]);
        account.Balance.ShouldBe(Reais(10m));
    }

    [Fact]
    public void Debiting_the_same_transfer_twice_is_a_no_op()
    {
        var account = Given(
            Opened,
            new FundsDeposited(Alice.Value, 100m),
            new TransferDebited(Alice.Value, Payout.Value, 40m));

        account.DebitForTransfer(Payout, Reais(40m));

        account.PendingEvents.ShouldBeEmpty();
        account.Balance.ShouldBe(Reais(60m));
    }

    [Fact]
    public void Crediting_a_closed_account_records_a_decline()
    {
        var account = Given(Opened, new AccountClosed(Alice.Value));

        account.CreditForTransfer(Payout, Reais(40m));

        account.PendingEvents.ShouldBe([
            new TransferCreditDeclined(Alice.Value, Payout.Value, 40m, DeclineReasons.AccountClosed),
        ]);
    }

    [Fact]
    public void Crediting_in_another_currency_records_a_decline()
    {
        var account = Given(Opened);

        account.CreditForTransfer(Payout, Money.Of(40m, Currency.From("EUR")));

        account.PendingEvents.ShouldBe([
            new TransferCreditDeclined(Alice.Value, Payout.Value, 40m, DeclineReasons.CurrencyMismatch),
        ]);
    }

    [Fact]
    public void Refunding_returns_the_debited_amount_once()
    {
        var account = Given(
            Opened,
            new FundsDeposited(Alice.Value, 100m),
            new TransferDebited(Alice.Value, Payout.Value, 40m));

        account.RefundTransfer(Payout);
        account.RefundTransfer(Payout);

        account.PendingEvents.ShouldBe([new TransferRefunded(Alice.Value, Payout.Value, 40m)]);
        account.Balance.ShouldBe(Reais(100m));
    }

    [Fact]
    public void Refunding_a_transfer_that_never_debited_the_account_is_rejected()
    {
        var account = Given(Opened);

        Should.Throw<DomainException>(() => account.RefundTransfer(Payout)).Code.ShouldBe("transfer_not_debited");
    }
}
