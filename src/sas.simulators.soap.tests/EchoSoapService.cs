using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

namespace sas.simulators.soap.tests;

public static class EchoSoapService
{
    public const string Namespace = "http://sas.tests/echo";
}

[ServiceContract(Namespace = EchoSoapService.Namespace)]
public interface IEchoSoapService
{
    [OperationContract]
    string Echo(string message);

    [OperationContract]
    string Shout(string message);
}

public class EchoSoapClient(Binding binding, EndpointAddress address)
    : ClientBase<IEchoSoapService>(binding, address), IEchoSoapService
{
    public string Echo(string message) => Channel.Echo(message);

    public string Shout(string message) => Channel.Shout(message);
}

[XmlType(TypeName = "Echo", Namespace = EchoSoapService.Namespace)]
public class EchoRequest
{
    [XmlElement("message", Namespace = EchoSoapService.Namespace)]
    public string Message { get; set; } = "";
}

[XmlType(TypeName = "EchoResponse", Namespace = EchoSoapService.Namespace)]
public class EchoResponse
{
    [XmlElement("EchoResult", Namespace = EchoSoapService.Namespace)]
    public string EchoResult { get; set; } = "";
}

[XmlType(TypeName = "Shout", Namespace = EchoSoapService.Namespace)]
public class ShoutRequest
{
    [XmlElement("message", Namespace = EchoSoapService.Namespace)]
    public string Message { get; set; } = "";
}

[XmlType(TypeName = "ShoutResponse", Namespace = EchoSoapService.Namespace)]
public class ShoutResponse
{
    [XmlElement("ShoutResult", Namespace = EchoSoapService.Namespace)]
    public string ShoutResult { get; set; } = "";
}