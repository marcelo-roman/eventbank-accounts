using EventBank.Accounts.Application.Ports;
using EventBank.Accounts.Domain;
using EventBank.BuildingBlocks.Application.Cqrs;
using EventBank.BuildingBlocks.Application.Persistence;
using EventBank.BuildingBlocks.Application.Security;

namespace EventBank.Accounts.Application.Commands;

public sealed class OpenAccountHandler(
    IEventSourcedRepository<BankAccount, AccountId> accounts,
    IApprovedCustomers approvedCustomers,
    IAccountNumbers accountNumbers,
    ICurrentUser currentUser) : ICommandHandler<OpenAccount, CommandResult>
{
    public async Task<CommandResult> Handle(OpenAccount command, CancellationToken cancellationToken)
    {
        var fullName = await approvedCustomers.FullNameOf(currentUser.Id, cancellationToken)
            ?? throw new ForbiddenException("Accounts are opened only for approved customers.");

        var account = BankAccount.Open(
            AccountId.New(),
            new OwnerId(currentUser.Id),
            AccountHolder.Create(fullName),
            Currency.From(command.Currency),
            AccountNumber.FromSequence(await accountNumbers.Next(cancellationToken)));
        await accounts.Save(account, cancellationToken);
        return new CommandResult(account.Id.Value, account.Version);
    }
}
