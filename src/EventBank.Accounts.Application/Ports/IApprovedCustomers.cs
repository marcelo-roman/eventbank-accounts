namespace EventBank.Accounts.Application.Ports;

public interface IApprovedCustomers
{
    Task<string?> FullNameOf(Guid customerId, CancellationToken cancellationToken);

    Task Register(Guid customerId, string fullName, CancellationToken cancellationToken);
}
