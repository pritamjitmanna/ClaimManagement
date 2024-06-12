using AutoMapper;
using InsuranceCompany.DAL;
using SharedModules;

namespace InsuranceCompany.BLL;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ClaimDetail, ClaimListOpenDTO>()
            .ForMember(cd => cd.ClaimId, opt => opt.MapFrom(cld => cld.ClaimId))
            .ForMember(cd => cd.PolicyNo, opt => opt.MapFrom(cld => cld.PolicyNo))
            .ForMember(cd => cd.EstimatedLoss, opt => opt.MapFrom(cld => cld.EstimatedLoss))
            .ForMember(cd => cd.DateOfAccident, opt => opt.MapFrom(cld => cld.DateOfAccident))
            .ForMember(cd => cd.SurveyorID, opt => opt.MapFrom(cld => cld.SurveyorID))
            .ForMember(cd => cd.AmtApprovedBySurveyor, opt => opt.MapFrom(cld => cld.AmtApprovedBySurveyor))
            .ForMember(cd => cd.WithdrawClaim, opt => opt.MapFrom(cld => cld.WithdrawClaim))
            .ForMember(cd=>cd.SurveyorFees,opt=>opt.MapFrom(cld=>cld.SurveyorFees))
            .ForMember(cd=>cd.ClaimStatus,opt=>opt.MapFrom(cld=>cld.ClaimStatus));

        CreateMap<ClaimDetailRequestDTO, ClaimDetail>()
            .ForMember(cd => cd.PolicyNo, opt => opt.MapFrom(cld => cld.PolicyNo))
            .ForMember(cd => cd.EstimatedLoss, opt => opt.MapFrom(cld => cld.EstimatedLoss))
            .ForMember(cd => cd.DateOfAccident, opt => opt.MapFrom(cld => cld.DateOfAccident));

        CreateMap<Fee, FeeDTO>()
            .ForMember(fd => fd.EstimateStartLimit, opt => opt.MapFrom(f => f.EstimateStartLimit))
            .ForMember(fd => fd.EstimateEndLimit, opt => opt.MapFrom(f => f.EstimateEndLimit))
            .ForMember(fd => fd.fees, opt => opt.MapFrom(f => f.fees));
        
        CreateMap<Surveyor, SurveyorDTO>()
            .ForMember(fd => fd.SurveyorId, opt => opt.MapFrom(f => f.SurveyorId))
            .ForMember(fd => fd.FirstName, opt => opt.MapFrom(f => f.FirstName))
            .ForMember(fd => fd.LastName, opt => opt.MapFrom(f => f.LastName))
            .ForMember(fd => fd.TimesAllocated, opt => opt.MapFrom(f => f.TimesAllocated))
            .ForMember(fd => fd.EstimateLimit, opt => opt.MapFrom(f => f.EstimateLimit));

        
    }
}
