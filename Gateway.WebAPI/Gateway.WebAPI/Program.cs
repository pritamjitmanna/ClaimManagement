using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Gateway.WebAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Ocelot.Authorization;
using Gateway.WebAPI.Notifications;
using SharedModules;
using DotNetEnv;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
var envName=builder.Environment.EnvironmentName;

Env.NoClobber().Load($"route.{envName}.env");

builder.Services.AddDbContext<AuthDBContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("Auth")),ServiceLifetime.Singleton);
builder.Services.AddSingleton<ChannelBackgroundService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ChannelBackgroundService>());

builder.Services.AddIdentity<AuthUser,IdentityRole>().AddEntityFrameworkStores<AuthDBContext>().AddDefaultTokenProviders();

var jwtDetails=builder.Configuration.GetSection("JWT").Get<JWT>();

builder.Services.AddAuthentication(options=>{
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>{
    options.SaveToken=true;
    options.RequireHttpsMetadata=false;
#pragma warning disable CS8604 // Possible null reference argument.
    options.TokenValidationParameters=new TokenValidationParameters{
        ValidateIssuer=true,
        ValidateAudience=true,
        ValidIssuer=jwtDetails?.Issuer,
        ValidAudience=jwtDetails?.Audience,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtDetails?.Secret)),
    };
#pragma warning restore CS8604 // Possible null reference argument.
});

// Source - https://stackoverflow.com/a
// Posted by Yong Shun, modified by community. See post 'Timeline' for change history
// Retrieved 2025-11-29, License - CC BY-SA 4.0


builder.Services.AddSwaggerGen();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

builder.Services.AddLogging(opt=>opt.AddConsole());
builder.Services.AddLogging(opt=>opt.AddDebug());

var routes=Path.Combine(builder.Environment.ContentRootPath,"Routes");
var processedRoutes=Path.Combine(builder.Environment.ContentRootPath,"processedRoutes");

if (!Directory.Exists(processedRoutes))
{
    Directory.CreateDirectory(processedRoutes);
}


//This loop replaces the {{}} values for port and host with the passed env values. Now when in development, the environment. getvariables takes from the env file, but when passed from docker-compose, it takes from their and skips the values from the env file here. The env file is loaded using DotNetEnv package. 
foreach(var file in Directory.GetFiles(routes,"ocelot.*.json"))
{
    var fileName=Path.GetFileName(file);

    var fileContent=File.ReadAllText(file);
    var pattern=@"{{ENV_\w+}}";
    Regex re=new Regex(pattern);

    var jsonContent=re.Replace(fileContent, match =>
    {
        var envVarName=match.Value.Substring(2,match.Value.Length-4);
        var EnvValue=Environment.GetEnvironmentVariable(envVarName);
        return EnvValue??match.Value;
    });

    var processedFile=Path.Combine(processedRoutes,fileName);
    File.WriteAllText(processedFile,jsonContent);
}




//The below does is to set the base path for ocelot in the given folder. It finds for files which starts with "ocelot.*.json" in the given folder and loads them. The order of loading is important as it will load the files in the order of their names. So if we have ocelot.json and ocelot.dev.json, then it will load ocelot.json first and then ocelot.dev.json. So if we have any duplicate routes in both files, then the routes in ocelot.dev.json will override the routes in ocelot.json. This is useful for having different routes for different environments.
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddOcelot(processedRoutes,builder.Environment);

// Console.WriteLine(builder.Environment.EnvironmentName);

builder.Services.AddOcelot();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        builder.WithOrigins("https://*devtunnels.ms").AllowAnyHeader().AllowAnyMethod();
    });
});

// builder.Services.AddSingleton<ChannelBackgroundService>();


var app = builder.Build();

app.UseCors("AllowAngularOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

// app.UseHttpsRedirection();

app.UseAuthentication();            //The order is important as it will first check the authentication and then the autorization. Else it will give error
app.UseAuthorization();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>{
    endpoints.MapControllerRoute(
        name: "default",
        pattern:"{controller=Home}/{action=Index}/{id?}");
});
#pragma warning restore ASP0014


var configuration = new OcelotPipelineConfiguration
{
    AuthorizationMiddleware = async (ctx, next) =>
    {
        if (OcelotAuthorize.Authorize(ctx))
        {
            await next.Invoke();

        }
        else {

            ctx.Items.SetError(new UnauthorizedError($"Fail to authorize"));
        }
        
    }
};



await app.UseOcelot(configuration);
app.UseProfileSetMiddleware();
app.UseNotificationMiddleware();

app.Run();

// Summary:
// This is the API Gateway bootstrap using ASP.NET Core and Ocelot.
// Responsibilities:
// - Configure EF Core for Identity (AuthDBContext) and register Identity stores (AddIdentity).
