using AutoMapper;
using gRPCClaimsService.Protos;
using SharedModules;
using Surveyor.DAL;

namespace Surveyor.BLL;

public class MapperProfile:Profile
{
    public MapperProfile(){
        CreateMap<SurveyReport,ReportDTO>()
        .ForMember(rd=>rd.ClaimId,opt=>opt.MapFrom(sr=>sr.ClaimId))
        .ForMember(rd=>rd.PolicyNo,opt=>opt.MapFrom(sr=>sr.PolicyNo))
        .ForMember(rd=>rd.LabourCharges,opt=>opt.MapFrom(sr=>sr.LabourCharges))
        .ForMember(rd=>rd.PartsCost,opt=>opt.MapFrom(sr=>sr.PartsCost))
        .ForMember(rd=>rd.PolicyClause,opt=>opt.MapFrom(sr=>sr.PolicyClause))
        .ForMember(rd=>rd.DepreciationCost,opt=>opt.MapFrom(sr=>sr.DepreciationCost))
        .ForMember(rd=>rd.TotalAmount,opt=>opt.MapFrom(sr=>sr.TotalAmount))
        .ForMember(rd=>rd.AccidentDetails,opt=>opt.MapFrom(sr=>sr.AccidentDetails));
        
        CreateMap<SurveyReportDTO,SurveyReport>()
        .ForMember(rd=>rd.ClaimId,opt=>opt.MapFrom(sr=>sr.ClaimId))
        .ForMember(rd=>rd.PolicyNo,opt=>opt.MapFrom(sr=>sr.PolicyNo))
        .ForMember(rd=>rd.LabourCharges,opt=>opt.MapFrom(sr=>sr.LabourCharges))
        .ForMember(rd=>rd.PartsCost,opt=>opt.MapFrom(sr=>sr.PartsCost))
        .ForMember(rd=>rd.DepreciationCost,opt=>opt.MapFrom(sr=>sr.DepreciationCost))
        .ForMember(rd=>rd.AccidentDetails,opt=>opt.MapFrom(sr=>sr.AccidentDetails));

        CreateMap<PropertyValidationResponsegRPC,PropertyValidationResponse>()
        .ForMember(pvr=>pvr.Property, opt => opt.MapFrom(pvg=>pvg.Property))
        .ForMember(pvr=>pvr.ErrorMessage, opt => opt.MapFrom(pvg=>pvg.ErrorMessage));
    }
}
