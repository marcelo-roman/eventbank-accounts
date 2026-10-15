namespace EventBank.Accounts.Application.Ports;

public interface IAccountNumbers
{
    Task<long> Next(CancellationToken cancellationToken);
}
