using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record ChargeAccountForCard(Guid ChargeId, Guid AccountId, decimal Amount, string Currency, string Description)
    : ICommand<CommandResult>;
