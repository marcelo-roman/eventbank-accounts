namespace EventBank.Accounts.Domain;

public sealed record Money
{
    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public Currency Currency { get; }

    public bool IsZero => Amount == 0;

    public static Money Of(decimal amount, Currency currency)
    {
        if (amount <= 0 || decimal.Round(amount, 2) != amount)
        {
            throw AccountErrors.InvalidAmount();
        }

        return new Money(amount, currency);
    }

    public static Money Zero(Currency currency) => new(0, currency);

    internal static Money Restore(decimal amount, Currency currency) => new(amount, currency);

    public bool IsLessThan(Money other) => Amount < other.Amount;

    public Money Add(decimal amount) => new(Amount + amount, Currency);

    public Money Subtract(decimal amount) => new(Amount - amount, Currency);

    public override string ToString() => $"{Currency} {Amount:0.00}";
}
