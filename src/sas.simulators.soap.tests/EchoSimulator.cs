using sas.Scenario;

namespace sas.simulators.soap.tests;

public class EchoSimulator : BaseSoapClientSimulator<IEchoSoapService, EchoSoapClient>
{
    protected override void Simulate(BaseScenario scenario)
    {
        if (scenario is not EchoScenario echoScenario)
        {
            return;
        }

        if (echoScenario.EchoGreeting is not null)
        {
            SoapClient.HandleSoapRequest<EchoRequest, EchoResponse>(
                request => new EchoResponse { EchoResult = $"{echoScenario.EchoGreeting} {request.Message}" });
        }

        if (echoScenario.ShoutGreeting is not null)
        {
            SoapClient.HandleSoapRequest<ShoutRequest, ShoutResponse>(
                request => new ShoutResponse { ShoutResult = $"{echoScenario.ShoutGreeting} {request.Message.ToUpperInvariant()}" });
        }
    }

    public void CheckSoapRequestsRecorded(int expectedRequests) => SoapRequestSpy.ASoapRequest().Occurred(expectedRequests);
}