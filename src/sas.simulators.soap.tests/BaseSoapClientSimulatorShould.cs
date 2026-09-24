using Microsoft.Extensions.DependencyInjection;
using NFluent;
using sas.Scenario;
using sas.simulators.http.Http;

namespace sas.simulators.soap.tests;

public class BaseSoapClientSimulatorShould
{
    [Fact]
    public void Simulate_the_scenario_it_is_registered_with()
    {
        var simulator = new EchoSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, EchoScenario.Echoing("Hello"));

        var client = services.BuildServiceProvider().GetRequiredService<IEchoSoapService>();

        Check.That(client.Echo("Ada")).IsEqualTo("Hello Ada");
    }

    [Fact]
    public void Not_stub_anything_when_there_is_no_scenario()
    {
        var simulator = new EchoSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, BaseScenario.None);

        var client = services.BuildServiceProvider().GetRequiredService<IEchoSoapService>();

        Check.ThatCode(() => client.Echo("Ada"))
            .Throws<HttpMessageInterceptionHandler.HttpRequestNotStubbedException>();
    }

    [Fact]
    public void Build_a_new_client_on_every_resolution()
    {
        var simulator = new EchoSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, EchoScenario.Echoing("Hello"));
        var provider = services.BuildServiceProvider();

        Check.That(provider.GetRequiredService<IEchoSoapService>())
            .IsDistinctFrom(provider.GetRequiredService<IEchoSoapService>());
    }

    [Fact]
    public void Simulate_the_new_scenario_and_forget_the_previous_one_when_bound_again()
    {
        var simulator = new EchoSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, EchoScenario.Echoing("Hello"));
        var provider = services.BuildServiceProvider();
        Check.That(provider.GetRequiredService<IEchoSoapService>().Echo("Ada")).IsEqualTo("Hello Ada");

        simulator.Bind(EchoScenario.Shouting("HELLO"));

        Check.That(provider.GetRequiredService<IEchoSoapService>().Shout("Ada")).IsEqualTo("HELLO ADA");
        Check.ThatCode(() => provider.GetRequiredService<IEchoSoapService>().Echo("Ada"))
            .Throws<HttpMessageInterceptionHandler.HttpRequestNotStubbedException>();
    }

    [Fact]
    public void Forget_the_requests_recorded_before_it_was_bound_again()
    {
        var simulator = new EchoSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, EchoScenario.Echoing("Hello"));
        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IEchoSoapService>().Echo("Ada");

        simulator.Bind(EchoScenario.Shouting("HELLO"));
        provider.GetRequiredService<IEchoSoapService>().Shout("Ada");

        simulator.CheckSoapRequestsRecorded(1);
    }
}