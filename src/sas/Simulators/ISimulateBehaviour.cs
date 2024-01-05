using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;

namespace sas.Simulators;

public interface ISimulateBehaviour
{
    void RegisterTo(IServiceCollection services, BaseScenario scenario);
}