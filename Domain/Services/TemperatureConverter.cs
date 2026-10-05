namespace Weatheria.Domain.Services;

public static class TemperatureConverter
{
    public static double ConvertFahrenheitToCelsius(double fahrenheit)
    {
        return (fahrenheit - 32) * 5 / 9;
    }
}