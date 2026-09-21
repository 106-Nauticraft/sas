using NFluent;
using sas.Api;
using sas.tests.api;

namespace sas.tests.Hosting;

public class BaseApiShould
{
    private static ApiHost<Startup> BuildHost() =>
        new(new GreetingScenario("nobody", "nothing"), [new GreetingSimulator()], []);

    [Fact]
    public async Task Build_its_own_host()
    {
        await using var api = GreetingApi.Create(new GreetingScenario("Ada", "Hello Ada"));

        var response = await api.Greet("Ada");

        Check.That(response).IsEqualTo("Hello Ada");
    }

    [Fact]
    public async Task Dispose_the_host()
    {
        var api = GreetingApi.Create(new GreetingScenario("Ada", "Hello Ada"));
        await api.Greet("Ada");

        await api.DisposeAsync();

        Check.ThatCode(api.GetRequiredService<IGreet>).Throws<ObjectDisposedException>();
    }

    [Fact]
    public async Task Run_on_the_host_given_in_input()
    {
        await using var host = BuildHost();

        await using var api = GreetingApi.RunningOn(host, new GreetingScenario("Ada", "Hello Ada"));

        Check.That(await api.Greet("Ada")).IsEqualTo("Hello Ada");
    }

    [Fact]
    public async Task Bind_the_given_host_to_its_own_scenario()
    {
        await using var host = BuildHost();

        await using var firstApi = GreetingApi.RunningOn(host, new GreetingScenario("Ada", "Hello Ada"));
        Check.That(await firstApi.Greet("Ada")).IsEqualTo("Hello Ada");

        await using var secondApi = GreetingApi.RunningOn(host, new GreetingScenario("Grace", "Hi Grace"));
        Check.That(await secondApi.Greet("Grace")).IsEqualTo("Hi Grace");
        Check.That(await secondApi.Greet("Ada")).IsEqualTo(GreetingSimulator.NotStubbed);
    }

    [Fact]
    public async Task Leave_the_given_host_alive_when_it_is_disposed()
    {
        await using var host = BuildHost();
        var api = GreetingApi.RunningOn(host, new GreetingScenario("Ada", "Hello Ada"));

        await api.DisposeAsync();

        using var client = host.CreateClient();
        Check.That(await client.GetStringAsync("/greet/Ada")).IsEqualTo("Hello Ada");
    }

    [Fact]
    public async Task Reach_the_simulators_of_the_given_host()
    {
        await using var host = BuildHost();
        await using var api = GreetingApi.RunningOn(host, new GreetingScenario("Ada", "Hello Ada"));

        Check.That(api.GetSimulator<GreetingSimulator>()).IsSameReferenceAs(host.GetSimulator<GreetingSimulator>());
    }
}
