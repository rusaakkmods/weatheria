using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weatheria.Application.Abstractions;
using Weatheria.Application.Abstractions.Persistence;
using Weatheria.Infrastructure.Catalog;
using Weatheria.Infrastructure.Persistence;
using Weatheria.Infrastructure.Persistence.Repositories;
using Weatheria.Infrastructure.Weather;

namespace Weatheria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WeatheriaDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("Weatheria")));

        services.AddScoped<
            IWeatherNoteRepository,
            WeatherNoteRepository>();
        services.AddScoped<ICountryCatalog, CountryCatalog>();
        services.AddHttpClient<IWeatherService, OpenWeatherWeatherService>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["OpenWeather:BaseUrl"] ?? "https://api.openweathermap.org/");
        });

        return services;
    }
}