using sas.Scenario;

namespace sas.simulators.soap.tests;

public class EchoScenario : BaseScenario
{
    private EchoScenario(string? echoGreeting, string? shoutGreeting)
    {
        EchoGreeting = echoGreeting;
        ShoutGreeting = shoutGreeting;
    }

    public string? EchoGreeting { get; }
    public string? ShoutGreeting { get; }

    public static EchoScenario Echoing(string greeting) => new(greeting, null);

    public static EchoScenario Shouting(string greeting) => new(null, greeting);
}