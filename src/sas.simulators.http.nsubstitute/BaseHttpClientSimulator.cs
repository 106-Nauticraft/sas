using sas.simulators.http.Http;

namespace sas.simulators.http.nsubstitute;

public abstract class BaseHttpClientSimulator<THttpClient> : AbstractHttpClientSimulator<THttpClient>
    where THttpClient : class
{
    protected override IDeferHttpRequestHandling HttpClient { get; } =
        NSubstitute.Substitute.For<IDeferHttpRequestHandling>();
}