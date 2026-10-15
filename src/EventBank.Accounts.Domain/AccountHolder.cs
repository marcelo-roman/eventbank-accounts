namespace EventBank.Accounts.Domain;

public sealed record AccountHolder
{
    public const int MinLength = 3;
    public const int MaxLength = 100;

    private AccountHolder(string name) => Name = name;

    public string Name { get; }

    public static AccountHolder Create(string? name)
    {
        var trimmed = name?.Trim() ?? string.Empty;
        if (trimmed.Length is < MinLength or > MaxLength)
        {
            throw AccountErrors.InvalidHolder();
        }

        return new AccountHolder(trimmed);
    }

    internal static AccountHolder Restore(string name) => new(name);
}
