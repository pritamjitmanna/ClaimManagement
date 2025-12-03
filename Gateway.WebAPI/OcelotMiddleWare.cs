using System.Reflection;
using System.Security.Claims;
using Ocelot.Configuration;

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

