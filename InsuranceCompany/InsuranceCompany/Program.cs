using System.Text.Json.Serialization;
using InsuranceCompany;
using InsuranceCompany.BLL;
using InsuranceCompany.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<InsuranceCompanyDBContext>(options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("InsuranceCompany"));
});

//Configuring the Dependencies in the application

builder.Services.AddScoped<IClaimDetailService, ClaimDetailService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<ISurveyorService, SurveyorService>();
builder.Services.AddScoped<IClaimDetail, ClaimDetailRepository>();
builder.Services.AddScoped<IFee, FeeRepository>();
builder.Services.AddScoped<IPolicy, PolicyRepository>();
builder.Services.AddScoped<ISurveyor, SurveyorRepository>();
builder.Services.AddScoped<ISharedLogic,SharedLogic>();

//AutoMapping service
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddAutoMapper(typeof(GRPCAutoMapperProfile));

builder.Services.AddGrpc();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    options.JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGrpcService<ClaimsServices>();
// app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseCors("AllowAngularOrigins");


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


