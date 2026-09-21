using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NFluent;
using NSubstitute;
using sas.Scenario;
using sas.simulators.http.Http;
using sas.simulators.http.nsubstitute;

namespace sas.simulators.http.tests;

public class BaseHttpClientSimulatorShould
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

        Check.ThatCode(() => serviceProvider.GetRequiredService<TestHttpClient>().Get("first-scenario"))
            .Throws<HttpMessageInterceptionHandler.HttpRequestNotStubbedException>();
    }

    [Fact]
    public async Task Forget_the_requests_recorded_before_it_was_bound_again()
    {
        var simulator = new TestHttpClientSimulator();
        var services = new ServiceCollection();

        simulator.RegisterTo(services, new TestScenario("first-scenario"));

        var serviceProvider = services.BuildServiceProvider();
        await serviceProvider.GetRequiredService<TestHttpClient>().Get("first-scenario");

        simulator.Bind(new TestScenario("second-scenario"));
        await serviceProvider.GetRequiredService<TestHttpClient>().Get("second-scenario");

        simulator.CheckSpyRecorded(1);
    }

    private class TestScenario(string stubbedPath) : BaseScenario
    {
        public string StubbedPath { get; } = stubbedPath;
    }

    private class TestHttpClient(HttpClient httpClient)
    {
        public Task<HttpResponseMessage> Get(string uri) => httpClient.GetAsync(uri);
    }

    private class TestHttpClientSimulator : BaseHttpClientSimulator<TestHttpClient>
    {
        protected override void Simulate(BaseScenario scenario)
        {
            if (scenario is not TestScenario testScenario)
            {
                return;
            }

            HttpClient.Get(testScenario.StubbedPath)
                .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
        }

        public void CheckSpyRecorded(int expectedRequests) => Spy.HasRecordedRequests(expectedRequests);
    }
}