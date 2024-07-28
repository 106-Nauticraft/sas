using System.Net;
using System.Net.Http.Json;
using System.Web;
using NSubstitute;
using sample.api.HttpClients;
using sample.api.Infra.HttpClients;
using sample.api.tests.xunit.GetWeatherForecasts.Scenario;
using sample.api.tests.xunit.GetWeatherForecasts.Scenario.Specifications;
using sas.Scenario;
using sas.simulators.http.nsubstitute;

namespace sample.api.tests.xunit.GetWeatherForecasts.Simulators;

public class OpenMeteoHttpClientSimulator : BaseHttpClientSimulator<OpenMeteoHttpClient>
{
    protected override void Simulate(BaseScenario scenario)
    {
        if (scenario is not GetWeatherForecastScenario getWeatherForecastScenario)
        {
            return;
        }

        SimulateDailyWeatherForecasts(getWeatherForecastScenario.DailyWeatherForecasts);
    }

    private void SimulateDailyWeatherForecasts(ICollection<DailyWeatherForecastSpecification> dailyWeatherForecasts)
    {
        HttpClient.Get(Arg.Is<string>(arg => arg.StartsWith("forecast")))
            .Returns(callInfo =>
            {
                var forecastDays = int.Parse(GetQueryParam(callInfo.Arg<string>(), "forecast_days"));
                
                var requestedDailyWeatherForecasts = dailyWeatherForecasts.Take(forecastDays).ToArray();
                
                var content = new
                {
                    daily = new
                    {
                        time = requestedDailyWeatherForecasts.Select(forecast => forecast.Date.ToString("yyyy-MM-dd")).ToArray(),
                        weatherCode = requestedDailyWeatherForecasts.Select(forecast => forecast.WeatherCode).ToArray(),
                        temperature_2m_min =
                            requestedDailyWeatherForecasts.Select(forecast => forecast.TemperatureMinC).ToArray(),
                        temperature_2m_max =
                            requestedDailyWeatherForecasts.Select(forecast => forecast.TemperatureMaxC).ToArray(),
                    }
                };


                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(content)
                };
            });
    }

    private static string GetQueryParam(string url, string? paramName)
    {
        return HttpUtility.ParseQueryString(url).Get(paramName) ?? throw new Exception($"No query param '{paramName}' found in '{url}'");
    }

    public void DailyWeatherForecastWasCalledWith(string latitude, string longitude, string forecast_days, string timezone)
    {
        Spy.AGetRequestTo("forecast")
            .WithQueryParam("latitude",latitude)
            .WithQueryParam("longitude",longitude)
            .WithQueryParam("forecast_days",forecast_days)
            .WithQueryParam("timezone",timezone)
            .OccurredOnce();
    } 
}