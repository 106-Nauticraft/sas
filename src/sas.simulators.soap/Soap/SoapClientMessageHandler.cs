using NSubstitute;
using sas.simulators.http.Http;

namespace sas.simulators.soap.Soap;

public class SoapClientMessageHandler
{
    private readonly IDeferHttpRequestHandling _httpClient;

    internal SoapClientMessageHandler(
        IDeferHttpRequestHandling httpClient)
    {
        _httpClient = httpClient;
    }

    public SoapClientMessageHandler HandleRequest<TRequest, TResponse>(Func<TRequest, TResponse> handle)
        where TRequest: class
    {
        _httpClient.Post("", 
                Arg.Is<HttpContent>(content => content.IsSoapRequest<TRequest>()))
            .Returns(info =>
            {
                var content = info.Arg<HttpContent>();
                return OnSoapRequest(content, handle);
            });

        return this;
    }
    
    private static async Task<HttpResponseMessage?> OnSoapRequest<TRequest, TResponse>(HttpContent content, Func<TRequest, TResponse> handle) 
    {
        var soapVersion = await content.GetSoapMessageVersion();

        var request = await content.
            ReadFromXmlSoapBodyAsync<TRequest>(soapVersion);
               
        var response = handle(request!);

        var httpContent = new SoapXmlContent(response, soapVersion);
        return new HttpResponseMessage
        {
            Content = httpContent,
        };
    }
}