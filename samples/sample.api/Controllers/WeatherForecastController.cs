using Microsoft.AspNetCore.Mvc;
using sample.api.Domain;
using sample.api.HttpClients;
using sample.api.Infra.HttpClients;
using TimeZone = sample.api.Domain.TimeZone;

namespace sample.api.Controllers;

[Controller]
[Route("[controller]")]
public class WeatherForecastController(WeatherForecasts weatherForecasts) : ControllerBase
{
    [HttpGet]
    [Route("paris")]
    public async Task<IActionResult> GetParisWeatherForecast(int nbDays = 5)
    {
        var weatherForecast = await weatherForecasts.GetDailyForecastsByCoordinates(Coordinates.Paris, TimeZone.Paris, nbDays);
        return Ok(weatherForecast);
    }
    
    [HttpGet]
    [Route("london")]
    public async Task<IActionResult> GetLondonWeatherForecast(int nbDays = 5)
    {
        var weatherForecast = await weatherForecasts.GetDailyForecastsByCoordinates(Coordinates.London, TimeZone.London, nbDays);
        return Ok(weatherForecast);
    }
    
    [HttpGet]
    [Route("newyork")]
    public async Task<IActionResult> GetNewYorkWeatherForecast(int nbDays = 5)
    {
        var weatherForecast = await weatherForecasts.GetDailyForecastsByCoordinates(Coordinates.NewYork, TimeZone.NewYork, nbDays);
        return Ok(weatherForecast);
    }
    
    [HttpGet]
    [Route("{townName}")]
    public async Task<IActionResult> GetWeatherForecast(string townName, int nbDays = 5)
    {
        var weatherForecast = await weatherForecasts.GetDailyForecastsByTownName(townName, nbDays);
        return Ok(weatherForecast);
    }
    
    
}