using EventBank.Accounts.Application.Commands;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Messaging;
using EventBank.Contracts;

namespace EventBank.Accounts.Api.Consumers;

public sealed class RefundAccountConsumer(ICommandHandler<RefundTransfer, CommandResult> handler)
    : IMessageHandler<RefundAccount>
{
    public Task Handle(RefundAccount message, CancellationToken cancellationToken) =>
        handler.Handle(new RefundTransfer(message.TransferId, message.AccountId), cancellationToken);
}
