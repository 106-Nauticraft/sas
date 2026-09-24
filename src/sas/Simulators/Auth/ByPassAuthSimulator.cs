using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using sas.Scenario;

namespace sas.Simulators.Auth;

public class ByPassAuthSimulator : ISimulateBehaviour, IBindScenario
{
    public void RegisterTo(IServiceCollection services, BaseScenario scenario)
    {
        services.AddMvc(options => options.Filters.Add(new AllowAnonymousFilter()));
    }

    public void Bind(BaseScenario scenario) { }
}