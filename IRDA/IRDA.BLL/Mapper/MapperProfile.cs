using AutoMapper;
using gRPCClaimsService.Protos;
using IRDA.DAL;

namespace IRDA.BLL;

/// <summary>
/// AutoMapper profile for IRDA mappings between gRPC DTOs and local DAL entities.
/// - Maps ClaimPaymentReportDTOgRPC -> PaymentOfClaims (converts numeric month to month name).
/// - Maps ClaimStatusReportDTOgRPC -> PendingStatusReports.
/// </summary>
public class MapperProfile:Profile
{
    // MonthName array used to convert numeric month values (1-12) into readable month names.
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    public MapperProfile(){
        // Map ClaimPaymentReportDTOgRPC to PaymentOfClaims, copying amount/year and turning month index into a name string.
        CreateMap<ClaimPaymentReportDTOgRPC,PaymentOfClaims>()
        .ForMember(pc=>pc.Payment,opt=>opt.MapFrom(cpr=>cpr.Amount))
        .ForMember(pc=>pc.Year,opt=>opt.MapFrom(cpr=>cpr.Year))
        .ForMember(pc=>pc.Month,opt=>opt.MapFrom(cpr=>MonthName[cpr.Month-1]));


        // Map claim status gRPC DTO to local PendingStatusReports entity (Count and Stage fields).
        CreateMap<ClaimStatusReportDTOgRPC,PendingStatusReports>()
        .ForMember(pc=>pc.Count,opt=>opt.MapFrom(cpr=>cpr.Count))
        .ForMember(pc=>pc.Stage,opt=>opt.MapFrom(cpr=>cpr.Stage));
    }
}
