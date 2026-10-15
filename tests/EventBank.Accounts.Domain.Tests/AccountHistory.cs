using EventBank.Accounts.Domain.Events;
using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain.Tests;

internal static class AccountHistory
{
    public static readonly AccountId Alice = new(Guid.Parse("0199a000-0000-7000-8000-000000000001"));
    public static readonly TransferId Payout = new(Guid.Parse("0199a000-0000-7000-8000-0000000000aa"));
    public static readonly Currency Real = Currency.From("BRL");

    public static readonly OwnerId Owner = new(Guid.Parse("0199a000-0000-7000-8000-0000000000c1"));
    public static readonly AccountNumber Number = AccountNumber.FromSequence(100001);

    public static readonly AccountOpened Opened = new(Alice.Value, Owner.Value, "Alice Martins", "BRL", "0001", Number.Number);

    public static BankAccount Given(params IDomainEvent[] history)
    {
        var account = (BankAccount)Activator.CreateInstance(typeof(BankAccount), nonPublic: true)!;
        account.LoadFromHistory(history);
        return account;
    }

    public static Money Reais(decimal amount) => Money.Of(amount, Real);
}
