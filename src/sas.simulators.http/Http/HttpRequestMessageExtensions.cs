// ReSharper disable once CheckNamespace
namespace System.Net.Http;

internal static class HttpRequestMessageExtensions
{
    public static async Task<HttpRequestMessage> Clone(this HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        // Copy the request's content (via a MemoryStream) into the cloned object
        var ms = new MemoryStream();
        if (request.Content != null)
        {
            await request.Content.CopyToAsync(ms, null, CancellationToken.None);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            // Copy the content headers
            foreach (var h in request.Content.Headers)
                clone.Content.Headers.Add(h.Key, h.Value);
        }

        clone.Version = request.Version;

        foreach (var (key, value) in request.Options)
            clone.Options.Set(new HttpRequestOptionsKey<object?>(key), value);
    
        foreach (var (key, value) in request.Headers)
            clone.Headers.TryAddWithoutValidation(key, value);

        return clone;
    }
}