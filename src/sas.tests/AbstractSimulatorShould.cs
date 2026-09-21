using Microsoft.Extensions.DependencyInjection;
using NFluent;
using sas.Scenario;
using sas.Simulators;

namespace sas.tests;

public class AbstractSimulatorShould
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
    public void Register_its_instance_as_a_singleton()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new GreetingScenario("Ada", "Hello Ada"));

        var provider = services.BuildServiceProvider();

        Check.That(provider.GetRequiredService<IGreet>())
            .IsSameReferenceAs(provider.GetRequiredService<IGreet>());
    }

    [Fact]
    public void Register_its_instance_even_when_there_is_no_scenario()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, BaseScenario.None);

        var greeter = services.BuildServiceProvider().GetRequiredService<IGreet>();

        Check.That(greeter).IsNotNull();
        Check.That(greeter.Greet("Ada")).IsEqualTo(Greeter.NotStubbed);
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
        Check.That(greeter.Greet("Ada")).IsEqualTo(Greeter.NotStubbed);
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

    [Fact]
    public void Forget_the_previous_scenario_when_bound_to_no_scenario()
    {
        var simulator = new GreetingSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new GreetingScenario("Ada", "Hello Ada"));
        var greeter = services.BuildServiceProvider().GetRequiredService<IGreet>();

        simulator.Bind(BaseScenario.None);

        Check.That(greeter.Greet("Ada")).IsEqualTo(Greeter.NotStubbed);
    }

    private class GreetingScenario(string name, string greeting) : BaseScenario
    {
        public string Name { get; } = name;
        public string Greeting { get; } = greeting;
    }

    private interface IGreet
    {
        string Greet(string name);
    }

    private class Greeter : IGreet
    {
        public const string NotStubbed = "not stubbed";

        private readonly Dictionary<string, string> _greetings = new();

        public string Greet(string name) => _greetings.GetValueOrDefault(name, NotStubbed);

        public void Stub(string name, string greeting) => _greetings[name] = greeting;

        public void Clear() => _greetings.Clear();
    }

    private class GreetingSimulator : AbstractSimulator<IGreet>
    {
        private readonly Greeter _greeter = new();

        protected override IGreet Instance => _greeter;

        protected override void Reset() => _greeter.Clear();

        protected override void Simulate(BaseScenario scenario)
        {
            if (scenario is GreetingScenario greetingScenario)
            {
                _greeter.Stub(greetingScenario.Name, greetingScenario.Greeting);
            }
        }
    }
}