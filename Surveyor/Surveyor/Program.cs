#pragma warning disable CS8604 // Possible null reference argument.

using gRPCClaimsService.Protos;
using Microsoft.EntityFrameworkCore;
using Surveyor.BLL;
using Surveyor.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SurveyorDBContext>(options=>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Surveyor"));
    });

builder.Services.AddGrpcClient<ClaimsService.ClaimsServiceClient>(options=>options.Address=new Uri(builder.Configuration.GetSection("URLs").GetSection("InsuranceCompanyURL").Value));

builder.Services.AddScoped<ISurveyor,SurveyorRepository>();
builder.Services.AddScoped<ISurveyorService,SurveyorService>();


builder.Services.AddAutoMapper(typeof(MapperProfile));

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
