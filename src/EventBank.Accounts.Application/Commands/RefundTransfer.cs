using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record RefundTransfer(Guid TransferId, Guid AccountId) : ICommand<CommandResult>;
