using AutoMapper;
using gRPCClaimsService.Protos;
using IRDA.DAL;

namespace IRDA.BLL;

public class MapperProfile:Profile
{
    private readonly string[] MonthName=["January","February","March","April","May","June","July","August","September","October","November","December"];
    public MapperProfile(){
        CreateMap<ClaimPaymentReportDTOgRPC,PaymentOfClaims>()
        .ForMember(pc=>pc.Payment,opt=>opt.MapFrom(cpr=>cpr.Amount))
        .ForMember(pc=>pc.Year,opt=>opt.MapFrom(cpr=>cpr.Year))
        .ForMember(pc=>pc.Month,opt=>opt.MapFrom(cpr=>MonthName[cpr.Month-1]));


        CreateMap<ClaimStatusReportDTOgRPC,PendingStatusReports>()
        .ForMember(pc=>pc.Count,opt=>opt.MapFrom(cpr=>cpr.Count))
        .ForMember(pc=>pc.Stage,opt=>opt.MapFrom(cpr=>cpr.Stage));
    }
}
