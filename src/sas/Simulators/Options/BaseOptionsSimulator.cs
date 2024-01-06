using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;

namespace sas.Simulators.Options;

public abstract class BaseOptionsSimulator<TOptions> : ISimulateBehaviour
    where TOptions : class
{
    protected abstract Action<TOptions> Configure(BaseScenario scenario);

    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        if (scenario is NoScenario)
        {
            return;
        }

        services.Configure(Configure(scenario));
    }
}