using Microsoft.EntityFrameworkCore;

namespace IRDA.DAL;

public class IRDADBContext:DbContext
{
    public IRDADBContext(DbContextOptions<IRDADBContext> options):base(options){}

    public virtual DbSet<PendingStatusReports> StatusReports{ get; set; }

    public virtual DbSet<PaymentOfClaims> PaymentClaims{get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PendingStatusReports>().HasKey(p=>p.ReportId).HasName("Pk_PendingStatusReports");
        modelBuilder.Entity<PaymentOfClaims>().HasKey(p=>p.ReportId).HasName("Pk_PaymentOfClaims");
    }


}
