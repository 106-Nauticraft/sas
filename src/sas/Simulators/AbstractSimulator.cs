using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;

namespace sas.Simulators;

public abstract class AbstractSimulator<T> : ISimulateBehaviour, IBindScenario
    where T : class
{
    protected abstract T Instance { get; }

    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        services.AddSingleton(Instance);
        Bind(scenario);
    }

    public void Bind(BaseScenario scenario)
    {
        Reset();
        if (scenario is not NoScenario)
            Simulate(scenario);
    }

    protected virtual void Reset() { }
    protected virtual void Simulate(BaseScenario scenario) { }
}