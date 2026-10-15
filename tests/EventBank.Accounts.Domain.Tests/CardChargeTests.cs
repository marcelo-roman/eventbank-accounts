using EventBank.Accounts.Domain.Events;
using static EventBank.Accounts.Domain.Tests.AccountHistory;

namespace EventBank.Accounts.Domain.Tests;

public sealed class CardChargeTests
{
    private static readonly ChargeId Purchase = new(Guid.Parse("0199a000-0000-7000-8000-0000000000d1"));

    [Fact]
    public void A_card_purchase_is_charged_when_the_balance_covers_it()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 100m));

        account.Charge(Purchase, Reais(42.9m), "Card 4242 at Padaria");

        account.PendingEvents.ShouldBe([new AccountCharged(Alice.Value, Purchase.Value, 42.9m, "Card 4242 at Padaria")]);
        account.Balance.ShouldBe(Reais(57.1m));
    }

    [Fact]
    public void A_card_purchase_without_funds_is_declined_as_a_fact()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 10m));

        account.Charge(Purchase, Reais(42.9m), "Card 4242 at Padaria");

        account.PendingEvents.ShouldBe([
            new AccountChargeDeclined(Alice.Value, Purchase.Value, 42.9m, "Card 4242 at Padaria", DeclineReasons.InsufficientFunds),
        ]);
    }

    [Fact]
    public void Charging_the_same_purchase_twice_is_a_no_op()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 100m), new AccountCharged(Alice.Value, Purchase.Value, 42.9m, "x"));

        account.Charge(Purchase, Reais(42.9m), "x");

        account.PendingEvents.ShouldBeEmpty();
    }
}
