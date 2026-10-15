using EventBank.Accounts.Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class EfApprovedCustomers(AccountsDbContext dbContext) : IApprovedCustomers
{
    public Task<string?> FullNameOf(Guid customerId, CancellationToken cancellationToken) =>
        dbContext.ApprovedCustomers
            .Where(customer => customer.CustomerId == customerId)
            .Select(customer => customer.FullName)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task Register(Guid customerId, string fullName, CancellationToken cancellationToken)
    {
        if (await dbContext.ApprovedCustomers.AnyAsync(customer => customer.CustomerId == customerId, cancellationToken))
        {
            return;
        }

        dbContext.ApprovedCustomers.Add(new ApprovedCustomer { CustomerId = customerId, FullName = fullName });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
