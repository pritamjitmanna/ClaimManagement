using Microsoft.EntityFrameworkCore;

namespace Surveyor.DAL;

/// <summary>
/// EF Core DbContext for Surveyor service persistence.
/// - Exposes DbSet<SurveyReport> Reports for survey records.
/// - Configures primary key and constraint name for SurveyReport entity in OnModelCreating.
/// </summary>
public class SurveyorDBContext:DbContext
{
    public SurveyorDBContext(DbContextOptions<SurveyorDBContext> options):base(options){}



    public DbSet<SurveyReport> Reports { get; set; }    

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<SurveyReport>().HasKey(e => e.ClaimId).HasName("Pk_SurveyReport");
    }
}
