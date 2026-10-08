using GSB.Test.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GSB.Test.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SanadCallback> SanadCallbacks { get; set; }

    public DbSet<UniqueIdentifierCallback> UniqueIdentifierCallbacks { get; set; }

    public DbSet<PoaEvaluationCallback> PoaEvaluationCallbacks { get; set; }

    public DbSet<MsbInvocationLog> MsbInvocationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var log = modelBuilder.Entity<MsbInvocationLog>();
        log.Property(x => x.Direction).HasMaxLength(16);
        log.Property(x => x.ServiceName).HasMaxLength(128);
        log.Property(x => x.HttpMethod).HasMaxLength(16);
        log.Property(x => x.OrganId).HasMaxLength(256);
        log.Property(x => x.OwTrakingCode).HasMaxLength(256);
        log.Property(x => x.MapConfirmationTrackingCode).HasMaxLength(256);
        log.Property(x => x.RequestId).HasMaxLength(256);
        log.Property(x => x.TraceIdentifier).HasMaxLength(256);
        log.HasIndex(x => x.CreatedAt);
        log.HasIndex(x => x.ServiceName);
        log.HasIndex(x => x.Direction);
        log.HasIndex(x => x.OrganId);
        log.HasIndex(x => x.OwTrakingCode);
        log.HasIndex(x => x.MapConfirmationTrackingCode);
        log.HasIndex(x => x.RequestId);
        log.HasIndex(x => x.TraceIdentifier);
        log.HasIndex(x => x.IsSuccess);
        log.HasIndex(x => new { x.ServiceName, x.CreatedAt });
        log.HasIndex(x => new { x.OrganId, x.CreatedAt });
    }
}
