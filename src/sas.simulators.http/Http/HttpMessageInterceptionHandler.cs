

namespace sas.simulators.http.Http;

public class HttpMessageInterceptionHandler : HttpMessageHandler
{
    private readonly IDeferHttpRequestHandling _deferredHttpClient;
    private readonly Uri _baseUri;
    private readonly Func<HttpRequestMessage, string> _buildUnexpectedRequestErrorMessage;

    public HttpMessageInterceptionHandler(IDeferHttpRequestHandling deferredHttpClient, Uri baseUri, Func<HttpRequestMessage, string> buildUnexpectedRequestErrorMessage)
    {
        _deferredHttpClient = deferredHttpClient;
        _baseUri = baseUri;
        _buildUnexpectedRequestErrorMessage = buildUnexpectedRequestErrorMessage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // MakeRelativeUrl replaces spaces by %20. To take into account different way of stubbing with or without %20 we try both  
        var relativeUri = _baseUri.MakeRelativeUri(request.RequestUri!).ToString();

        var cloned = await request.Clone();

        if (request.Method == HttpMethod.Get)
        {
            return await GetWithOrWithoutCancellationToken(relativeUri, cancellationToken) 
                   ?? await GetWithOrWithoutCancellationToken(Uri.UnescapeDataString(relativeUri), cancellationToken)
                   ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
        }

        if (request.Method == HttpMethod.Post)
        {
            return await PostWithOrWithoutCancellationToken(relativeUri, cloned.Content, cancellationToken)
                   ?? await PostWithOrWithoutCancellationToken(Uri.UnescapeDataString(relativeUri), cloned.Content, cancellationToken)
                   ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
        }

        if (request.Method == HttpMethod.Put)
        {
            return await PutWithOrWithoutCancellationToken(relativeUri, cloned.Content, cancellationToken) 
                   ?? await PutWithOrWithoutCancellationToken(Uri.UnescapeDataString(relativeUri), cloned.Content, cancellationToken)
                   ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
        }

        if (request.Method == HttpMethod.Patch)
        {
            return await PatchWithOrWithoutCancellationToken(relativeUri, cloned.Content, cancellationToken) 
                   ?? await PatchWithOrWithoutCancellationToken(Uri.UnescapeDataString(relativeUri), cloned.Content, cancellationToken)
                   ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
        }

        if (request.Method == HttpMethod.Delete)
        {
            if(cloned.Content != null)
            {
                return await DeleteWithOrWithoutCancellationToken(relativeUri, cloned.Content, cancellationToken) 
                    ?? await DeleteWithOrWithoutCancellationToken(Uri.UnescapeDataString(relativeUri), cloned.Content, cancellationToken)
                    ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
            }

            return await DeleteWithOrWithoutCancellationToken(relativeUri, cancellationToken)
                   ?? await DeleteWithOrWithoutCancellationToken(Uri.UnescapeDataString(relativeUri), cancellationToken)
                   ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
        }

        return  await _deferredHttpClient.Send(cloned, cancellationToken) 
                ?? await _deferredHttpClient.Send(cloned)
                ?? throw new HttpRequestNotStubbedException(request, _buildUnexpectedRequestErrorMessage);
    }

    private async Task<HttpResponseMessage?> DeleteWithOrWithoutCancellationToken(string relativeUri, CancellationToken cancellationToken)
    {
        return await _deferredHttpClient.Delete(relativeUri, cancellationToken)
               ?? await _deferredHttpClient.Delete(relativeUri);
    }

    private async Task<HttpResponseMessage?> DeleteWithOrWithoutCancellationToken(string relativeUri, HttpContent content, CancellationToken cancellationToken)
    {
        return await _deferredHttpClient.Delete(relativeUri, content, cancellationToken) 
               ?? await _deferredHttpClient.Delete(relativeUri, content);
    }

    private async Task<HttpResponseMessage?> PatchWithOrWithoutCancellationToken(string relativeUri, HttpContent? content,
        CancellationToken cancellationToken)
    {
        return await _deferredHttpClient.Patch(relativeUri, content, cancellationToken) 
               ?? await _deferredHttpClient.Patch(relativeUri, content);
    }

    private async Task<HttpResponseMessage?> PutWithOrWithoutCancellationToken(string relativeUri, HttpContent? content,
        CancellationToken cancellationToken)
    {
        return await _deferredHttpClient.Put(relativeUri, content, cancellationToken) 
               ?? await _deferredHttpClient.Put(relativeUri, content);
    }

    private async Task<HttpResponseMessage?> PostWithOrWithoutCancellationToken(string relativeUri, HttpContent? requestContent,
        CancellationToken cancellationToken)
    {
        return await _deferredHttpClient.Post(relativeUri, requestContent, cancellationToken) 
               ?? await _deferredHttpClient.Post(relativeUri, requestContent);
    }

    private async Task<HttpResponseMessage?> GetWithOrWithoutCancellationToken(string relativeUri, CancellationToken cancellationToken)
    {
        return await _deferredHttpClient.Get(relativeUri, cancellationToken) 
               ?? await _deferredHttpClient.Get(relativeUri);
    }


    public class HttpRequestNotStubbedException : Exception
    {
        public HttpRequestNotStubbedException(HttpRequestMessage request, Func<HttpRequestMessage,string> buildMessage) : base(buildMessage(request)) { }
    }
    
}