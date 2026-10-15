namespace EventBank.Accounts.Domain;

public sealed record Currency
{
    public static readonly IReadOnlyList<string> SupportedCodes = ["BRL", "USD", "EUR"];

    private Currency(string code) => Code = code;

    public string Code { get; }

    public static Currency From(string? code)
    {
        var normalized = code?.Trim().ToUpperInvariant();
        if (normalized is null || !SupportedCodes.Contains(normalized))
        {
            throw AccountErrors.UnsupportedCurrency(code);
        }

        return new Currency(normalized);
    }

    internal static Currency Restore(string code) => new(code);

    public override string ToString() => Code;
}
