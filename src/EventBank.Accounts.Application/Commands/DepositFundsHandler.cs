using EventBank.Accounts.Application.Access;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class DepositFundsHandler(IEventSourcedRepository<BankAccount, AccountId> accounts, AccountAccess access)
    : ICommandHandler<DepositFunds, CommandResult>
{
    public async Task<CommandResult> Handle(DepositFunds command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        access.EnsureCanMove(account);
        account.Deposit(Money.Of(command.Amount, account.Currency));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
