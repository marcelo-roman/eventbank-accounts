using EventBank.Accounts.Application.Ports;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class EfAccountNumbers(AccountsDbContext dbContext) : IAccountNumbers
{
    public const string Sequence = "account_numbers";

    public async Task<long> Next(CancellationToken cancellationToken) =>
        await dbContext.Database
            .SqlQuery<long>($"SELECT nextval('account_numbers') AS \"Value\"")
            .SingleAsync(cancellationToken);
}
