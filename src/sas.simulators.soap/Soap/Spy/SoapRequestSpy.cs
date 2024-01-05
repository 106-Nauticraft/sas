
namespace sas.simulators.soap.Soap.Spy;

public class SoapRequestSpy
{
    private readonly Uri _uri;
    private readonly HttpRequestSpy.HttpRequestSpy _httpRequestSpy;

    public SoapRequestSpy(Uri uri, HttpRequestSpy.HttpRequestSpy httpRequestSpy)
    {
        _uri = uri;
        _httpRequestSpy = httpRequestSpy;
    }

    public SoapMessagePayloadAssertion ASoapRequest()
    {
        return new SoapMessagePayloadAssertion(_httpRequestSpy.APostRequestTo(_uri.ToString()));
    }
}