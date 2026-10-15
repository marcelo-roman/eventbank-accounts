namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class AccountSummary
{
    public Guid Id { get; init; }

    public required string Holder { get; init; }

    public required string Currency { get; init; }

    public decimal Balance { get; set; }

    public required string Status { get; set; }

    public long Version { get; set; }

    public DateTimeOffset OpenedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; set; }
}
