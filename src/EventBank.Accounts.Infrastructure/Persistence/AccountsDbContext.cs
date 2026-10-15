using EventBank.BuildingBlocks.EventSourcing.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace EventBank.Accounts.Infrastructure.Persistence;

public sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options)
{
    public DbSet<AccountSummary> AccountSummaries => Set<AccountSummary>();

    public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSubscriptionCheckpoints();

        modelBuilder.Entity<AccountSummary>(summary =>
        {
            summary.ToTable("account_summaries");
            summary.HasKey(entry => entry.Id);
            summary.Property(entry => entry.Holder).HasMaxLength(100);
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
            transaction.Property(entry => entry.Detail).HasMaxLength(100);
            transaction.Property(entry => entry.Amount).HasPrecision(18, 2);
            transaction.Property(entry => entry.BalanceAfter).HasPrecision(18, 2);
        });
    }
}
