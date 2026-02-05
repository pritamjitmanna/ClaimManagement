using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using gRPCPoliciesService.Protos;
using gRPCSharedProtos.Protos;
using SharedModules;

namespace Insured.BLL;

/// <summary>
/// AutoMapper profile that defines mappings between the local DTOs and the gRPC DTOs used by the remote service.
/// - Provides mapping for ClaimDetailRequestDTO -> ClaimDetailRequestDTOgRPC (used when sending a new claim to gRPC).
/// - Provides mapping for PropertyValidationResponsegRPC -> PropertyValidationResponse (used when receiving validation errors).
/// </summary>
public class MapperProfile:Profile
{
    #pragma warning disable CS8629 // Nullable value type may be null.
    public MapperProfile(){
        // Map simple scalar properties directly (PolicyNo, EstimatedLoss).
        // The ForMember calls map specific destination members from source members.
        _ = CreateMap<ClaimDetailRequestDTO, ClaimDetailRequestDTOgRPC>()
        .ForMember(cdr => cdr.PolicyNo, opt => opt.MapFrom(cd => cd.PolicyNo))
        .ForMember(cdr => cdr.EstimatedLoss, opt => opt.MapFrom(cd => cd.EstimatedLoss))
        .ForMember(cdr => cdr.DateOfAccident, opt =>
        {
            // PreCondition ensures the mapping for DateOfAccident runs only when the source value is not null.
            // This prevents attempts to convert a null DateOnly? into a Timestamp.
            opt.PreCondition(cd => cd.DateOfAccident != null);

            // MapFrom converts the DateOnly (local C# type) into Google.Protobuf.WellKnownTypes.Timestamp.
            // DateOnly.ToDateTime(TimeOnly.MinValue) produces a DateTime representing midnight of that date.
            // DateTime.SpecifyKind(..., DateTimeKind.Utc) sets the kind to UTC so Timestamp.FromDateTime interprets it correctly.
            // Timestamp.FromDateTime produces the protobuf Timestamp required by the gRPC contract.
            opt.MapFrom(cd => Timestamp.FromDateTime(DateTime.SpecifyKind(((DateOnly)cd.DateOfAccident).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)));
        });

        _=CreateMap<PolicyEntryDTO, PolicyRequestDTOgRPC>()
        .ForMember(pedg=>pedg.InsuredFirstName, opt=>opt.MapFrom(ped=>ped.InsuredFirstName))
        .ForMember(pedg=>pedg.InsuredLastName, opt=>opt.MapFrom(ped=>ped.InsuredLastName))
        .ForMember(pedg=>pedg.DateOfInsurance, opt=>opt.MapFrom(ped=>Timestamp.FromDateTime(DateTime.SpecifyKind(ped.DateOfInsurance.ToDateTime(TimeOnly.MinValue),DateTimeKind.Utc))));

        // Map validation response messages received from gRPC into local SharedModules validation DTOs.
        _=CreateMap<PropertyValidationResponsegRPC,PropertyValidationResponse>()
        .ForMember(pvr=>pvr.Property, opt => opt.MapFrom(pvg=>pvg.Property))
        .ForMember(pvr=>pvr.ErrorMessage, opt => opt.MapFrom(pvg=>pvg.ErrorMessage));

        _=CreateMap<PolicyDTOgRPC,PolicyResponseDTO>()
        .ForMember(pdg=>pdg.PolicyNo, opt=>opt.MapFrom(pdg=>pdg.PolicyNo))
        .ForMember(pdg=>pdg.InsuredFirstName, opt=>opt.MapFrom(pdg=>pdg.InsuredFirstName))
        .ForMember(pdg=>pdg.InsuredLastName, opt=>opt.MapFrom(pdg=>pdg.InsuredLastName))
        .ForMember(cd=>cd.DateOfInsurance,opt=>{
            // PreCondition ensures DateOfAccident exists before attempting conversion (Timestamp -> DateOnly).
            opt.MapFrom(cg=>DateOnly.FromDateTime(cg.DateOfInsurance.ToDateTime()));})
        .ForMember(pdg=>pdg.VehicleNo, opt=>opt.MapFrom(pdg=>pdg.VehicleNo))
        .ForMember(pdg=>pdg.Status, opt=>opt.MapFrom(pdg=>pdg.Status));
    }
}
