using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain.Events;

public interface IAccountEvent : IDomainEvent
{
    Guid AccountId { get; }
}
