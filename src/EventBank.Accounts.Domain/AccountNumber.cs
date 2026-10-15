namespace EventBank.Accounts.Domain;

public sealed record AccountNumber
{
    public const string DefaultBranch = "0001";
    private static readonly int[] Weights = [2, 3, 4, 5, 6, 7];

    private AccountNumber(string branch, string number)
    {
        Branch = branch;
        Number = number;
    }

    public string Branch { get; }

    public string Number { get; }

    public string Formatted => $"{Branch}/{Number}";

    public static AccountNumber FromSequence(long sequence)
    {
        var digits = sequence.ToString("D6", System.Globalization.CultureInfo.InvariantCulture);
        return new AccountNumber(DefaultBranch, $"{digits}-{CheckDigit(digits)}");
    }

    internal static AccountNumber Restore(string branch, string number) => new(branch, number);

    public override string ToString() => Formatted;

    private static int CheckDigit(string digits)
    {
        var sum = digits.Reverse().Select((digit, index) => (digit - '0') * Weights[index % Weights.Length]).Sum();
        var check = 11 - (sum % 11);
        return check >= 10 ? 0 : check;
    }
}
