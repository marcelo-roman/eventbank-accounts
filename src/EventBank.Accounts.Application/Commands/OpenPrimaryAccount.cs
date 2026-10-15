using EventBank.BuildingBlocks.Application.Cqrs;

namespace EventBank.Accounts.Application.Commands;

public sealed record OpenPrimaryAccount(Guid CustomerId, string FullName) : ICommand<CommandResult>;
