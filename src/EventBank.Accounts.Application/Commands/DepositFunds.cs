using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record DepositFunds(Guid AccountId, decimal Amount) : ICommand<CommandResult>;
