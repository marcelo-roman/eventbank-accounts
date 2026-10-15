using EventBank.BuildingBlocks.Domain;

namespace EventBank.Accounts.Domain;

public static class AccountIds
{
    private static readonly Guid Namespace = Guid.Parse("6f0d7a52-3c1e-4b8a-9d2f-0c7e5b4a1d93");

    public static AccountId PrimaryFor(OwnerId owner) => new(DeterministicGuid.Create(Namespace, $"primary:{owner.Value}"));
}
