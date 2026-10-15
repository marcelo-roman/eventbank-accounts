namespace EventBank.Accounts.Application.Queries;

public sealed record AccountTransactionView(
    Guid EventId,
    long Version,
    string Kind,
    decimal Amount,
    decimal BalanceAfter,
    Guid? TransferId,
    string? Detail,
    DateTimeOffset OccurredAt);
