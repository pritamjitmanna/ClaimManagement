﻿// <auto-generated />
using System;
using InsuranceCompany.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InsuranceCompany.DAL.Migrations
{
    [DbContext(typeof(InsuranceCompanyDBContext))]
    partial class InsuranceCompanyDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InsuranceCompany.DAL.ClaimDetail", b =>
                {
                    b.Property<string>("ClaimId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("AmtApprovedBySurveyor")
                        .HasColumnType("int");

                    b.Property<int>("ClaimStatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<DateOnly>("DateOfAccident")
                        .HasColumnType("date");

                    b.Property<int>("EstimatedLoss")
                        .HasColumnType("int");

                    b.Property<bool>("InsuranceCompanyApproval")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("PolicyNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("SurveyorFees")
                        .HasColumnType("int");

                    b.Property<int?>("SurveyorID")
                        .HasColumnType("int");

                    b.Property<int>("WithdrawClaim")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("ClaimId")
                        .HasName("Pk_ClaimDetail");

                    b.HasIndex("PolicyNo");

                    b.HasIndex("SurveyorID");

                    b.ToTable("ClaimDetails");
                });

            modelBuilder.Entity("InsuranceCompany.DAL.Fee", b =>
                {
                    b.Property<int>("FeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeeId"));

                    b.Property<int>("EstimateEndLimit")
                        .HasColumnType("int");

                    b.Property<int>("EstimateStartLimit")
                        .HasColumnType("int");

                    b.Property<int>("fees")
                        .HasColumnType("int");

                    b.HasKey("FeeId")
                        .HasName("Pk_Fee");

                    b.ToTable("Fees");

                    b.HasData(
                        new
                        {
                            FeeId = 1,
                            EstimateEndLimit = 10000,
                            EstimateStartLimit = 5000,
                            fees = 1000
                        },
                        new
                        {
                            FeeId = 2,
                            EstimateEndLimit = 20000,
                            EstimateStartLimit = 10000,
                            fees = 2000
                        },
                        new
                        {
                            FeeId = 3,
                            EstimateEndLimit = 70000,
                            EstimateStartLimit = 20000,
                            fees = 7000
                        });
                });

            modelBuilder.Entity("InsuranceCompany.DAL.Policy", b =>
                {
                    b.Property<string>("PolicyNo")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateOnly>("DateOfInsurance")
                        .HasColumnType("date");

                    b.Property<string>("EmailId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InsuredFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InsuredLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("status")
                        .HasColumnType("bit");

                    b.HasKey("PolicyNo")
                        .HasName("Pk_Policy");

                    b.ToTable("Policies");

                    b.HasData(
                        new
                        {
                            PolicyNo = "MA33724",
                            DateOfInsurance = new DateOnly(2024, 3, 14),
                            EmailId = "pm@cognizant.com",
                            InsuredFirstName = "Pritamjit",
                            InsuredLastName = "Manna",
                            VehicleNo = "HR26DK8337",
                            status = true
                        },
                        new
                        {
                            PolicyNo = "PR88624",
                            DateOfInsurance = new DateOnly(2024, 3, 13),
                            EmailId = "mp@cognizant.com",
                            InsuredFirstName = "Manna",
                            InsuredLastName = "Pritamjit",
                            VehicleNo = "WB31W6886",
                            status = true
                        },
                        new
                        {
                            PolicyNo = "SO18824",
                            DateOfInsurance = new DateOnly(2024, 3, 15),
                            EmailId = "ms@cognizant.com",
                            InsuredFirstName = "Souvik",
                            InsuredLastName = "Maity",
                            VehicleNo = "WB32U3188",
                            status = false
                        });
                });

            modelBuilder.Entity("InsuranceCompany.DAL.Surveyor", b =>
                {
                    b.Property<int>("SurveyorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SurveyorId"));

                    b.Property<int>("EstimateLimit")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TimesAllocated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("SurveyorId")
                        .HasName("Pk_Surveyor");

                    b.ToTable("Surveyors");

                    b.HasData(
                        new
                        {
                            SurveyorId = 1,
                            EstimateLimit = 6000,
                            FirstName = "R",
                            LastName = "K",
                            TimesAllocated = 0
                        },
                        new
                        {
                            SurveyorId = 2,
                            EstimateLimit = 15000,
                            FirstName = "K",
                            LastName = "R",
                            TimesAllocated = 0
                        },
                        new
                        {
                            SurveyorId = 3,
                            EstimateLimit = 50000,
                            FirstName = "P",
                            LastName = "M",
                            TimesAllocated = 0
                        },
                        new
                        {
                            SurveyorId = 4,
                            EstimateLimit = 15000,
                            FirstName = "S",
                            LastName = "M",
                            TimesAllocated = 0
                        });
                });

            modelBuilder.Entity("InsuranceCompany.DAL.ClaimDetail", b =>
                {
                    b.HasOne("InsuranceCompany.DAL.Policy", "Policy")
                        .WithMany("ClaimDetails")
                        .HasForeignKey("PolicyNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("Fk_Policy_ClaimDetail");

                    b.HasOne("InsuranceCompany.DAL.Surveyor", "Surveyor")
                        .WithMany("ClaimDetails")
                        .HasForeignKey("SurveyorID")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("Fk_Surveyor_ClaimDetail");

                    b.Navigation("Policy");

                    b.Navigation("Surveyor");
                });

            modelBuilder.Entity("InsuranceCompany.DAL.Policy", b =>
                {
                    b.Navigation("ClaimDetails");
                });

            modelBuilder.Entity("InsuranceCompany.DAL.Surveyor", b =>
                {
                    b.Navigation("ClaimDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
