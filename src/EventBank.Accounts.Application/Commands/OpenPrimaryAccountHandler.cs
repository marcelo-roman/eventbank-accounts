using EventBank.Accounts.Application.Ports;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;

namespace EventBank.Accounts.Application.Commands;

public sealed class OpenPrimaryAccountHandler(
    IEventSourcedRepository<BankAccount, AccountId> accounts,
    IApprovedCustomers approvedCustomers,
    IAccountNumbers accountNumbers) : ICommandHandler<OpenPrimaryAccount, CommandResult>
{
    private static readonly Currency PrimaryCurrency = Currency.From("BRL");

    public async Task<CommandResult> Handle(OpenPrimaryAccount command, CancellationToken cancellationToken)
    {
        await approvedCustomers.Register(command.CustomerId, command.FullName, cancellationToken);

        var owner = new OwnerId(command.CustomerId);
        var id = AccountIds.PrimaryFor(owner);
        if (await accounts.Find(id, cancellationToken) is { } existing)
        {
            return new CommandResult(existing.Id.Value, existing.Version);
        }

        var account = BankAccount.Open(
            id,
            owner,
            AccountHolder.Create(command.FullName),
            PrimaryCurrency,
            AccountNumber.FromSequence(await accountNumbers.Next(cancellationToken)));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
