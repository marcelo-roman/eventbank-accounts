using EventBank.Accounts.Application.Commands;
using EventBank.Accounts.Application.Queries;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;
using EventBank.BuildingBlocks.Web;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EventBank.Accounts.Api.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var accounts = endpoints.MapGroup("/accounts").WithTags("Accounts");

        accounts.MapPost("/", Open);
        accounts.MapPost("/{accountId:guid}/deposits", Deposit);
        accounts.MapPost("/{accountId:guid}/withdrawals", Withdraw);
        accounts.MapPost("/{accountId:guid}/close", Close);

        accounts.MapGet("/", List);
        accounts.MapGet("/{accountId:guid}", Get);
        accounts.MapGet("/{accountId:guid}/transactions", Transactions);
        accounts.MapGet("/{accountId:guid}/events", Events);
        accounts.MapGet("/{accountId:guid}/state", StateAt);

        return endpoints;
    }

    private static async Task<Created<CommandResult>> Open(
        OpenAccountRequest request,
        ICommandHandler<OpenAccount, CommandResult> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new OpenAccount(request.Holder, request.Currency), cancellationToken);
        return TypedResults.Created($"/accounts/{result.Id}", result);
    }

    private static async Task<Ok<CommandResult>> Deposit(
        Guid accountId,
        AmountRequest request,
        ICommandHandler<DepositFunds, CommandResult> handler,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await handler.Handle(new DepositFunds(accountId, request.Amount), cancellationToken));

    private static async Task<Ok<CommandResult>> Withdraw(
        Guid accountId,
        AmountRequest request,
        ICommandHandler<WithdrawFunds, CommandResult> handler,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await handler.Handle(new WithdrawFunds(accountId, request.Amount), cancellationToken));

    private static async Task<Ok<CommandResult>> Close(
        Guid accountId,
        ICommandHandler<CloseAccount, CommandResult> handler,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await handler.Handle(new CloseAccount(accountId), cancellationToken));

    private static async Task<Ok<IReadOnlyList<AccountSummaryView>>> List(
        IQueryHandler<ListAccounts, IReadOnlyList<AccountSummaryView>> handler,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await handler.Handle(new ListAccounts(), cancellationToken));

    private static async Task<Results<Ok<AccountSummaryView>, NotFound>> Get(
        Guid accountId,
        IQueryHandler<GetAccount, AccountSummaryView?> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new GetAccount(accountId), cancellationToken) is { } account
            ? TypedResults.Ok(account)
            : TypedResults.NotFound();

    private static async Task<Ok<IReadOnlyList<AccountTransactionView>>> Transactions(
        Guid accountId,
        IQueryHandler<ListAccountTransactions, IReadOnlyList<AccountTransactionView>> handler,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await handler.Handle(new ListAccountTransactions(accountId), cancellationToken));

    private static async Task<Ok<IReadOnlyList<StoredEventResponse>>> Events(
        Guid accountId,
        IQueryHandler<GetAccountEvents, IReadOnlyList<RecordedEvent>> handler,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(StoredEventResponse.From(await handler.Handle(new GetAccountEvents(accountId), cancellationToken)));

    private static async Task<Results<Ok<AccountStateView>, NotFound>> StateAt(
        Guid accountId,
        long version,
        IQueryHandler<GetAccountStateAt, AccountStateView?> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new GetAccountStateAt(accountId, version), cancellationToken) is { } state
            ? TypedResults.Ok(state)
            : TypedResults.NotFound();
}
