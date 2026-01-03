using System.Reflection;
using System.Security.Claims;
using Ocelot.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Ocelot.Middleware;
using System.Text.Json;

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
        if (key == null || key == "") return true;
        if (route.RouteClaimsRequirement.Count == 0) return true;
        //flag for authorization
        bool auth = true;
        Claim[] claims = ctx.User.Claims.ToArray<Claim>();
        Dictionary<string, string> required = route.RouteClaimsRequirement;



        foreach (KeyValuePair<string, string> reqclaim in required)
        {

            string[] values=reqclaim.Value.Split(",").Select(inp=>inp.Trim()).ToArray();   //Gives the matches for the claims present in the configuration.json. Here it is only Role, it gives all the roles present.


            bool possible=false;

            foreach(var val in values){
                var vals=claims.Where(cl=>GetClaimTypeValue(cl.Type).Equals(reqclaim.Key,StringComparison.CurrentCultureIgnoreCase) && cl.Value==val).Select(cl=>cl.Value).ToList();
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
        Console.WriteLine(values);
        
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
                            var body = await response.Content.ReadAsStringAsync();
                            var user=await _userManager.FindByNameAsync(username);
                            using var json = JsonDocument.Parse(body);
                            var profileId = json.RootElement.GetProperty("output").GetInt32();
                            if(user!=null)
                            {
                                if (!user.profileSet)
                                {
                                    user.profileSet=true;
                                    user.profileId=profileId;
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



