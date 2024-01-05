namespace sas.Scenario;

public class BaseScenario
{
    public static readonly BaseScenario None = new NoScenario();
}


public sealed class NoScenario : BaseScenario
{
}