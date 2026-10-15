using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class ChargeAccountForCardHandler(IEventSourcedRepository<BankAccount, AccountId> accounts)
    : ICommandHandler<ChargeAccountForCard, CommandResult>
{
    public async Task<CommandResult> Handle(ChargeAccountForCard command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        account.Charge(new ChargeId(command.ChargeId), Money.Of(command.Amount, Currency.From(command.Currency)), command.Description);
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
