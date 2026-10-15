using EventBank.BuildingBlocks.EventSourcing.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options)
{
    public DbSet<AccountSummary> AccountSummaries => Set<AccountSummary>();

    public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();

    public DbSet<ApprovedCustomer> ApprovedCustomers => Set<ApprovedCustomer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSubscriptionCheckpoints();
        modelBuilder.HasSequence<long>(EfAccountNumbers.Sequence).StartsAt(100001);

        modelBuilder.Entity<ApprovedCustomer>(customer =>
        {
            customer.ToTable("approved_customers");
            customer.HasKey(entry => entry.CustomerId);
            customer.Property(entry => entry.FullName).HasMaxLength(120);
        });

        modelBuilder.Entity<AccountSummary>(summary =>
        {
            summary.ToTable("account_summaries");
            summary.HasKey(entry => entry.Id);
            summary.HasIndex(entry => entry.OwnerId);
            summary.Property(entry => entry.Holder).HasMaxLength(120);
            summary.Property(entry => entry.Branch).HasMaxLength(4);
            summary.Property(entry => entry.Number).HasMaxLength(8);
            summary.Property(entry => entry.Currency).HasMaxLength(3);
            summary.Property(entry => entry.Status).HasMaxLength(20);
            summary.Property(entry => entry.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<AccountTransaction>(transaction =>
        {
            transaction.ToTable("account_transactions");
            transaction.HasKey(entry => entry.EventId);
            transaction.HasIndex(entry => new { entry.AccountId, entry.Version });
            transaction.Property(entry => entry.Kind).HasMaxLength(40);
            transaction.Property(entry => entry.Description).HasMaxLength(200);
            transaction.Property(entry => entry.Detail).HasMaxLength(100);
            transaction.Property(entry => entry.Amount).HasPrecision(18, 2);
            transaction.Property(entry => entry.BalanceAfter).HasPrecision(18, 2);
        });
    }
}
