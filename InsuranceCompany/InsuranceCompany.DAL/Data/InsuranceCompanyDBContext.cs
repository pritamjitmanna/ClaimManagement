using System.Globalization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SharedModules;

namespace InsuranceCompany.DAL;

public class InsuranceCompanyDBContext:DbContext
{

    //This step makes the class call any type of constructor which may be parameterized and non-parameterized. Part of DBContext dependency injection
    public InsuranceCompanyDBContext(DbContextOptions<InsuranceCompanyDBContext>options):base(options) {} 


    public virtual DbSet<Fee> Fees { get; set; }
    public virtual DbSet<Policy> Policies { get; set; } 
    public virtual DbSet<Surveyor> Surveyors { get; set; } 
    public virtual DbSet<ClaimDetail> ClaimDetails { get; set; } 




    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder.Entity<Policy>().HasKey(p => p.PolicyNo).HasName("Pk_Policy");
        modelBuilder.Entity<Policy>().HasMany(p => p.ClaimDetails).WithOne(p => p.Policy).IsRequired().HasForeignKey(c => c.PolicyNo).OnDelete(DeleteBehavior.Cascade).HasConstraintName("Fk_Policy_ClaimDetail");
        modelBuilder.Entity<Surveyor>().HasKey(s => s.SurveyorId).HasName("Pk_Surveyor");
        modelBuilder.Entity<Surveyor>().HasMany(s => s.ClaimDetails).WithOne(c => c.Surveyor).HasForeignKey(c => c.SurveyorID).OnDelete(DeleteBehavior.SetNull).HasConstraintName("Fk_Surveyor_ClaimDetail");
        modelBuilder.Entity<Surveyor>().Property(s => s.SurveyorId).ValueGeneratedOnAdd();
        modelBuilder.Entity<Surveyor>().Property(s => s.TimesAllocated).HasDefaultValue(0);
        modelBuilder.Entity<Fee>().HasKey(f => f.FeeId).HasName("Pk_Fee");
        modelBuilder.Entity<Fee>().Property(f => f.FeeId).ValueGeneratedOnAdd();
        modelBuilder.Entity<ClaimDetail>().HasKey(f => f.ClaimId).HasName("Pk_ClaimDetail");
        modelBuilder.Entity<ClaimDetail>().Property(c => c.WithdrawClaim).HasDefaultValue(WITHDRAWSTATUS.NOSTATUS);
        modelBuilder.Entity<ClaimDetail>().Property(c => c.InsuranceCompanyApproval).HasDefaultValue(false);
        modelBuilder.Entity<ClaimDetail>().Property(c => c.ClaimStatus).HasDefaultValue(ClaimStatus.Open);

        modelBuilder.Entity<Surveyor>().HasData([
            new Surveyor { SurveyorId = 1, FirstName = "R", LastName = "K", EstimateLimit = 6000 },
            new Surveyor { SurveyorId = 2, FirstName = "K", LastName = "R", EstimateLimit = 15000 },
            new Surveyor { SurveyorId = 3, FirstName = "P", LastName = "M", EstimateLimit = 50000 },
            new Surveyor { SurveyorId = 4, FirstName = "S", LastName = "M", EstimateLimit = 15000 }
        ]);

        modelBuilder.Entity<Policy>().HasData([
            new Policy { PolicyNo = "MA33724", InsuredFirstName = "Pritamjit", InsuredLastName = "Manna", DateOfInsurance = DateOnly.FromDateTime(DateTime.ParseExact("2024-03-14", "yyyy-MM-dd", CultureInfo.InvariantCulture)), EmailId = "pm@cognizant.com", VehicleNo = "HR26DK8337", status = true },
            new Policy { PolicyNo = "PR88624", InsuredFirstName = "Manna", InsuredLastName = "Pritamjit", DateOfInsurance = DateOnly.FromDateTime(DateTime.ParseExact("2024-03-13", "yyyy-MM-dd", CultureInfo.InvariantCulture)), EmailId = "mp@cognizant.com", VehicleNo = "WB31W6886", status = true },
            new Policy { PolicyNo = "SO18824", InsuredFirstName = "Souvik", InsuredLastName = "Maity", DateOfInsurance = DateOnly.FromDateTime(DateTime.ParseExact("2024-03-15", "yyyy-MM-dd", CultureInfo.InvariantCulture)), EmailId = "ms@cognizant.com", VehicleNo = "WB32U3188", status = false },
        ]);

        modelBuilder.Entity<Fee>().HasData([
            new Fee { FeeId = 1, EstimateStartLimit = 5000, EstimateEndLimit = 10000, fees = 1000 },
            new Fee { FeeId = 2, EstimateStartLimit = 10000, EstimateEndLimit = 20000, fees = 2000 },
            new Fee { FeeId = 3, EstimateStartLimit = 20000, EstimateEndLimit = 70000, fees = 7000 },
        ]);
    }
}
