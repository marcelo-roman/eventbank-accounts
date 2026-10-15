using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain;

public readonly record struct AccountId(Guid Value) : IEntityId
{
    public static AccountId New() => new(Guid.CreateVersion7());
}
