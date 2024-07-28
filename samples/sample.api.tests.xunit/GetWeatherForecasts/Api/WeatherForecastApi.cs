using sample.api.tests.xunit.GetWeatherForecasts.Scenario;
using sample.api.tests.xunit.GetWeatherForecasts.Simulators;
using sas.Api;
using sas.Configurations;
using sas.Scenario;
using sas.Simulators;

namespace sample.api.tests.xunit.GetWeatherForecasts.Api;

public class WeatherForecastApi : BaseApi<Startup>
{
    private WeatherForecastApi(
        BaseScenario scenario, 
        ISimulateBehaviour[] simulators, 
        IEnrichConfiguration[] configurations) 
        : base(scenario, simulators, configurations) { }

    public static WeatherForecastApi Create(BaseScenario scenario)
    {
        return new WeatherForecastApi(scenario, [
            new OpenMeteoHttpClientSimulator(),
            new CityLocatorSimulator()
        ], []);
    }
    
    public static WeatherForecastApi CreateForIntegrationTests()
    {
        return new WeatherForecastApi(BaseScenario.None, [], []);
    }

    public async Task<HttpResponseMessage> GetParisWeatherForecast(int? nbDays = null)
    {
        nbDays = Defaulting(nbDays).From<GetWeatherForecastScenario>(scenario => scenario.NbDays);
        return await HttpClient.GetAsync($"/WeatherForecast/paris?nbDays={nbDays}");
    }
    
    public async Task<HttpResponseMessage> GetWeatherForecast(string city = "", int? nbDays = null)
    {
        nbDays = Defaulting(nbDays).From<GetWeatherForecastScenario>(scenario => scenario.NbDays);
        city = Defaulting(city).From<GetWeatherForecastScenario>(scenario => scenario.City.Name);
        
        return await HttpClient.GetAsync($"/WeatherForecast/{city}?nbDays={nbDays}");
    }
}