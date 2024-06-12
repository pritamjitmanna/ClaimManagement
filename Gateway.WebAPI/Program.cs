using Ocelot.Middleware;
using Ocelot.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("configuration.json").Build();

builder.Services.AddOcelot(configuration);

var app = builder.Build();

// app.MapGet("/", () => "Hello World!");
app.UseAuthorization();
await app.UseOcelot();

app.Run();
