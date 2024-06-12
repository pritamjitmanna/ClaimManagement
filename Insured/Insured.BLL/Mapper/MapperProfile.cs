using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using SharedModules;

namespace Insured.BLL;

public class MapperProfile:Profile
{
    #pragma warning disable CS8629 // Nullable value type may be null.
    public MapperProfile(){
        _ = CreateMap<ClaimDetailRequestDTO, ClaimDetailRequestDTOgRPC>()
        .ForMember(cdr => cdr.PolicyNo, opt => opt.MapFrom(cd => cd.PolicyNo))
        .ForMember(cdr => cdr.EstimatedLoss, opt => opt.MapFrom(cd => cd.EstimatedLoss))
        .ForMember(cdr => cdr.DateOfAccident, opt =>
        {
            opt.PreCondition(cd => cd.DateOfAccident != null);

            opt.MapFrom(cd => Timestamp.FromDateTime(DateTime.SpecifyKind(((DateOnly)cd.DateOfAccident).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)));
        });

        CreateMap<PropertyValidationResponsegRPC,PropertyValidationResponse>()
        .ForMember(pvr=>pvr.Property, opt => opt.MapFrom(pvg=>pvg.Property))
        .ForMember(pvr=>pvr.ErrorMessage, opt => opt.MapFrom(pvg=>pvg.ErrorMessage));
    }
}
