using Weatheria.Application.Abstractions;
using Weatheria.Infrastructure.Catalog;
using Weatheria.Application.Features.Countries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICountryCatalog, CountryCatalog>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetCountriesQuery).Assembly));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();