using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using Reto.Tecnico.Yape.Models;

namespace Reto.Tecnico.Yape.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            entityType.SetTableName(tableName!.ToLowerInvariant());

            foreach (var property in entityType.GetProperties())
            {
                var propertyName = property.Name;
                property.SetColumnName(propertyName.ToLowerInvariant());
            }
        }

        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("transactions");
            e.Property(t => t.Status)
            .HasConversion<string>(); // Convert enum to string for storage
            e.Property(e => e.ExternalTransactionID)
            .HasConversion<string>();
            e.Property(e => e.TransactionID)
            .HasConversion<string>();
            e.Property(e => e.SourceAccountId)
            .HasConversion<string>();
            e.Property(e => e.TargetAccountId)
            .HasConversion<string>();
            e.Ignore(e => e.DailyTransfer);
        });
    }
}
