using Microsoft.EntityFrameworkCore;
using SensorProcessor.Models;

namespace SensorProcessor.Data;

public class SensorDbContext : DbContext
{
    public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options) { }

    public DbSet<SensorData> SensorData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SensorId).IsRequired();
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.SensorId);
        });
    }
}