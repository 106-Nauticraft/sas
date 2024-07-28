using Microsoft.Extensions.DependencyInjection;
using sample.api.Domain;
using sample.api.tests.xunit.GetWeatherForecasts.Scenario;
using sas.Scenario;
using sas.Simulators;
using TimeZone = sample.api.Domain.TimeZone;

namespace sample.api.tests.xunit.GetWeatherForecasts.Simulators;

public class CityLocatorSimulator : ISimulateBehaviour
{
    private class FakeCityLocator(GetWeatherForecastScenario scenario) : CityLocator
    {
        public (Coordinates coordinates, TimeZone timeZone) Locate(string citySearchInput)
        {
            if (citySearchInput != scenario.City.Name)
            {
                throw new InvalidOperationException($"Unknown city matching {citySearchInput}");
            }
            
            var (latitude, longitude) = scenario.City.Coordinates;
            var coordinates = new Coordinates(latitude, longitude);
            var timeZone = new TimeZone(scenario.City.TimeZone.Name);
            return (coordinates, timeZone);
        }
    }
    
    
    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        if (scenario is not GetWeatherForecastScenario getWeatherForecastScenario)
        {
            return;
        }

        services.AddTransient<CityLocator>(_ => new FakeCityLocator(getWeatherForecastScenario));
    }
}