namespace sample.api.Domain;

public record DailyWeatherForecast(
    DateOnly Date,
    Temperature TemperatureMinC,
    Temperature TemperatureMaxC,
    string? Summary)
{
    public Temperature TemperatureMinF { get; } = TemperatureMinC.ToFahrenheit();
    public Temperature TemperatureMaxF { get; } = TemperatureMaxC.ToFahrenheit();
};