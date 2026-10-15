using EventBank.Accounts.Application.Queries;
using EventBank.Accounts.Domain;
using EventBank.Accounts.Infrastructure.Integration;
using EventBank.Accounts.Infrastructure.Persistence;
using EventBank.Accounts.Infrastructure.Projections;
using EventBank.BuildingBlocks.EventSourcing;
using EventBank.BuildingBlocks.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBank.Accounts.Infrastructure;

public static class AccountsInfrastructure
{
    public static IServiceCollection AddAccountsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Accounts")
            ?? throw new InvalidOperationException("Connection string 'Accounts' is missing.");

        services.AddEventSourcing<AccountsDbContext>(connectionString, typeof(BankAccount).Assembly);
        services.AddAggregateRepository<BankAccount, AccountId>();
        services.AddProjection<AccountsDbContext, AccountReadModelProjection>();
        services.AddProjectionSubscription<AccountsDbContext>();
        services.AddIntegrationEventPublishing<AccountsDbContext, AccountsIntegrationEventMapper>();
        services.AddScoped<IAccountReadStore, EfAccountReadStore>();
        services.AddRabbitMq(configuration);

        return services;
    }
}
