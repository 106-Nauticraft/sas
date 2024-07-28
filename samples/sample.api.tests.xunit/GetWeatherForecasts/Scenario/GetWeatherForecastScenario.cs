using Diverse;
using sample.api.tests.xunit.GetWeatherForecasts.Scenario.Specifications;
using sas.Scenario;
using sas.Scenario.Setup;

namespace sample.api.tests.xunit.GetWeatherForecasts.Scenario;

public class GetWeatherForecastScenario : BaseScenario
{
    private readonly IFuzz _fuzzer;

    public CitySpecification City { get; private set; }
    public DateOnly Today { get; private set; }

    public int NbDays => DailyWeatherForecasts.Count;
    public ICollection<DailyWeatherForecastSpecification> DailyWeatherForecasts { get; }

    public GetWeatherForecastScenario(int? seed = null)
    { 
        _fuzzer = new Fuzzer(seed);
        City = new CitySpecification.Builder(_fuzzer).Build();
        Today = DateOnly.FromDateTime(DateTime.Today);

        DailyWeatherForecasts = GenerateForecasts();
    }
    
    public GetWeatherForecastScenario WithToday(DateOnly today)
    {
        ResetDailyForecasts();
        
        Today = today;
        return this;
    }

    private void ResetDailyForecasts()
    {
        if (DailyWeatherForecasts.Count > 0)
        {
            DailyWeatherForecasts.Clear();    
        }
    }

    private List<DailyWeatherForecastSpecification> GenerateForecasts()
    {
        var nbDays = _fuzzer.GenerateInteger(1, 15);

        return Enumerable.Range(1, nbDays)
            .Select(index => Today.AddDays(index))
            .Select(day => new DailyWeatherForecastSpecification.Builder(_fuzzer).Build(day)).ToList();
    }

    public GetWeatherForecastScenario AddDailyForecast(Setup<DailyWeatherForecastSpecification.Builder>? setup = null)
    {
        var builder = new DailyWeatherForecastSpecification.Builder(_fuzzer);

        DailyWeatherForecasts.Add(setup.Apply(builder).Build(Today.AddDays(NbDays + 1)));
        return this;
    }

    public GetWeatherForecastScenario WithCity(Setup<CitySpecification.Builder>? setup = null)
    {
        var builder = new CitySpecification.Builder(_fuzzer);
        City = setup.Apply(builder).Build();
        return this;
    }
}