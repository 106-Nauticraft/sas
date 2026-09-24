using Microsoft.Extensions.DependencyInjection;
using sas.Api;
using sas.Configurations;
using sas.Scenario;
using sas.Simulators;
using sas.Simulators.Options;
using sas.tests.api;

namespace sas.tests.Hosting;

public class GreetingApi : BaseApi<Startup>
{
    private GreetingApi(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations)
        : base(scenario, simulators, configurations) { }

    private GreetingApi(BaseScenario scenario, ApiHost<Startup> borrowedHost)
        : base(scenario, borrowedHost) { }

    public static GreetingApi Create(BaseScenario scenario) => new(scenario, [new GreetingSimulator()], []);

    public static GreetingApi RunningOn(ApiHost<Startup> borrowedHost, BaseScenario scenario) =>
        new(scenario, borrowedHost);

    public Task<string> Greet(string name) => HttpClient.GetStringAsync($"/greet/{name}");
}

public class LazyGreetingApi : LazyBaseApi<Startup>
{
    private LazyGreetingApi(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations)
        : base(scenario, simulators, configurations) { }

    private LazyGreetingApi(BaseScenario scenario, ApiHost<Startup> borrowedHost)
        : base(scenario, borrowedHost) { }

    public static LazyGreetingApi Create(BaseScenario scenario) => new(scenario, [new GreetingSimulator()], []);

    public static LazyGreetingApi RunningOn(ApiHost<Startup> borrowedHost, BaseScenario scenario) =>
        new(scenario, borrowedHost);

    public Task<string> Greet(string name) => BuildHttpClient().GetStringAsync($"/greet/{name}");

    public Task<string> Prefix(Action<IServiceCollection> postSetup) => BuildHttpClient(postSetup).GetStringAsync("/prefix");
}

public class GreetingScenario(string name, string greeting) : BaseScenario
{
    public string Name { get; } = name;
    public string Greeting { get; } = greeting;
}

public class GreetingPrefixScenario(string prefix) : BaseScenario
{
    public string Prefix { get; } = prefix;
}

public class GreetingSimulator : AbstractSimulator<IGreet>
{
    public const string NotStubbed = "not stubbed";

    private readonly StubbedGreeter _greeter = new();

    protected override IGreet Instance => _greeter;

    protected override void Reset() => _greeter.Clear();

    protected override void Simulate(BaseScenario scenario)
    {
        if (scenario is GreetingScenario greeting)
        {
            _greeter.Stub(greeting.Name, greeting.Greeting);
        }
    }

    private class StubbedGreeter : IGreet
    {
        private readonly Dictionary<string, string> _greetings = new();

        public string Greet(string name) => _greetings.GetValueOrDefault(name, NotStubbed);

        public void Stub(string name, string greeting) => _greetings[name] = greeting;

        public void Clear() => _greetings.Clear();
    }
}

public class GreetingPrefixSimulator : BaseOptionsSimulator<GreetingOptions>
{
    protected override Action<GreetingOptions> Configure(BaseScenario scenario) =>
        options => options.Prefix = (scenario as GreetingPrefixScenario)?.Prefix ?? GreetingOptions.NotConfigured;
}