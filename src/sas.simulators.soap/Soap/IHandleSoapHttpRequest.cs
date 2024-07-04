namespace sas.simulators.soap.Soap;

public interface IHandleSoapHttpRequest
{
    IHandleSoapHttpRequest HandleSoapRequest<TSoapRequest, TSoapResponse>(Func<TSoapRequest, TSoapResponse> handle)
        where TSoapRequest : class;
}