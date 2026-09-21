using sas.Scenario;

namespace sas.Simulators;

/// <summary>
/// Opt-in contract for a simulator that can be re-bound to another scenario after the host was built.
/// A family implements it once it has a correct reset; until then it stays visibly unbindable.
/// </summary>
public interface IBindScenario
{
    void Bind(BaseScenario scenario);
}