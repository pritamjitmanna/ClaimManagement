using Microsoft.EntityFrameworkCore;

namespace Surveyor.DAL;

public class SurveyorDBContext:DbContext
{
    public SurveyorDBContext(DbContextOptions<SurveyorDBContext> options):base(options){}



    public DbSet<SurveyReport> Reports { get; set; }    

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<SurveyReport>().HasKey(e => e.ClaimId).HasName("Pk_SurveyReport");
    }
}
