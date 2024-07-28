namespace sample.api.Domain;

public interface WeatherForecastsProvider
{
    Task<IEnumerable<DailyWeatherForecast>> GetDailyWeatherForecasts(Coordinates coordinates, TimeZone timeZone,int nbDays);
}