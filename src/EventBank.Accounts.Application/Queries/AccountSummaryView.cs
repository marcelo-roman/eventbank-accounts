namespace EventBank.Accounts.Application.Queries;

public sealed record AccountSummaryView(
    Guid Id,
    string Holder,
    string Currency,
    decimal Balance,
    string Status,
    long Version,
    DateTimeOffset OpenedAt,
    DateTimeOffset UpdatedAt);
