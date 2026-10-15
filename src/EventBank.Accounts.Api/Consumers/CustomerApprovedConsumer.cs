using EventBank.Accounts.Application.Commands;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Messaging;
using EventBank.Contracts;

namespace EventBank.Accounts.Api.Consumers;

public sealed class CustomerApprovedConsumer(ICommandHandler<OpenPrimaryAccount, CommandResult> handler)
    : IMessageHandler<CustomerApproved>
{
    public Task Handle(CustomerApproved message, CancellationToken cancellationToken) =>
        handler.Handle(new OpenPrimaryAccount(message.CustomerId, message.FullName), cancellationToken);
}
