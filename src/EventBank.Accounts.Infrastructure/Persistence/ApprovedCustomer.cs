namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class ApprovedCustomer
{
    public Guid CustomerId { get; init; }

    public required string FullName { get; init; }
}
