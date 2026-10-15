using EventBank.Accounts.Application.Access;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class WithdrawFundsHandler(IEventSourcedRepository<BankAccount, AccountId> accounts, AccountAccess access)
    : ICommandHandler<WithdrawFunds, CommandResult>
{
    public async Task<CommandResult> Handle(WithdrawFunds command, CancellationToken cancellationToken)
    {
        var account = await accounts.Get(new AccountId(command.AccountId), cancellationToken);
        access.EnsureCanMove(account);
        account.Withdraw(Money.Of(command.Amount, account.Currency));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
