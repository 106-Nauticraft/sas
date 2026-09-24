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

    [Fact]
    public void Serve_the_options_of_the_scenario_it_is_bound_to()
    {
        var simulator = new EndpointOptionsSimulator();
        var provider = Register(simulator, new EndpointScenario("https://first-scenario/"));
        var options = provider.GetRequiredService<IOptions<EndpointOptions>>();
        Check.That(options.Value.Url).IsEqualTo("https://first-scenario/");

        simulator.Bind(new EndpointScenario("https://second-scenario/"));

        Check.That(options.Value.Url).IsEqualTo("https://second-scenario/");
    }

    [Fact]
    public void Update_the_options_monitor_when_bound_again()
    {
        var simulator = new EndpointOptionsSimulator();
        var provider = Register(simulator, new EndpointScenario("https://first-scenario/"));
        var monitor = provider.GetRequiredService<IOptionsMonitor<EndpointOptions>>();
        Check.That(monitor.CurrentValue.Url).IsEqualTo("https://first-scenario/");
        string? notified = null;
        using var subscription = monitor.OnChange(options => notified = options.Url);

        simulator.Bind(new EndpointScenario("https://second-scenario/"));

        Check.That(monitor.CurrentValue.Url).IsEqualTo("https://second-scenario/");
        Check.That(notified).IsEqualTo("https://second-scenario/");
    }

    [Fact]
    public void Serve_the_options_of_the_scenario_it_is_bound_to_in_a_new_scope()
    {
        var simulator = new EndpointOptionsSimulator();
        var provider = Register(simulator, new EndpointScenario("https://first-scenario/"));
        using (var firstScope = provider.CreateScope())
        {
            var snapshot = firstScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<EndpointOptions>>();
            Check.That(snapshot.Value.Url).IsEqualTo("https://first-scenario/");
        }

        simulator.Bind(new EndpointScenario("https://second-scenario/"));

        using var secondScope = provider.CreateScope();
        var secondSnapshot = secondScope.ServiceProvider.GetRequiredService<IOptionsSnapshot<EndpointOptions>>();
        Check.That(secondSnapshot.Value.Url).IsEqualTo("https://second-scenario/");
    }

    [Fact]
    public void Forget_the_previous_scenario_when_bound_to_no_scenario()
    {
        var simulator = new EndpointOptionsSimulator();
        var provider = Register(simulator, new EndpointScenario("https://first-scenario/"));
        var options = provider.GetRequiredService<IOptions<EndpointOptions>>();
        Check.That(options.Value.Url).IsEqualTo("https://first-scenario/");

        simulator.Bind(BaseScenario.None);

        Check.That(options.Value.Url).IsNull();
    }

    [Fact]
    public void Configure_the_options_from_a_scenario_bound_after_registering_with_no_scenario()
    {
        var simulator = new EndpointOptionsSimulator();
        var provider = Register(simulator, BaseScenario.None);
        var options = provider.GetRequiredService<IOptions<EndpointOptions>>();
        Check.That(options.Value.Url).IsNull();

        simulator.Bind(new EndpointScenario("https://first-scenario/"));

        Check.That(options.Value.Url).IsEqualTo("https://first-scenario/");
    }

    private static ServiceProvider Register(ISimulateBehaviour simulator, BaseScenario scenario)
    {
        var services = new ServiceCollection().AddOptions();
        simulator.RegisterTo(services, scenario);
        return services.BuildServiceProvider();
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
