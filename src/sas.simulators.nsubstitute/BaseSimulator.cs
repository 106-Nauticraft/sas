using NSubstitute;
using sas.Simulators;

namespace sas.simulators.nsubstitute;

public class BaseSimulator<T> : AbstractSimulator<T>
    where T : class
{
    protected override T Instance { get; } = Substitute.For<T>();
}