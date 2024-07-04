using System.Globalization;
using System.Text.Json.Serialization;
using sample.api.Domain;
using sample.api.HttpClients.Tools;
using TimeZone = sample.api.Domain.TimeZone;

namespace sample.api.HttpClients;

public class OpenMeteoHttpClient(HttpClient httpClient)
{
    private record OpenMeteoDailyForecastsResponse(string[] Time, int[] WeatherCode, 
        [property:JsonPropertyName("temperature_2m_min")] decimal[] Temperature2mMin,
        [property:JsonPropertyName("temperature_2m_max")] decimal[] Temperature2mMax);
    private record OpenMeteoForecastsResponse(OpenMeteoDailyForecastsResponse Daily);
    
    public async Task<IEnumerable<DailyWeatherForecast>> GetDailyWeatherForecasts(Coordinates coordinates, TimeZone timeZone, int nbDays)
    {
        var url = UrlBuilder.From("forecast")
            .With("latitude", coordinates.Latitude.ToString(CultureInfo.CreateSpecificCulture("en-US")))
            .With("longitude", coordinates.Longitude.ToString(CultureInfo.CreateSpecificCulture("en-US")))
            .With("timezone", timeZone.Name)
            .With("forecast_days", nbDays, days => days is > 0 and <= 15)
            .With("daily", new []{
                "weathercode", "temperature_2m_min", "temperature_2m_max"
            })
            .Build();
        
        var response = await httpClient.GetFromJsonAsync<OpenMeteoForecastsResponse>(url);

        if (response is null)
        {
            throw new Exception("No response from OpenMeteo");
        }
        
        var forecasts = new List<DailyWeatherForecast>();
        
        for(var index = 0; index < response.Daily.Time.Length; index++)
        {
            var time = DateOnly.ParseExact(response.Daily.Time[index], "yyyy-MM-dd");
            var weatherCode = response.Daily.WeatherCode[index];
            var temperature2MMin = new Temperature(response.Daily.Temperature2mMin[index]);
            var temperature2MMax = new Temperature(response.Daily.Temperature2mMax[index]);
            
            forecasts.Add(new DailyWeatherForecast(time, temperature2MMin, temperature2MMax, weatherCode.ToString()));
        }
        
        
        return forecasts;
    }

}