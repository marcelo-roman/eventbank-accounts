namespace EventBank.Accounts.Application.Queries;

public sealed record AccountSummaryView(
    Guid Id,
    Guid OwnerId,
    string Holder,
    string Branch,
    string Number,
    string Currency,
    decimal Balance,
    string Status,
    long Version,
    DateTimeOffset OpenedAt,
    DateTimeOffset UpdatedAt);
