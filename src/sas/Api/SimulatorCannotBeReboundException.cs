using sas.Simulators;

namespace sas.Api;

/// <summary>
/// Raised when a host is asked to bind another scenario while one of its simulators cannot follow.
/// </summary>
public class SimulatorCannotBeReboundException(IEnumerable<ISimulateBehaviour> simulators)
    : InvalidOperationException(BuildMessage(simulators))
{
    private static string BuildMessage(IEnumerable<ISimulateBehaviour> simulators)
    {
        var names = string.Join(", ", simulators.Select(simulator => simulator.GetType().Name));

        return $"{names} cannot be bound to another scenario because it does not implement {nameof(IBindScenario)}. " +
               $"A host holding it can only serve the scenario it was built with. " +
               $"Implement {nameof(IBindScenario)} on it - with an empty {nameof(IBindScenario.Bind)} if it reads nothing from the scenario - or build one host per scenario.";
    }
}
