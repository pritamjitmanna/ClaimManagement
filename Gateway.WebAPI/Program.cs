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

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AuthDBContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("AuthTesting")));

builder.Services.AddIdentity<AuthUser,IdentityRole>().AddEntityFrameworkStores<AuthDBContext>().AddDefaultTokenProviders();

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
        ValidIssuer=builder.Configuration["JWT:Issuer"],
        ValidAudience=builder.Configuration["JWT:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
    };
#pragma warning restore CS8604 // Possible null reference argument.
});

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
builder.Services.AddOcelot(new ConfigurationBuilder().AddJsonFile("configuration.json").Build());

var app = builder.Build();

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
await app.UseOcelot();

app.Run();
