namespace EventBank.Accounts.Api.Endpoints;

public sealed record OpenAccountRequest(string? Currency);

public sealed record AmountRequest(decimal Amount);
