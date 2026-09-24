using Microsoft.Extensions.DependencyInjection;
using NFluent;
using NFluent.ApiChecks;
using sas.Api;
using sas.Scenario;
using sas.Simulators;
using sas.tests.api;

namespace sas.tests.Hosting;

public class ApiHostShould
{
    [Fact]
    public async Task Serve_the_scenario_it_was_built_with()
    {
        await using var host = BuildHost(new GreetingScenario("Ada", "Hello Ada"));
        using var client = host.CreateClient();

        Check.That(await client.GetStringAsync("/greet/Ada")).IsEqualTo("Hello Ada");
    }

    [Fact]
    public async Task Leave_the_application_untouched_where_no_simulator_replaces_it()
    {
        await using var host = new ApiHost<Startup>(new GreetingScenario("Ada", "Hello Ada"), [], []);
        using var client = host.CreateClient();

        Check.That(await client.GetStringAsync("/greet/Ada")).StartsWith(Greeter.RealImplementation);
    }

    [Fact]
    public async Task Serve_the_new_scenario_and_forget_the_previous_one_when_bound_again()
    {
        await using var host = BuildHost(new GreetingScenario("Ada", "Hello Ada"));
        using var client = host.CreateClient();

        host.Bind(new GreetingScenario("Grace", "Hi Grace"));

        Check.That(await client.GetStringAsync("/greet/Grace")).IsEqualTo("Hi Grace");
        Check.That(await client.GetStringAsync("/greet/Ada")).IsEqualTo(GreetingSimulator.NotStubbed);
    }

    [Fact]
    public async Task Keep_the_application_it_already_built_when_bound_again()
    {
        await using var host = BuildHost(new GreetingScenario("Ada", "Hello Ada"));
        var servicesBeforeBind = host.Services;
        var greeterBeforeBind = host.GetRequiredService<IGreet>();

        host.Bind(new GreetingScenario("Grace", "Hi Grace"));

        Check.That(host.Services).IsSameReferenceAs(servicesBeforeBind);
        Check.That(host.GetRequiredService<IGreet>()).IsSameReferenceAs(greeterBeforeBind);
    }

    [Fact]
    public async Task Bind_a_scenario_on_a_host_nothing_has_resolved_yet()
    {
        await using var host = BuildHost(new GreetingScenario("Ada", "Hello Ada"));

        host.Bind(new GreetingScenario("Grace", "Hi Grace"));

        using var client = host.CreateClient();
        Check.That(await client.GetStringAsync("/greet/Grace")).IsEqualTo("Hi Grace");
    }

    [Fact]
    public async Task Serve_the_options_of_the_new_scenario_when_bound_again()
    {
        await using var host = new ApiHost<Startup>(new GreetingPrefixScenario("Hey"),
            [new GreetingPrefixSimulator()], []);
        using var client = host.CreateClient();
        Check.That(await client.GetStringAsync("/prefix")).IsEqualTo("Hey");

        host.Bind(new GreetingPrefixScenario("Yo"));

        Check.That(await client.GetStringAsync("/prefix")).IsEqualTo("Yo");
    }

    [Fact]
    public async Task Configure_the_options_of_a_scenario_bound_after_being_built_with_no_scenario()
    {
        await using var host = new ApiHost<Startup>(BaseScenario.None, [new GreetingPrefixSimulator()], []);
        using var client = host.CreateClient();
        Check.That(await client.GetStringAsync("/prefix")).IsEqualTo(GreetingOptions.NotConfigured);

        host.Bind(new GreetingPrefixScenario("Hey"));

        Check.That(await client.GetStringAsync("/prefix")).IsEqualTo("Hey");
    }

    [Fact]
    public async Task Refuse_to_bind_a_scenario_when_a_simulator_cannot_follow()
    {
        await using var host = new ApiHost<Startup>(BaseScenario.None, [new UnboundSimulator()], []);

        Check.ThatCode(() => host.Bind(BaseScenario.None))
            .Throws<SimulatorCannotBeReboundException>()
            .AndWhichMessage()
            .Contains(nameof(UnboundSimulator), nameof(IBindScenario));
    }

    [Fact]
    public async Task Name_every_simulator_that_cannot_follow_when_it_refuses_to_bind()
    {
        await using var host = new ApiHost<Startup>(BaseScenario.None,
            [new GreetingSimulator(), new UnboundSimulator(), new AnotherUnboundSimulator()], []);

        Check.ThatCode(() => host.Bind(BaseScenario.None))
            .Throws<SimulatorCannotBeReboundException>()
            .AndWhichMessage()
            .Contains(nameof(UnboundSimulator), nameof(AnotherUnboundSimulator));
    }

    [Fact]
    public async Task Leave_every_simulator_on_its_current_scenario_when_it_refuses_to_bind()
    {
        await using var host = new ApiHost<Startup>(new GreetingScenario("Ada", "Hello Ada"),
            [new GreetingSimulator(), new UnboundSimulator()], []);
        using var client = host.CreateClient();

        Check.ThatCode(() => host.Bind(new GreetingScenario("Grace", "Hi Grace")))
            .Throws<SimulatorCannotBeReboundException>();

        Check.That(await client.GetStringAsync("/greet/Ada")).IsEqualTo("Hello Ada");
    }

    private static ApiHost<Startup> BuildHost(BaseScenario scenario) =>
        new(scenario, [new GreetingSimulator()], []);

    public class UnboundSimulator : ISimulateBehaviour
    {
        public void RegisterTo(IServiceCollection services, BaseScenario scenario) { }
    }

    public class AnotherUnboundSimulator : ISimulateBehaviour
    {
        public void RegisterTo(IServiceCollection services, BaseScenario scenario) { }
    }
}
