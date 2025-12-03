using Microsoft.EntityFrameworkCore;

namespace IRDA.DAL;

/// <summary>
/// EF Core DbContext for IRDA persistence.
/// - Exposes DbSet for PendingStatusReports and PaymentOfClaims entities.
/// - Configures primary keys and constraint names in OnModelCreating.
/// </summary>
public class IRDADBContext:DbContext
{
    public IRDADBContext(DbContextOptions<IRDADBContext> options):base(options){}

    /// <summary>
    /// Represents PendingStatusReports table.
    /// </summary>
    public virtual DbSet<PendingStatusReports> StatusReports{ get; set; }

    /// <summary>
    /// Represents PaymentOfClaims table.
    /// </summary>
    public virtual DbSet<PaymentOfClaims> PaymentClaims{get; set; }

    /// <summary>
    /// Configures entity primary keys and names used in migrations to make schema clearer.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PendingStatusReports>().HasKey(p=>p.ReportId).HasName("Pk_PendingStatusReports");
        modelBuilder.Entity<PaymentOfClaims>().HasKey(p=>p.ReportId).HasName("Pk_PaymentOfClaims");
    }


}
