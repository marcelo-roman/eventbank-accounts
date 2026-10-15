using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain;

public static class AccountErrors
{
    public static DomainException InvalidHolder() =>
        new("invalid_holder", $"Holder name must have between {AccountHolder.MinLength} and {AccountHolder.MaxLength} characters.");

    public static DomainException UnsupportedCurrency(string? code) =>
        new("unsupported_currency", $"Currency '{code}' is not supported. Use one of: {string.Join(", ", Currency.SupportedCodes)}.");

    public static DomainException InvalidAmount() =>
        new("invalid_amount", "Amount must be greater than zero and have at most two decimal places.");

    public static DomainException CurrencyMismatch(Currency expected, Currency actual) =>
        new("currency_mismatch", $"Account operates in {expected}, not {actual}.");

    public static DomainException AccountClosed() =>
        new("account_closed", "The account is closed.");

    public static DomainException InsufficientFunds() =>
        new("insufficient_funds", "The balance does not cover this amount.");

    public static DomainException BalanceNotZero() =>
        new("balance_not_zero", "Only accounts with a zero balance can be closed.");

    public static DomainException TransferNotDebited(TransferId transferId) =>
        new("transfer_not_debited", $"Transfer '{transferId.Value}' never debited this account.");
}
