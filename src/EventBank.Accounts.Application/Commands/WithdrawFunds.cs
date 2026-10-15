using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record WithdrawFunds(Guid AccountId, decimal Amount) : ICommand<CommandResult>;
