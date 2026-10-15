using EventBank.BuildingBlocks.Application.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace EventBank.Accounts.Application;

public static class AccountsApplication
{
    public static IServiceCollection AddAccountsApplication(this IServiceCollection services) =>
        services.AddCqrsHandlers(typeof(AccountsApplication).Assembly);
}
