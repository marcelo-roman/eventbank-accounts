using EventBank.Accounts.Application.Access;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class CloseAccountHandler(IEventSourcedRepository<BankAccount, AccountId> accounts, AccountAccess access)
    : ICommandHandler<CloseAccount, CommandResult>
{
    public async Task<CommandResult> Handle(CloseAccount command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        access.EnsureCanMove(account);
        account.Close();
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
