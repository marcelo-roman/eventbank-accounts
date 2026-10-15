using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.Domain;
using static EventBank.Accounts.Domain.Tests.AccountHistory;

namespace EventBank.Accounts.Domain.Tests;

public sealed class OpeningAccountTests
{
    [Fact]
    public void Opening_raises_AccountOpened_with_normalized_values()
    {
        var account = BankAccount.Open(Alice, AccountHolder.Create("  Alice Martins "), Currency.From("brl"));

        account.PendingEvents.ShouldBe([Opened]);
        account.Balance.ShouldBe(Money.Zero(Real));
        account.Status.ShouldBe(AccountStatus.Open);
        account.Version.ShouldBe(1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Al")]
    public void Opening_rejects_a_holder_name_that_is_too_short(string? holder) =>
        Should.Throw<DomainException>(() => AccountHolder.Create(holder)).Code.ShouldBe("invalid_holder");

    [Fact]
    public void Opening_rejects_an_unsupported_currency() =>
        Should.Throw<DomainException>(() => Currency.From("JPY")).Code.ShouldBe("unsupported_currency");

    [Fact]
    public void Rehydrating_from_history_rebuilds_state_without_pending_events()
    {
        var account = Given(
            Opened,
            new FundsDeposited(Alice.Value, 300m),
            new FundsWithdrawn(Alice.Value, 120m));

        account.Balance.ShouldBe(Reais(180m));
        account.Version.ShouldBe(3);
        account.PersistedVersion.ShouldBe(3);
        account.PendingEvents.ShouldBeEmpty();
    }
}
