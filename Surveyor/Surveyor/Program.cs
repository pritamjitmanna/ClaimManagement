#pragma warning disable CS8604 // Possible null reference argument.

using gRPCClaimsService.Protos;
using gRPCPoliciesService.Protos;
using Microsoft.EntityFrameworkCore;
using Surveyor.BLL;
using Surveyor.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SurveyorDBContext>(options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Surveyor"));
    });

builder.Services.AddGrpcClient<ClaimsService.ClaimsServiceClient>(options=>options.Address=new Uri(builder.Configuration.GetSection("URLs").GetSection("InsuranceCompanyURL").Value));
builder.Services.AddGrpcClient<PoliciesService.PoliciesServiceClient>(options=>options.Address=new Uri(builder.Configuration.GetSection("URLs").GetSection("InsuranceCompanyURL").Value));

builder.Services.AddScoped<ISurveyor,SurveyorRepository>();
builder.Services.AddScoped<ISurveyorService,SurveyorService>();


builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Program.cs bootstraps the Surveyor microservice.
// - Registers EF Core DbContext (SurveyorDBContext) using SQL Server connection string "Surveyor".
// - Registers a typed gRPC client for remote ClaimsService (base address from configuration).
// - Registers repository and service implementations with scoped lifetime (one per HTTP request).
// - Registers AutoMapper profiles, controllers, and Swagger/OpenAPI tooling.
// - Configures middleware pipeline (Swagger in Development, HTTPS redirection, Authorization, controller routing).
// Explanation of key calls:
// - AddDbContext<T>(options => options.UseSqlServer(...)) configures EF Core to use SQL Server provider.
// - AddGrpcClient<TClient>(...) registers typed gRPC client injected by DI.
// - AddScoped<TService, TImpl>() registers a scoped service that lives for the duration of the HTTP request.
// - AddAutoMapper(typeof(MapperProfile)) scans and registers mapping profiles for AutoMapper.
// - UseSwagger/UseSwaggerUI() and AddSwaggerGen() enable interactive API documentation during development.
