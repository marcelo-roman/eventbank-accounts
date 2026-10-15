using EventBank.Accounts.Application.Commands;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Messaging;
using EventBank.Contracts;

namespace EventBank.Accounts.Api.Consumers;

public sealed class DebitAccountConsumer(ICommandHandler<DebitAccountForTransfer, CommandResult> handler)
    : IMessageHandler<DebitAccount>
{
    public Task Handle(DebitAccount message, CancellationToken cancellationToken) =>
        handler.Handle(
            new DebitAccountForTransfer(message.TransferId, message.AccountId, message.Amount, message.Currency),
            cancellationToken);
}
