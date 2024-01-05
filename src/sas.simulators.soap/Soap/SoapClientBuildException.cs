
namespace sas.simulators.soap.Soap;

public class SoapClientBuildException : Exception
{
    public SoapClientBuildException(Type type) : base($"Cannot create a client of type ${type}")
    {
        
    }
}