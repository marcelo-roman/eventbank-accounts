using EventBank.Accounts.Api.Consumers;
using EventBank.Accounts.Api.Endpoints;
using EventBank.Accounts.Application;
using EventBank.Accounts.Infrastructure;
using EventBank.Accounts.Infrastructure.Persistence;
using EventBank.BuildingBlocks.Messaging;
using EventBank.BuildingBlocks.Web;
using EventBank.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services
    .AddAccountsApplication()
    .AddAccountsInfrastructure(builder.Configuration)
    .AddMessageHandler<DebitAccount, DebitAccountConsumer>()
    .AddMessageHandler<CreditAccount, CreditAccountConsumer>()
    .AddMessageHandler<RefundAccount, RefundAccountConsumer>();

var app = builder.Build();

app.UseServiceDefaults();
app.MapAccountEndpoints();
app.MapOperationsEndpoints<AccountsDbContext>();

await app.InitializeDatabase<AccountsDbContext>();
await app.RunAsync();
