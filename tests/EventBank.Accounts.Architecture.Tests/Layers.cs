using System.Reflection;

namespace EventBank.Accounts.Architecture.Tests;

internal static class Layers
{
    public const string Root = "EventBank.Accounts";

    public static readonly string[] OtherBoundedContexts = ["EventBank.Transfers", "EventBank.Customers", "EventBank.Cards", "EventBank.Notifications"];

    public static readonly Assembly Domain = typeof(Domain.BankAccount).Assembly;

    public static readonly Assembly Application = typeof(Application.AccountsApplication).Assembly;

    public static readonly Assembly Infrastructure = typeof(Infrastructure.AccountsInfrastructure).Assembly;

    public static readonly Assembly Api = typeof(Api.Endpoints.AccountEndpoints).Assembly;

    public static readonly Assembly[] All = [Domain, Application, Infrastructure, Api];
}
