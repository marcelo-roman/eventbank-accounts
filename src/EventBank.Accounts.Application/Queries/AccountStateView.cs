using EventBank.Accounts.Domain;

namespace EventBank.Accounts.Application.Queries;

public sealed record AccountStateView(Guid Id, string Holder, string Currency, decimal Balance, string Status, long Version)
{
    public static AccountStateView From(BankAccount account) => new(
        account.Id.Value,
        account.Holder.Name,
        account.Currency.Code,
        account.Balance.Amount,
        account.Status.ToString(),
        account.Version);
}
