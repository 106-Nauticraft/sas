using Microsoft.AspNetCore.Mvc;
using sample.api.Domain;
using sample.api.HttpClients;
using TimeZone = sample.api.Domain.TimeZone;

namespace sample.api.Controllers;

[Controller]
[Route("[controller]")]
public class WeatherForecastController(OpenMeteoHttpClient openMeteoHttpClient) : ControllerBase
{
    [HttpGet]
    [Route("paris")]
    public async Task<IActionResult> GetParisWeatherForecast(int nbDays = 5)
    {
        var weatherForecast = await openMeteoHttpClient.GetDailyWeatherForecasts(Coordinates.Paris, TimeZone.Paris, nbDays);
        return Ok(weatherForecast);
    }
    
    [HttpGet]
    [Route("london")]
    public async Task<IActionResult> GetLondonWeatherForecast(int nbDays = 5)
    {
        var weatherForecast = await openMeteoHttpClient.GetDailyWeatherForecasts(Coordinates.London, TimeZone.London, nbDays);
        return Ok(weatherForecast);
    }
    
    [HttpGet]
    [Route("newyork")]
    public async Task<IActionResult> GetNewYorkWeatherForecast(int nbDays = 5)
    {
        var weatherForecast = await openMeteoHttpClient.GetDailyWeatherForecasts(Coordinates.NewYork, TimeZone.NewYork, nbDays);
        return Ok(weatherForecast);
    }
}