using System;
using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using gRPCClaimsService.Protos;
using System.Globalization;

public class Program
{
    #pragma warning disable CS0436 // Type conflicts with imported type
    public static void Main(string[] args)
    {

        DateTime date=DateTime.ParseExact("2024-05-22","yyyy-MM-dd",CultureInfo.InvariantCulture);

        // Create a channel and a client
        using var channel = GrpcChannel.ForAddress("http://localhost:5000");

        var client = new ClaimsService.ClaimsServiceClient(channel);

        // Make a request to the gRPC service
        // var response = client.AddNewClaim(new ClaimDetailRequestDTOgRPC
        // {
        //     PolicyNo="PR88624",
        //     EstimatedLoss=6000,
        //     DateOfAccident=Timestamp.FromDateTime(DateTime.SpecifyKind(date,DateTimeKind.Utc))
        // });
        // var response=client.GetClaimStatusReports(new GetClaimStatusReportsMonthAndYear{
        //     Month=0,
        //     Year=2024
        // });
        var response=client.GetClaimByClaimId(new GetClaimByIdString{
            ClaimId="CLMA332024"
        });
        Console.WriteLine(response.StatusCode);

        // Check if the Output field contains data
        if (response.Output != null)
        {
            // Attempt to unpack the Any type to SomeOtherMessage
            // if (response.Output.TryUnpack(out ErrorsListgRPC someOtherMessage))
            // {
            //     foreach(var error in someOtherMessage.Errors){
            //         Console.WriteLine(error.ErrorMessage+" "+error.Property);
            //     }
            // }
            // if(response.Output.TryUnpack(out ClaimStatusReportsListgRPC result)){
            //     foreach (var claimStatusReport in result.Reports){
            //         Console.WriteLine(claimStatusReport.Stage+" "+claimStatusReport.Count);
            //     }
            // }
            if(response.Output.TryUnpack(out ClaimDTOgRPC result)){
                Console.WriteLine(result.ClaimStatus);
            }
            else if(response.Output.TryUnpack(out StringValue res)){
                Console.WriteLine(res);
            }
            else
            {
                Console.WriteLine("Failed to unpack SomeOtherMessage.");
            }

            
        }
        else
        {
            Console.WriteLine("Output is null.");
        }


    }
}
