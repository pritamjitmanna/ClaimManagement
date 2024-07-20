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

