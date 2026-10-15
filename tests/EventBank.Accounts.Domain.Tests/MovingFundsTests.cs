using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.Domain;
using static EventBank.Accounts.Domain.Tests.AccountHistory;

namespace EventBank.Accounts.Domain.Tests;

public sealed class MovingFundsTests
{
    [Fact]
    public void Depositing_raises_FundsDeposited_and_increases_the_balance()
    {
        var account = Given(Opened);

        account.Deposit(Reais(150.25m));

        account.PendingEvents.ShouldBe([new FundsDeposited(Alice.Value, 150.25m)]);
        account.Balance.ShouldBe(Reais(150.25m));
        account.PersistedVersion.ShouldBe(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(10.001)]
    public void Amounts_must_be_positive_with_at_most_two_decimals(decimal amount) =>
        Should.Throw<DomainException>(() => Reais(amount)).Code.ShouldBe("invalid_amount");

    [Fact]
    public void Depositing_in_another_currency_is_rejected()
    {
        var account = Given(Opened);

        Should.Throw<DomainException>(() => account.Deposit(Money.Of(10m, Currency.From("USD"))))
            .Code.ShouldBe("currency_mismatch");
    }

    [Fact]
    public void Withdrawing_more_than_the_balance_is_rejected()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 50m));

        Should.Throw<DomainException>(() => account.Withdraw(Reais(50.01m))).Code.ShouldBe("insufficient_funds");
        account.PendingEvents.ShouldBeEmpty();
    }

    [Fact]
    public void Withdrawing_the_whole_balance_is_allowed()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 50m));

        account.Withdraw(Reais(50m));

        account.PendingEvents.ShouldBe([new FundsWithdrawn(Alice.Value, 50m)]);
        account.Balance.IsZero.ShouldBeTrue();
    }

    [Fact]
    public void Closing_requires_a_zero_balance()
    {
        var account = Given(Opened, new FundsDeposited(Alice.Value, 1m));

        Should.Throw<DomainException>(account.Close).Code.ShouldBe("balance_not_zero");
    }

    [Fact]
    public void A_closed_account_rejects_deposits()
    {
        var account = Given(Opened, new AccountClosed(Alice.Value));

        Should.Throw<DomainException>(() => account.Deposit(Reais(10m))).Code.ShouldBe("account_closed");
    }
}
