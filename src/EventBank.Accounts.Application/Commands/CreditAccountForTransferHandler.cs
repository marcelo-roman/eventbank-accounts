using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class CreditAccountForTransferHandler(IEventSourcedRepository<BankAccount, AccountId> accounts)
    : ICommandHandler<CreditAccountForTransfer, CommandResult>
{
    public async Task<CommandResult> Handle(CreditAccountForTransfer command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        account.CreditForTransfer(new TransferId(command.TransferId), Money.Of(command.Amount, Currency.From(command.Currency)));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
