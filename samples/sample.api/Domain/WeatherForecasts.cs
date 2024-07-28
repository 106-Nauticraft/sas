namespace sample.api.Domain;

public class WeatherForecasts(WeatherForecastsProvider weatherForecastsProvider, CityLocator cityLocator)
{
    public async Task<IEnumerable<DailyWeatherForecast>> GetDailyForecastsByTownName(string town, int nbDays)
    {
        var (coordinates, timezone) = cityLocator.Locate(town);
        return await GetDailyForecastsByCoordinates(coordinates, timezone, nbDays);
    }
    
    
    public async Task<IEnumerable<DailyWeatherForecast>> GetDailyForecastsByCoordinates(Coordinates coordinates, TimeZone timeZone, int nbDays)
    {
        return await weatherForecastsProvider.GetDailyWeatherForecasts(coordinates, timeZone, nbDays);
    }
}