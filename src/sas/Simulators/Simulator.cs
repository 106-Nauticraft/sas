using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using sas.Scenario;

namespace sas.Simulators;

public abstract class Simulator<T> : ISimulateBehaviour
    where T : class
{
    protected T Instance { get; } = Substitute.For<T>();
    
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
        
    public T Received(int i)
    {
        return Instance.Received(i);
    }
        
    public T Received()
    {
        return Instance.Received();
    }
    
    public T DidNotReceive()
    {
        return Instance.DidNotReceive();
    }
}