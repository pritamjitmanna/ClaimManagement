using System.Reflection;
using System.Security.Claims;
using Ocelot.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Ocelot.Middleware;
using System.Text.Json;
using Gateway.WebAPI.Notifications;
using System.Text.RegularExpressions;

namespace Gateway.WebAPI;

public static class OcelotAuthorize{
    private static readonly Dictionary<string, string> defaultClaims = GetClaimTypesConstantValues();

    private static string GetClaimTypeValue(string claim)
    {
        string claimType = claim;
        if (defaultClaims.TryGetValue(claimType, out string? claimName))
        {
            claimType = claimName;
        }
        return claimType.ToLower();
    }
    public static bool Authorize(HttpContext ctx)
    {
        DownstreamRoute route = (DownstreamRoute)ctx.Items["DownstreamRoute"];
        string key = route.AuthenticationOptions.AuthenticationProviderKey;
        Console.WriteLine("--------------");
        if (key == null || key == "") return true;
        if (route.RouteClaimsRequirement.Count == 0) return true;
        //flag for authorization
        bool auth = true;
        Claim[] claims = ctx.User.Claims.ToArray<Claim>();
        Dictionary<string, string> required = route.RouteClaimsRequirement;



        foreach (KeyValuePair<string, string> reqclaim in required)
        {

            string[] values=reqclaim.Value.Split(",").Select(inp=>inp.Trim()).ToArray();   //Gives the matches for the claims present in the `configuration.json`. Here it is only Role, it gives all the roles present.


            bool possible=false;

            foreach(var val in values){
                var vals=claims.Where(cl=>GetClaimTypeValue(cl.Type).Equals(reqclaim.Key,StringComparison.CurrentCultureIgnoreCase) && cl.Value==val).Select(cl=>cl.Value).ToList();
                // var v=claims.Where(cl=>GetClaimTypeValue(cl.Type).Equals(reqclaim.Key,StringComparison.CurrentCultureIgnoreCase)).ToList();
                // foreach(var q in v)Console.WriteLine(q);
                if(vals.Count>0){
                    possible=true;
                    break;
                }
            }    
            
            if(!possible){
                auth=false;
                break;
            }          
        }
        return auth;
    }

    private static Dictionary<string, string> GetClaimTypesConstantValues()
    {
        Type type = typeof(ClaimTypes);
        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        var values=fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToDictionary(fi => fi.GetValue(null)!.ToString()!, fi => fi.Name);
        
        return values;
    }
}

public class ProfileSetMiddleware
{   
    private readonly RequestDelegate _next;

    public ProfileSetMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,UserManager<AuthUser> _userManager)
    {
        try
        {
            DownstreamRoute route = (DownstreamRoute)context.Items["DownstreamRoute"];
            
            await _next(context);

            if(context.Request.Path.StartsWithSegments("/api/surveyors/addsurveyor", out var remainder))
            {
                // remainder e.g. "/johndoe"
                var username = remainder.Value.TrimStart('/');
                    if (context.Items.TryGetValue("DownstreamResponse", out var downstream))
                    {
                        var response = downstream as DownstreamResponse;

                        if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var user=await _userManager.FindByNameAsync(username);
                            if(user!=null)
                            {
                                if (!user.profileSet)
                                {
                                    user.profileSet=true;
                                    await _userManager.UpdateAsync(user);
                                }
                            }
                            else
                            {
                                await context.Response.WriteAsync("User not found");
                            }
                        }
                    }                        
                }
            
               
        }
        catch(Exception ex)
        {
            // Handle exception (logging, etc.)
            throw;
        }

        
    }
} 

public static class ProfileSetMiddlewareExtensions
{
    public static IApplicationBuilder UseProfileSetMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ProfileSetMiddleware>();
    }
}



public class NotificationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ChannelBackgroundService _channelBackgroundService;
    public NotificationMiddleware(RequestDelegate next, ChannelBackgroundService channelBackgroundService)
    {
        _next = next;
        _channelBackgroundService = channelBackgroundService;
    }

    public async Task InvokeAsync(HttpContext context,UserManager<AuthUser> _userManager)
    {
        try
        {
            DownstreamRoute route = (DownstreamRoute)context.Items["DownstreamRoute"];
            await _next(context);

            if(context.Items.TryGetValue("DownstreamResponse", out var downstream))
            {
                var response = downstream as DownstreamResponse;
                string receiverIdsValue = string.Empty;
                // Console.WriteLine(receiverIdsValue);
                DateTimeOffset timestamp = DateTimeOffset.UtcNow;
                if(response?.Headers!=null){
                    foreach(var header in response.Headers){
                        if(header.Key.Equals("Receiver-Id", StringComparison.OrdinalIgnoreCase)){
                            receiverIdsValue=header.Values.FirstOrDefault();
                        }
                        else if(header.Key.Equals("X-Timestamp", StringComparison.OrdinalIgnoreCase)){
                            if(DateTimeOffset.TryParse(header.Values.FirstOrDefault(), out var parsedTimestamp)){
                                timestamp=parsedTimestamp;
                            }
                        }
                    }
                }
                

                if (receiverIdsValue!=string.Empty &&response?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    using var json = JsonDocument.Parse(body);
                    List<string> receiverIdValues;
                    string pattern = @"\[(?<content>[^\]]+)\]";                    
                    MatchCollection matchesI = Regex.Matches(receiverIdsValue, pattern);
                    receiverIdValues = [.. matchesI.Cast<Match>().Select(m => m.Groups["content"].Value)];
                    List<string> messages=new List<string>();
                    
                    var messageElement = json.RootElement.GetProperty("message").GetString();
                    MatchCollection matches = Regex.Matches(messageElement, pattern);
                    messages = [.. matches.Cast<Match>().Select(m => m.Groups["content"].Value)];
                    for(int i=0;i<receiverIdValues.Count;i++)
                    {
                        string receiverIdValue = receiverIdValues[i];
                        if (receiverIdValue == "IC")
                        {                           
                            var users = await _userManager.GetUsersInRoleAsync("InsuranceCompany");
                            receiverIdValue = users.Select(u => u.Id).FirstOrDefault(); 
                        }
                        string message = messages[i];
                        // Console.WriteLine($"Receiver ID: {receiverIdValue}, Message: {message}");
                        bool notificationResult = await helperPushMessage(new NotificationModel
                        {
                            ToUserId = receiverIdValue,
                            Message = message,
                            Timestamp = timestamp,
                            IsRead = false
                        });
                        if (notificationResult)
                        {
                            Console.WriteLine("Notification queued successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to queue notification.");
                        }
                    }
                    // We can do one thing, we can send the message created from the microservice side only along with the response, the message will have a format like "Claim created successfully for user {userId}" and we can extract the userId from the message and send the notification to that user. This way we don't have to make any assumptions about the response structure. Also in that case, we don't need to write if else block as to for which endpoint the message should be sent. If the response body has the message field, we can just extract it.

                    //Also we can delete the message field while sending it from here to frontend as it is not required. Now, I have used JsonDocument which is immutable, thus I will change the logic later maybe.

                    // json.Remove()
                }
            }
            
        }
        catch(Exception ex)
        {
            // Handle exception (logging, etc.)
            Console.WriteLine($"Error in NotificationMiddleware: {ex.Message}");
            throw;
        }
    }

    private async Task<bool> helperPushMessage(NotificationModel notificationModel)
    {
        // Implementation for pushing notification message
        bool result= await _channelBackgroundService.QueueModelAsync(notificationModel);
        return result;
    }
}

public static class NotificationMiddlewareExtensions
{
    public static IApplicationBuilder UseNotificationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<NotificationMiddleware>();
    }
}

// Summary:
// This static helper implements a custom authorization check used by Ocelot's pipeline configuration.
// The Authorize(HttpContext) method inspects the configured DownstreamRoute.RouteClaimsRequirement
// and ensures the current HttpContext.User has matching claims (e.g., Role claims) as specified in the Ocelot configuration.
//
// Key behaviors:
// - If the route's AuthenticationProviderKey is null/empty, the route is considered public and the method returns true.
// - If RouteClaimsRequirement is empty, no claim-based restrictions are applied.
// - The method compares required claim types (keys in route.RouteClaimsRequirement) against the current user's claims.
//   Multiple allowed values for a claim in configuration.json are split by comma and treated as an OR list.
// - GetClaimTypeValue normalizes claim type names using a dictionary built from System.Security.Claims.ClaimTypes
//   so configuration can use friendly keys (e.g., "role") while still matching the actual claim type URIs.
//
// Explanation of helper functions:
// - GetClaimTypesConstantValues() uses reflection on System.Security.Claims.ClaimTypes to create a dictionary mapping
//   claim type URIs to their constant field names. This allows the code to look up friendly names for claim types.
//   It enumerates public static fields from ClaimTypes and builds a dictionary of their string values.
//
// - GetClaimTypeValue(string claim) looks up the friendly name for a claim type URI, then lowercases it for case-insensitive comparisons.
//   If no mapping is found, it returns the original claim string lowercased.
//
// - Authorize(HttpContext ctx):
//   - Extracts the DownstreamRoute from ctx.Items and reads AuthenticationOptions.AuthenticationProviderKey.
//   - If authentication provider key is empty the route is allowed (no auth required).



