using Diverse;

namespace sample.api.tests.xunit.GetWeatherForecasts.Scenario.Specifications;

public record CoordinatesSpecification(double Latitude, double Longitude)
{
    public class Builder(IFuzz fuzzer)
    {
        private readonly double _latitude = Convert.ToDouble(fuzzer.GenerateDecimal(0, 90));
        private readonly double _longitude = Convert.ToDouble(fuzzer.GenerateDecimal(-180, 180));
        
        public CoordinatesSpecification Build()
        {
            return new CoordinatesSpecification(_latitude, _longitude);
        }
    }   
}