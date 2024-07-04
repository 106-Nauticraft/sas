using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;

namespace sas.Simulators;

public abstract class AbstractSimulator<T> : ISimulateBehaviour
    where T : class
{
    protected abstract T Instance { get; }
    
    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        if (scenario is NoScenario)
        {
            return;
        }
        services.AddSingleton(Instance);
        Simulate(scenario);
    }
        
    protected virtual void Simulate(BaseScenario scenario) { }
}