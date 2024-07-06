using NFluent;
using sample.api.Domain;
using sample.api.tests.nunit.GetWeatherForecasts.Api;
using sample.api.tests.nunit.GetWeatherForecasts.Scenario;

namespace sample.api.tests.nunit.GetWeatherForecasts;

public class WeatherForecastApiShould
{
    [Test]
    [Category("integration")]
    public async Task Return_Paris_Weather_using_real_api()
    {
        var api = WeatherForecastApi.CreateForIntegrationTests();
        var parisWeatherForecast = await api.GetParisWeatherForecast(5);
        Check.That(parisWeatherForecast).IsOk<List<DailyWeatherForecast>>()
            .WhichPayload(forecasts =>
            {
                Check.That(forecasts).IsNotNull();
                Check.That(forecasts).HasSize(5);
            });
    }
    
    
    [Test]
    public async Task Return_Paris_Weather_Forecast()
    {
        var scenario = new GetWeatherForecastScenario()
            .WithToday(DateOnly.Parse("2024-01-01"))
            .AddDailyForecast(forecast => 
                forecast.WithTemperatureMin(0m).WithTemperatureMax(23.5m)
            )
            .AddDailyForecast(forecast => 
                forecast.WithTemperatureMin(3.9m).WithTemperatureMax(19.2m)
            );

        var api = WeatherForecastApi.Create(scenario);

        var parisWeatherForecast = await api.GetParisWeatherForecast();
        
        Check.That(parisWeatherForecast).IsOk<List<DailyWeatherForecast>>()
            .WhichPayload(forecasts =>
        {
            Check.That(forecasts).IsNotNull();
            Check.That(forecasts).HasSize(2);
            
            var dayOneForecast = forecasts![0];
            
            Check.That(dayOneForecast.Date).IsEqualTo(DateOnly.Parse("2024-01-02"));
            Check.That(dayOneForecast.TemperatureMinC.Value).IsEqualTo(0m);
            Check.That(dayOneForecast.TemperatureMaxC.Value).IsEqualTo(23.5m);
            
            var dayTwoForecast = forecasts[1];
            
            Check.That(dayTwoForecast.Date).IsEqualTo(DateOnly.Parse("2024-01-03"));
            Check.That(dayTwoForecast.TemperatureMinC.Value).IsEqualTo(3.9m);
            Check.That(dayTwoForecast.TemperatureMaxC.Value).IsEqualTo(19.2m);
            
        });
    }
}