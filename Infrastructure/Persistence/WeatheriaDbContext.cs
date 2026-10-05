using Microsoft.EntityFrameworkCore;
using Weatheria.Domain.Entities;

namespace Weatheria.Infrastructure.Persistence;

public sealed class WeatheriaDbContext(DbContextOptions<WeatheriaDbContext> options) : DbContext(options)
{
    public DbSet<WeatherNote> WeatherNotes => Set<WeatherNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatheriaDbContext).Assembly);
    }
}