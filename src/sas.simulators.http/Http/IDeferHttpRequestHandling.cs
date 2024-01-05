
namespace sas.simulators.http.Http;

public interface IDeferHttpRequestHandling
{
    Task<HttpResponseMessage?> Get(string uri);
    Task<HttpResponseMessage?> Get(string uri, CancellationToken cancellationToken);
        
    Task<HttpResponseMessage?> Post(string uri, HttpContent? content);
    Task<HttpResponseMessage?> Post(string uri, HttpContent? content, CancellationToken cancellationToken);
        
        
    Task<HttpResponseMessage?> Put(string uri, HttpContent? content);
    Task<HttpResponseMessage?> Put(string uri, HttpContent? content, CancellationToken cancellationToken);
        
        
    Task<HttpResponseMessage?> Patch(string uri, HttpContent? content);
    Task<HttpResponseMessage?> Patch(string uri, HttpContent? content, CancellationToken cancellationToken);


    Task<HttpResponseMessage?> Delete(string uri);
    Task<HttpResponseMessage?> Delete(string uri, CancellationToken cancellationToken);

    // These nest 2 methods were added because SynXis exposes some APIs that require a payload on DELETE methods...
    Task<HttpResponseMessage?> Delete(string uri, HttpContent content);
    Task<HttpResponseMessage?> Delete(string uri, HttpContent content, CancellationToken cancellationToken);

        
    Task<HttpResponseMessage?> Send(HttpRequestMessage request);
    Task<HttpResponseMessage?> Send(HttpRequestMessage request, CancellationToken cancellationToken);
}