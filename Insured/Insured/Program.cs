#pragma warning disable CS8604 // Possible null reference argument.
using System.Text;
using gRPCClaimsService.Protos;
using gRPCPoliciesService.Protos;
using Insured.BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Summary: Program.cs bootstraps the ASP.NET Core app.
// - Builds the WebApplication using configuration and command-line args.
// - Registers services used by the Insured app: a gRPC client for the InsuranceCompany service,
//   the Insured business service, AutoMapper, controllers and Swagger for API documentation.
// - Configures middleware pipeline: Swagger (in Development), HTTPS redirection, Authorization and routing.
//
// Explanation of key methods used:
// - builder.Services.AddGrpcClient<TClient>(options => options.Address = new Uri(...)):
//   Registers a typed gRPC client that will be resolved via DI. The client will use the provided base address
//   when calling the remote gRPC service.
// - builder.Services.AddScoped<TService, TImpl>(): registers a service with scoped lifetime (one instance per HTTP request).
// - builder.Services.AddAutoMapper(typeof(MapperProfile)): registers AutoMapper and scans the provided profile type for mappings.
// - builder.Services.AddControllers(): registers MVC controllers for API endpoints.
// - builder.Services.AddEndpointsApiExplorer() and AddSwaggerGen(): enable OpenAPI generation and Swagger UI for API docs.
// - app.UseSwagger() / app.UseSwaggerUI(): enable interactive API documentation in Development environment.
// - app.UseHttpsRedirection(): redirects HTTP requests to HTTPS.
// - app.UseAuthorization(): enables Authorization middleware (policies would be applied by attributes).
// - app.MapControllers(): maps attribute-routed controllers to endpoints.
//
// Note: No code logic is changed; comments above explain the intent of each registration and middleware.

builder.Services.AddGrpcClient<ClaimsService.ClaimsServiceClient>(options=>options.Address=new Uri(builder.Configuration.GetSection("URLs").GetSection("InsuranceCompanyURL").Value));
builder.Services.AddGrpcClient<PoliciesService.PoliciesServiceClient>(options=>options.Address=new Uri(builder.Configuration.GetSection("URLs").GetSection("InsuranceCompanyURL").Value));

builder.Services.AddScoped<IInsuredService,InsuredService>();


builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
