using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weatheria.Domain.Entities;

namespace Weatheria.Infrastructure.Persistence.Configurations;

public sealed class WeatherNoteConfiguration : IEntityTypeConfiguration<WeatherNote>
{
    public void Configure(EntityTypeBuilder<WeatherNote> builder)
    {
        builder.ToTable("WeatherNotes");
        builder.HasKey(note => note.Id);
        builder.Property(note => note.City)
        .IsRequired()
        .HasMaxLength(100);

        builder.Property(note => note.Content)
        .IsRequired()
        .HasMaxLength(1000);

        builder.Property(note => note.CreatedAt)
        .IsRequired();


    }    
}