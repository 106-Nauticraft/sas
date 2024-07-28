using Diverse;

namespace sample.api.tests.xunit.GetWeatherForecasts.Scenario.Specifications;

public record TimeZoneSpecification(string Name) 
{
    public class Builder(IFuzz fuzzer)
    {
        private readonly string _name = fuzzer.PickOneFrom(new[] { "Europe/Paris", "Europe/London", "America/New_York" });
        
        public TimeZoneSpecification Build()
        {
            return new TimeZoneSpecification(_name);
        }
    }
}