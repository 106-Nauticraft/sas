using Diverse;

namespace sample.api.tests.xunit.GetWeatherForecasts.Scenario;

public record DailyWeatherForecastSpecification(DateOnly Date, decimal TemperatureMinC, decimal TemperatureMaxC, string WeatherCode)
{
    public class Builder(IFuzz fuzzer)
    {
        private decimal _temperatureMinC = Math.Round(fuzzer.GenerateDecimal(-20m, 20m), 2);
        private decimal _temperatureMaxC = Math.Round(fuzzer.GenerateDecimal(-20m, 20m), 2);
        private readonly string _weatherCode = fuzzer.GenerateInteger(0, 100).ToString();


        public Builder WithTemperatureMin(decimal temperatureC)
        {
            _temperatureMinC = temperatureC;
            return this;
        }
        
        public Builder WithTemperatureMax(decimal temperatureC)
        {
            _temperatureMaxC = temperatureC;
            return this;
        }

        public DailyWeatherForecastSpecification Build(DateOnly date)
        {
            return new DailyWeatherForecastSpecification(date, _temperatureMinC, _temperatureMaxC, _weatherCode);
        }
    }
}