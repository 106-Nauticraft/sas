using Diverse;
using NFluent;
using sample.api.Domain;
using sample.api.tests.xunit.GetWeatherForecasts.Api;
using sample.api.tests.xunit.GetWeatherForecasts.Scenario;
using Xunit.Abstractions;

namespace sample.api.tests.xunit.GetWeatherForecasts;

public class WeatherForecastApiShould
{

    public WeatherForecastApiShould(ITestOutputHelper testOutputHelper)
    {
        Fuzzer.Log = testOutputHelper.WriteLine;
    }

    [Fact]
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