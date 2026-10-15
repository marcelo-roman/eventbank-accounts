using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record CloseAccount(Guid AccountId) : ICommand<CommandResult>;
