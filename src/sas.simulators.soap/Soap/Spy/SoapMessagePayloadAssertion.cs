using HttpRequestSpy.Assertions;
using HttpRequestSpy.Assertions.HttpRequestAssertions;

namespace sas.simulators.soap.Soap.Spy;

public class SoapMessagePayloadAssertion
{
    private static readonly PropertyXPath BodyXPath = "/Envelope/Body";
    private static readonly PropertyXPath BodyXPathWithNamespaces = PropertyXPath.WithNamespaces("/s:Envelope/s:Body");
    
    private static readonly PropertyXPath HeaderXPath = "/Envelope/Header";
    private static readonly PropertyXPath HeaderXPathWithNamespaces = PropertyXPath.WithNamespaces("/s:Envelope/s:Header");
    private HttpRequestWithPayloadAssertion _assertion;

    public SoapMessagePayloadAssertion(HttpRequestWithPayloadAssertion assertion)
    {
        _assertion = assertion;
    }

    public SoapMessagePayloadAssertion WithBody(object expectedBody)
    {
        _assertion = _assertion.WithXmlPayloadProperty(BodyXPathWithNamespaces, expectedBody);
        return this;
    }
    
    public SoapMessagePayloadAssertion WithBodyPart(PropertyXPath xPath,object expectedBodyPart)
    {
        if (xPath.CheckNamespaces && !xPath.StartsWith(BodyXPathWithNamespaces))
        {
            xPath = BodyXPathWithNamespaces + xPath;
        }
        
        if (!xPath.CheckNamespaces && !xPath.StartsWith(BodyXPath))
        {
            xPath = BodyXPath + xPath;
        }
        
        _assertion = _assertion.WithXmlPayloadProperty(xPath, expectedBodyPart);
        return this;
    }
    
    public SoapMessagePayloadAssertion WithHeader(object expectedHeader)
    {
        _assertion = _assertion.WithXmlPayloadProperty(HeaderXPathWithNamespaces, expectedHeader);
        return this;
    }
    
    public SoapMessagePayloadAssertion WithHeaderPart(PropertyXPath xPath,object expectedHeaderPart)
    {
        if (xPath.CheckNamespaces && !xPath.StartsWith(HeaderXPathWithNamespaces))
        {
            xPath = HeaderXPathWithNamespaces + xPath;
        }
        
        if (!xPath.CheckNamespaces && !xPath.StartsWith(HeaderXPath))
        {
            xPath = HeaderXPath + xPath;
        }
        
        _assertion = _assertion.WithXmlPayloadProperty(xPath, expectedHeaderPart);
        return this;
    }

    public void Occurred(int n)
    {
        _assertion.Occurred(n);
    }
    
    public void OccurredOnce()
    {
        Occurred(1);
    }
    
    public void OccurredTwice()
    {
        Occurred(2);
    }
    
    public void NeverOccurred()
    {
        _assertion.NeverOccurred();
    }
}