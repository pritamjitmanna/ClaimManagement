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


builder.Services.AddAutoMapper(typeof(MapperProfile));



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
