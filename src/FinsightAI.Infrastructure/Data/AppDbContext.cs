using FinsightAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinsightAI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<CryptoRate> CryptoRates => Set<CryptoRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Position>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.AssetType).IsRequired().HasMaxLength(50);
            e.Property(p => p.Amount).HasColumnType("decimal(18,8)");
            e.Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.InterestRate).HasColumnType("decimal(5,2)");
            e.HasOne(p => p.User).WithMany(u => u.Positions).HasForeignKey(p => p.UserId);
        });

        modelBuilder.Entity<ExchangeRate>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Type).IsRequired().HasMaxLength(20);
            e.Property(r => r.Buy).HasColumnType("decimal(18,2)");
            e.Property(r => r.Sell).HasColumnType("decimal(18,2)");
            e.HasIndex(r => new { r.Type, r.RecordedAt });
        });

        modelBuilder.Entity<CryptoRate>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Symbol).IsRequired().HasMaxLength(10);
            e.Property(r => r.PriceUsd).HasColumnType("decimal(18,2)");
            e.Property(r => r.PriceArs).HasColumnType("decimal(18,2)");
            e.Property(r => r.ChangePercent24h).HasColumnType("decimal(8,4)");
            e.HasIndex(r => new { r.Symbol, r.RecordedAt });
        });
    }
}
