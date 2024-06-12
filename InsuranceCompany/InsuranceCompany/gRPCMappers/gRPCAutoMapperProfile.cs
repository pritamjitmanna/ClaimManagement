using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany;

public class GRPCAutoMapperProfile:Profile
{
    public GRPCAutoMapperProfile(){

        CreateMap<ClaimDetailRequestDTOgRPC,ClaimDetailRequestDTO>()
        .ForMember(cd=>cd.DateOfAccident,opt=>{
            opt.PreCondition(cg=>cg.DateOfAccident!=null);
            opt.MapFrom(cg=>DateOnly.FromDateTime(cg.DateOfAccident.ToDateTime()));
    }   )
        .ForMember(cd=>cd.EstimatedLoss,opt=>opt.MapFrom(cg=>cg.EstimatedLoss))
        .ForMember(cd=>cd.PolicyNo,opt=>opt.MapFrom(cg=>cg.PolicyNo));

        CreateMap<PropertyValidationResponse,PropertyValidationResponsegRPC>()
        .ForMember(pg=>pg.Property,opt=>opt.MapFrom(pr=>pr.Property))
        .ForMember(pg=>pg.ErrorMessage,opt=>opt.MapFrom(pr=>pr.ErrorMessage));

        CreateMap<ClaimListOpenDTO,ClaimDTOgRPC>()
        .ForMember(cd=>cd.ClaimId,opt=>opt.MapFrom(cld=>cld.ClaimId))
        .ForMember(cd=>cd.PolicyNo,opt=>opt.MapFrom(cld=>cld.PolicyNo))
        .ForMember(cd=>cd.DateOfAccident,opt=>opt.MapFrom(cg=>Timestamp.FromDateTime(DateTime.SpecifyKind(cg.DateOfAccident.ToDateTime(TimeOnly.MinValue),DateTimeKind.Utc))))
        .ForMember(cd=>cd.SurveyorID,opt=>opt.MapFrom(cld=>cld.SurveyorID))
        .ForMember(cd=>cd.AmtApprovedBySurveyor,opt=>opt.MapFrom(cld=>cld.AmtApprovedBySurveyor))
        .ForMember(cd=>cd.InsuranceCompanyApproval,opt=>opt.MapFrom(cld=>cld.InsuranceCompanyApproval))
        .ForMember(cd=>cd.WithdrawClaim,opt=>opt.MapFrom(cld=>cld.WithdrawClaim))
        .ForMember(cd=>cd.ClaimStatus,opt=>opt.MapFrom(cld=>cld.ClaimStatus))
        .ForMember(cd=>cd.SurveyorFees,opt=>opt.MapFrom(cld=>cld.SurveyorFees));


        CreateMap<ClaimStatusReportDTO,ClaimStatusReportDTOgRPC>()
        .ForMember(crg=>crg.Stage,opt=>opt.MapFrom(cr=>cr.Stage))
        .ForMember(crg=>crg.Count,opt=>opt.MapFrom(cr=>cr.Count));


        CreateMap<ClaimPaymentReportDTO,ClaimPaymentReportDTOgRPC>()
        .ForMember(crg=>crg.Month,opt=>opt.MapFrom(cr=>cr.Month))
        .ForMember(crg=>crg.Amount,opt=>opt.MapFrom(cr=>cr.Amount))
        .ForMember(crg=>crg.Year,opt=>opt.MapFrom(cr=>cr.Year));

        CreateMap<Policy, PolicyDTOgRPC>()
        .ForMember(pd=>pd.InsuredFirstName,opt=>opt.MapFrom(p=>p.InsuredFirstName))
        .ForMember(pd=>pd.InsuredLastName,opt=>opt.MapFrom(p=>p.InsuredLastName))
        .ForMember(cd=>cd.DateOfInsurance,opt=>opt.MapFrom(cg=>Timestamp.FromDateTime(DateTime.SpecifyKind(cg.DateOfInsurance.ToDateTime(TimeOnly.MinValue),DateTimeKind.Utc))))
        .ForMember(pd=>pd.Status,opt=>opt.MapFrom(p=>p.status))
        .ForMember(pd=>pd.PolicyNo,opt=>opt.MapFrom(p=>p.PolicyNo))
        .ForMember(pd=>pd.EmailId,opt=>opt.MapFrom(p=>p.EmailId))
        .ForMember(pd=>pd.VehicleNo,opt=>opt.MapFrom(p=>p.VehicleNo));
    }
}
