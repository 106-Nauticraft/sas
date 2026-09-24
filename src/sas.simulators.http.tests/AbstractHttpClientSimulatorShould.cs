using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NFluent;
using NSubstitute;
using NSubstitute.ClearExtensions;
using sas.Scenario;
using sas.simulators.http.Http;

namespace sas.simulators.http.tests;

public class AbstractHttpClientSimulatorShould
{
    [Fact]
    public async Task Simulate_the_new_scenario_and_forget_the_previous_one_when_bound_again()
    {
        var simulator = new TestHttpClientSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new TestScenario("first-scenario"));

        var serviceProvider = services.BuildServiceProvider();

        var firstResponse = await serviceProvider.GetRequiredService<TestHttpClient>().Get("first-scenario");
        Check.That(firstResponse.StatusCode).IsEqualTo(HttpStatusCode.Accepted);

        simulator.Bind(new TestScenario("second-scenario"));

        var secondResponse = await serviceProvider.GetRequiredService<TestHttpClient>().Get("second-scenario");
        Check.That(secondResponse.StatusCode).IsEqualTo(HttpStatusCode.Accepted);

        simulator.CheckSpyRecorded(1);

        Check.ThatCode(() => serviceProvider.GetRequiredService<TestHttpClient>().Get("first-scenario"))
            .Throws<HttpMessageInterceptionHandler.HttpRequestNotStubbedException>();
    }

    private class TestScenario(string stubbedPath) : BaseScenario
    {
        public string StubbedPath { get; } = stubbedPath;
    }

    private class TestHttpClient(HttpClient httpClient)
    {
        public Task<HttpResponseMessage> Get(string uri) => httpClient.GetAsync(uri);
    }

    private class TestHttpClientSimulator : AbstractHttpClientSimulator<TestHttpClient>
    {
        protected override IDeferHttpRequestHandling HttpClient { get; } =
            Substitute.For<IDeferHttpRequestHandling>();

        protected override void Simulate(BaseScenario scenario)
        {
            if (scenario is not TestScenario testScenario)
            {
                return;
            }

            HttpClient.Get(testScenario.StubbedPath)
                .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
        }

        protected override void ResetHttpClient() => HttpClient.ClearSubstitute();

        public void CheckSpyRecorded(int expectedRequests) => Spy.HasRecordedRequests(expectedRequests);
    }
}
