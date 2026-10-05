using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Weatheria.Application.Abstractions;
using Weatheria.Infrastructure.Persistence;

namespace Weatheria.Tests.Api;

public sealed class WeatheriaWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IWeatherService? _weatherService;
    private SqliteConnection? _connection;

    public WeatheriaWebApplicationFactory(IWeatherService? weatherService = null)
    {
        _weatherService = weatherService;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                x => x.ServiceType == typeof(DbContextOptions<WeatheriaDbContext>)
            );

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("Data Source=:memory:");

            _connection.Open();

            services.AddDbContext<WeatheriaDbContext>(options => options.UseSqlite(_connection));
        });

        if (_weatherService is not null)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IWeatherService>();
                services.AddSingleton(_weatherService);
            });
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection?.Dispose();
    }
}