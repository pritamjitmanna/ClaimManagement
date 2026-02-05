// Summary:
// AutoMapper profile for mapping between gRPC-generated DTOs (ClaimDetailRequestDTOgRPC, ClaimDTOgRPC, etc.)
// and internal DTOs used by the BLL. Highlights:
// - Conversion between google.protobuf.Timestamp and System.DateOnly/DateTime.
// - Mapping of validation/error DTOs to their gRPC equivalents.
// - Mapping claim report DTOs and policy DTOs for gRPC responses.

using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using gRPCPoliciesService.Protos;
using gRPCSharedProtos.Protos;
using InsuranceCompany.BLL;
using InsuranceCompany.BLL.RequestDTO;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany;

public class GRPCAutoMapperProfile:Profile
{
    public GRPCAutoMapperProfile(){

        CreateMap<ClaimDetailRequestDTOgRPC,ClaimDetailRequestDTO>()
        .ForMember(cd=>cd.DateOfAccident,opt=>{
            // PreCondition ensures DateOfAccident exists before attempting conversion (Timestamp -> DateOnly).
            opt.PreCondition(cg=>cg.DateOfAccident!=null);
            opt.MapFrom(cg=>DateOnly.FromDateTime(cg.DateOfAccident.ToDateTime()));
    }   )
        .ForMember(cd=>cd.EstimatedLoss,opt=>opt.MapFrom(cg=>cg.EstimatedLoss))
        .ForMember(cd=>cd.PolicyNo,opt=>opt.MapFrom(cg=>cg.PolicyNo));

        // Map validation error objects to gRPC error messages.
        CreateMap<PropertyValidationResponse,PropertyValidationResponsegRPC>()
        .ForMember(pg=>pg.Property,opt=>opt.MapFrom(pr=>pr.Property))
        .ForMember(pg=>pg.ErrorMessage,opt=>opt.MapFrom(pr=>pr.ErrorMessage));

        // Map internal ClaimListOpenDTO to ClaimDTOgRPC.
        // Note: DateOfAccident mapping uses Timestamp.FromDateTime and ensures UTC kind to avoid timezone issues.
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
        
        CreateMap<PolicyRequestDTOgRPC,PolicyEntryDTO>()
        .ForMember(prdg=>prdg.InsuredFirstName,opt=>opt.MapFrom(ped=>ped.InsuredFirstName))
        .ForMember(prdg=>prdg.InsuredLastName,opt=>opt.MapFrom(ped=>ped.InsuredLastName))
        .ForMember(prdg => prdg.DateOfInsurance, opt =>
        {
            // PreCondition ensures DateOfInsurance exists before conversion.
            opt.PreCondition(pedg => pedg.DateOfInsurance != null);
            opt.MapFrom(pedg => DateOnly.FromDateTime(pedg.DateOfInsurance.ToDateTime()));
        })
        .ForMember(prdg=>prdg.VehicleNo,opt=>opt.MapFrom(ped=>ped.VehicleNo));

        // Map Policy -> PolicyDTOgRPC; DateOfInsurance is converted to Timestamp with UTC kind.
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
