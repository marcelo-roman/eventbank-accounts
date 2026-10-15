using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class OpenAccountHandler(IEventSourcedRepository<BankAccount, AccountId> accounts)
    : ICommandHandler<OpenAccount, CommandResult>
{
    public async Task<CommandResult> Handle(OpenAccount command, CancellationToken cancellationToken)
    {
        var account = BankAccount.Open(AccountId.New(), AccountHolder.Create(command.Holder), Currency.From(command.Currency));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
