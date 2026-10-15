using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class CloseAccountHandler(IEventSourcedRepository<BankAccount, AccountId> accounts)
    : ICommandHandler<CloseAccount, CommandResult>
{
    public async Task<CommandResult> Handle(CloseAccount command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        account.Close();
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
