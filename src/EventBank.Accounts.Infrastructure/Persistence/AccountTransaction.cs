namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class AccountTransaction
{
    public Guid EventId { get; init; }

    public Guid AccountId { get; init; }

    public long Version { get; init; }

    public required string Kind { get; init; }

    public decimal Amount { get; init; }

    public decimal BalanceAfter { get; init; }

    public Guid? TransferId { get; init; }

    public string? Description { get; init; }

    public string? Detail { get; init; }

    public DateTimeOffset OccurredAt { get; init; }
}
