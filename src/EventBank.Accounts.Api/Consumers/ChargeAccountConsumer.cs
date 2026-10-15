using EventBank.Accounts.Application.Commands;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Messaging;
using EventBank.Contracts;

namespace EventBank.Accounts.Api.Consumers;

public sealed class ChargeAccountConsumer(ICommandHandler<ChargeAccountForCard, CommandResult> handler)
    : IMessageHandler<ChargeAccount>
{
    public Task Handle(ChargeAccount message, CancellationToken cancellationToken) =>
        handler.Handle(
            new ChargeAccountForCard(message.ChargeId, message.AccountId, message.Amount, message.Currency, message.Description),
            cancellationToken);
}
