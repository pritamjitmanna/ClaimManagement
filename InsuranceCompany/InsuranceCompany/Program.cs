//dotnet run --project InsuranceCompany 



// Summary:
// Application entry (Minimal API style) that configures services and middleware.
// - Registers the EF Core DbContext with SQL Server provider.
// - Configures application services and repository implementations via DI (AddScoped).
// - Registers AutoMapper profiles used by BLL and gRPC mapping.
// - Enables gRPC and MVC controllers with JSON options (enum as string, ignore nulls, reference loop handling).
// - Adds a CORS policy for the Angular frontend, Swagger for API exploration, and maps gRPC & HTTP endpoints.
// Notes on key functions:
// - AddDbContext<T>(options => options.UseSqlServer(...)): registers DbContext with a SQL Server provider and connection string.
// - AddScoped<TService, TImpl>(): registers services with scoped lifetime (one per request/connection scope).
// - AddAutoMapper(typeof(ProfileType)): auto-discovers and registers AutoMapper mapping profiles.
// - AddGrpc(): registers gRPC services so they can be mapped with MapGrpcService<T>().
// - AddJsonOptions(...): configures System.Text.Json serializer behaviour for controllers.
// - AddCors/AddPolicy: configures and names a CORS policy; UseCors applies it at runtime.
// - UseSwagger/UseSwaggerUI: enables interactive OpenAPI UI in development.
// - MapGrpcService<T> and MapControllers: map gRPC and MVC endpoints into the request pipeline.

using System.Text.Json.Serialization;
using InsuranceCompany;
using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register the EF Core DbContext using SQL Server; the connection string is read from configuration.
// UseSqlServer configures the database provider and its options (connection string, command timeout, etc.).
builder.Services.AddDbContext<InsuranceCompanyDBContext>(options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("InsuranceCompany"));
});

// Configuring the Dependencies in the application
// AddScoped: creates one instance per HTTP request / gRPC call scope for these services and repositories.
builder.Services.AddScoped<IClaimDetailService, ClaimDetailService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<ISurveyorService, SurveyorService>();
builder.Services.AddScoped<IClaimDetail, ClaimDetailRepository>();
builder.Services.AddScoped<IFee, FeeRepository>();
builder.Services.AddScoped<IPolicy, PolicyRepository>();
builder.Services.AddScoped<ISurveyor, SurveyorRepository>();
builder.Services.AddScoped<ISharedLogic,SharedLogic>();

// AutoMapper registration:
// Register mapping profiles so IMapper can be injected into services and controllers.
// AddAutoMapper scans the provided types' assemblies for Profile implementations.
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddAutoMapper(typeof(GRPCAutoMapperProfile));

// Register gRPC server support so gRPC services can be mapped later.
builder.Services.AddGrpc();

// Register MVC controllers and configure JSON serialization behavior.
// - JsonStringEnumConverter: serializes enums as strings instead of numeric values.
// - DefaultIgnoreCondition = WhenWritingNull: omits null properties from JSON.
// - ReferenceHandler.IgnoreCycles: prevents serializer from throwing on circular references (may produce truncated graphs).
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

// Configure CORS policy used by the frontend (example: Angular at localhost:4200).
// WithOrigins restricts origins; AllowAnyHeader/AllowAnyMethod permit any header/method from that origin.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});
// Swagger/OpenAPI generation for HTTP controllers (development-time).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map gRPC service implementation to handle gRPC calls.
// MapGrpcService<T>() wires up the gRPC service type into the Kestrel endpoints.
app.MapGrpcService<ClaimsServices>();
// Note: browser requests to gRPC endpoints require a gRPC-web client or a non-browser gRPC client.

// Apply the configured CORS policy to pipeline so HTTP endpoints accept requests from configured origins.
app.UseCors("AllowAngularOrigins");

// Configure the HTTP request pipeline for development vs production.
// In development, enable Swagger UI for interactive API exploration.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enforce HTTPS redirection for HTTP requests.
app.UseHttpsRedirection();

// Authorization middleware (policy-based or JWT/etc. can be plugged in).
app.UseAuthorization();

// Map attribute-routed controllers to endpoints (standard HTTP JSON APIs).
app.MapControllers();

// Start the web application and block until shutdown.
app.Run();


