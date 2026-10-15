using EventBank.Accounts.Application.Commands;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Messaging;
using EventBank.Contracts;

namespace EventBank.Accounts.Api.Consumers;

public sealed class CreditAccountConsumer(ICommandHandler<CreditAccountForTransfer, CommandResult> handler)
    : IMessageHandler<CreditAccount>
{
    public Task Handle(CreditAccount message, CancellationToken cancellationToken) =>
        handler.Handle(
            new CreditAccountForTransfer(message.TransferId, message.AccountId, message.Amount, message.Currency),
            cancellationToken);
}
