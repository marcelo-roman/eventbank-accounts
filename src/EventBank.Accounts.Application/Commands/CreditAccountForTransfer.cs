using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record CreditAccountForTransfer(Guid TransferId, Guid AccountId, decimal Amount, string Currency)
    : ICommand<CommandResult>;
