using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record OpenAccount(string? Currency) : ICommand<CommandResult>;
