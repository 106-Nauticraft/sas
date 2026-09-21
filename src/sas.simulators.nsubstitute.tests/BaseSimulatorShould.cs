using Microsoft.Extensions.DependencyInjection;
using NFluent;
using NSubstitute;
using sas.Scenario;

namespace sas.simulators.nsubstitute.tests;

public class BaseSimulatorShould
{
    [Fact]
    public void Simulate_the_scenario_it_is_registered_with()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new GreetingScenario("Ada", "Hello Ada"));

        var greeter = services.BuildServiceProvider().GetRequiredService<IGreet>();

        Check.That(greeter.Greet("Ada")).IsEqualTo("Hello Ada");
    }

    [Fact]
    public void Simulate_the_new_scenario_and_forget_the_previous_one_when_bound_again()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new GreetingScenario("Ada", "Hello Ada"));
        var greeter = services.BuildServiceProvider().GetRequiredService<IGreet>();

        simulator.Bind(new GreetingScenario("Grace", "Hi Grace"));

        Check.That(greeter.Greet("Grace")).IsEqualTo("Hi Grace");
        Check.That(greeter.Greet("Ada")).IsEmpty();
    }

    [Fact]
    public void Forget_the_calls_recorded_before_it_was_bound_again()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new GreetingScenario("Ada", "Hello Ada"));
        var greeter = services.BuildServiceProvider().GetRequiredService<IGreet>();
        greeter.Greet("Ada");

        simulator.Bind(new GreetingScenario("Grace", "Hi Grace"));

        Check.That(greeter.ReceivedCalls()).IsEmpty();
    }

    [Fact]
    public void Keep_the_instance_the_container_captured_when_bound_again()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new GreetingScenario("Ada", "Hello Ada"));
        var provider = services.BuildServiceProvider();
        var greeterBeforeBind = provider.GetRequiredService<IGreet>();

        simulator.Bind(new GreetingScenario("Grace", "Hi Grace"));

        Check.That(provider.GetRequiredService<IGreet>()).IsSameReferenceAs(greeterBeforeBind);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public interface IGreet
    {
        string Greet(string name);
    }

    private class GreetingScenario(string name, string greeting) : BaseScenario
    {
        public string Name { get; } = name;
        public string Greeting { get; } = greeting;
    }

    private class GreetingSimulator : BaseSimulator<IGreet>
    {
        protected override void Simulate(BaseScenario scenario)
        {
            if (scenario is GreetingScenario greetingScenario)
            {
                Instance.Greet(greetingScenario.Name).Returns(greetingScenario.Greeting);
            }
        }
    }
}