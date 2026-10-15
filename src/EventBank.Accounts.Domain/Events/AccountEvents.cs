namespace EventBank.Accounts.Domain.Events;

public sealed record AccountOpened(Guid AccountId, string Holder, string Currency) : IAccountEvent;

public sealed record FundsDeposited(Guid AccountId, decimal Amount) : IAccountEvent;

public sealed record FundsWithdrawn(Guid AccountId, decimal Amount) : IAccountEvent;

public sealed record TransferDebited(Guid AccountId, Guid TransferId, decimal Amount) : IAccountEvent;

public sealed record TransferDebitDeclined(Guid AccountId, Guid TransferId, decimal Amount, string Reason) : IAccountEvent;

public sealed record TransferCredited(Guid AccountId, Guid TransferId, decimal Amount) : IAccountEvent;

public sealed record TransferCreditDeclined(Guid AccountId, Guid TransferId, decimal Amount, string Reason) : IAccountEvent;

public sealed record TransferRefunded(Guid AccountId, Guid TransferId, decimal Amount) : IAccountEvent;

public sealed record AccountClosed(Guid AccountId) : IAccountEvent;
