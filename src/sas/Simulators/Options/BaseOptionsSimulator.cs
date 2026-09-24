using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using sas.Scenario;

namespace sas.Simulators.Options;

public abstract class BaseOptionsSimulator<TOptions> : ISimulateBehaviour, IBindScenario
    where TOptions : class
{
    private readonly ScenarioChangeTokenSource _scenarioChanged = new();
    private BaseScenario _scenario = BaseScenario.None;

    protected abstract Action<TOptions> Configure(BaseScenario scenario);

    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        // The scenario is read each time the options are built.
        services.Configure<TOptions>(ConfigureFromCurrentScenario);
        services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(_scenarioChanged);
        // The default IOptions<T> computes its value once for the lifetime of the host and would never see a Bind.
        // A closed registration takes precedence over the open generic one, whatever the order.
        services.AddSingleton<IOptions<TOptions>>(provider =>
            new CurrentOptions(provider.GetRequiredService<IOptionsMonitor<TOptions>>()));

        Bind(scenario);
    }

    public void Bind(BaseScenario scenario)
    {
        _scenario = scenario;
        _scenarioChanged.Signal();
    }

    private void ConfigureFromCurrentScenario(TOptions options)
    {
        if (_scenario is not NoScenario)
        {
            var configure = Configure(_scenario);
            configure(options);
        }
    }

    private sealed class CurrentOptions(IOptionsMonitor<TOptions> monitor) : IOptions<TOptions>
    {
        public TOptions Value => monitor.CurrentValue;
    }

    /// <summary>
    /// Makes <see cref="IOptionsMonitor{TOptions}"/> drop its cached value and notify its listeners. See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/change-tokens
    /// </summary>
    private sealed class ScenarioChangeTokenSource : IOptionsChangeTokenSource<TOptions>
    {
        private CancellationTokenSource _changed = new();

        public string Name => Microsoft.Extensions.Options.Options.DefaultName;

        public IChangeToken GetChangeToken() => new CancellationChangeToken(_changed.Token);

        // Swap first, then cancel the old one. The monitor asks for its next token from within the callback:
        // cancelling first hands it the already cancelled token, whose callback re-fires on registration, until stack overflow.
        public void Signal() => Interlocked.Exchange(ref _changed, new CancellationTokenSource()).Cancel();
    }
}
