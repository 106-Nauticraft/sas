using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NFluent;
using sas.Scenario;
using sas.Simulators;
using sas.Simulators.Options;

namespace sas.tests;

public class BaseOptionsSimulatorShould
{
    [Fact]
    public void Configure_the_options_from_the_scenario()
    {
        var simulator = new EndpointOptionsSimulator();
        var services = new ServiceCollection().AddOptions();

        simulator.RegisterTo(services, new EndpointScenario("https://first-scenario/"));

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<EndpointOptions>>();

        Check.That(options.Value.Url).IsEqualTo("https://first-scenario/");
    }

    [Fact]
    public void Leave_the_options_untouched_when_there_is_no_scenario()
    {
        var simulator = new EndpointOptionsSimulator();
        var services = new ServiceCollection().AddOptions();

        simulator.RegisterTo(services, BaseScenario.None);

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<EndpointOptions>>();

        Check.That(options.Value.Url).IsNull();
    }

    private class EndpointScenario(string url) : BaseScenario
    {
        public string Url { get; } = url;
    }

    private class EndpointOptions
    {
        public string? Url { get; set; }
    }

    private class EndpointOptionsSimulator : BaseOptionsSimulator<EndpointOptions>
    {
        protected override Action<EndpointOptions> Configure(BaseScenario scenario) =>
            options => options.Url = (scenario as EndpointScenario)?.Url;
    }
}