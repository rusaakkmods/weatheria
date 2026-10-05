namespace Weatheria.Domain.Entities;

public sealed class WeatherNote
{
    private WeatherNote(
        Guid id,
        string city,
        string content,
        DateTimeOffset createdAt
    )

    {
        Id = id;
        City = city;
        Content = content;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string City { get; }
    public string Content { get; }
    public DateTimeOffset CreatedAt { get; }

    public static WeatherNote Create(string city, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new WeatherNote(
            Guid.NewGuid(),
            city.Trim(),
            content.Trim(),
            DateTimeOffset.UtcNow
        );
    }
}