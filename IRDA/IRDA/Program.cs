// Top-level file that bootstraps the ASP.NET Core application.
// Summary:
// - Builds the WebApplication (reads configuration, environment, etc.).
// - Registers services used by IRDA: DbContext, gRPC client, BLL services and repositories, AutoMapper, controllers and JSON options.
// - Configures middleware pipeline: Swagger in Development, HTTPS redirection, Authorization and controller routing.
//
// Explanation of key registrations & options:
// - AddDbContext<IRDADBContext>(options => options.UseSqlServer(...)):
//   Registers EF Core DbContext with SQL Server provider using connection string named "IRDA" from configuration.
// - AddGrpcClient<ClaimsService.ClaimsServiceClient>(...):
//   Registers a typed gRPC client that will be resolved via DI. The Address sets the base URI used to call the remote Claims gRPC service.
// - AddScoped<...>(): registers services/repositories with scoped lifetime (one instance per HTTP request).
// - AddAutoMapper(typeof(MapperProfile)): scans and registers AutoMapper profiles (mapping configurations).
// - AddControllers().AddJsonOptions(...): configures JSON serialization options:
//     - JsonStringEnumConverter: serialize enums as strings.
//     - DefaultIgnoreCondition = WhenWritingNull: omit nulls from output JSON.
//     - ReferenceHandler.IgnoreCycles: prevents self-referencing loop serialization errors.
// - AddEndpointsApiExplorer() and AddSwaggerGen(): enable OpenAPI metadata and Swagger UI.
// - Middleware:
//     - UseSwagger/UseSwaggerUI(): exposes API docs in Development environment.
//     - UseHttpsRedirection(): forces HTTPS.
//     - UseAuthorization(): ensures authorization middleware runs (attributes control access).
//     - MapControllers(): maps attribute-routed controllers into the endpoint pipeline.

#pragma warning disable CS8604 // Possible null reference argument.

using gRPCClaimsService.Protos;
using IRDA.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IRDA.BLL;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<IRDADBContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("IRDA")));

builder.Services.AddGrpcClient<ClaimsService.ClaimsServiceClient>(options=>options.Address=new Uri(builder.Configuration.GetSection("URLs").GetSection("InsuranceCompanyURL").Value));

builder.Services.AddScoped<IPaymentOfClaimsService,PaymentOfClaimsService>();
builder.Services.AddScoped<IPaymentOfClaims,PaymentOfClaimsRepository>();
builder.Services.AddScoped<IPendingStatusReportsService,PendingStatusReportsService>();
builder.Services.AddScoped<IPendingStatusReports,PendingStatusReportsRepository>();


builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperProfile>());



builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});


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
