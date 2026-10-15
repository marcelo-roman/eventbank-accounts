using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class RefundTransferHandler(IEventSourcedRepository<BankAccount, AccountId> accounts)
    : ICommandHandler<RefundTransfer, CommandResult>
{
    public async Task<CommandResult> Handle(RefundTransfer command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        account.RefundTransfer(new TransferId(command.TransferId));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
