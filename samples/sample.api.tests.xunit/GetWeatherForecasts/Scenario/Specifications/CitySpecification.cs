using Diverse;

namespace sample.api.tests.xunit.GetWeatherForecasts.Scenario.Specifications;

public record CitySpecification(string Name, CoordinatesSpecification Coordinates, TimeZoneSpecification TimeZone)
{
    public class Builder(IFuzz fuzzer)
    {
        private string _name = fuzzer.PickOneFrom(new[] { "paris", "london", "new-york" });
        private CoordinatesSpecification _coordinates = new CoordinatesSpecification.Builder(fuzzer).Build();
        private TimeZoneSpecification _timeZone = new TimeZoneSpecification.Builder(fuzzer).Build();
        
        public CitySpecification Build()
        {
            return new CitySpecification(_name, _coordinates, _timeZone);
        }

        public Builder WithName(string name)
        {
            _name = name;
            return this;
        }
        
        public Builder WithCoordinates(double latitude, double longitude)
        {
            _coordinates = new CoordinatesSpecification(latitude, longitude);
            return this;
        }

        public Builder WithTimeZone(string timeZone)
        {
            _timeZone = new TimeZoneSpecification(timeZone);
            return this;
        }
    }
}