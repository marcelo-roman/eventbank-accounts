namespace EventBank.Accounts.Api.Endpoints;

public sealed record OpenAccountRequest(string? Holder, string? Currency);

public sealed record AmountRequest(decimal Amount);
