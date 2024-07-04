using System.Net.Mime;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static class HttpContextExtensions
{
    public static string[] GetSoapRequestsTypes(this HttpContent content)
    {
        if (!content.IsSoapRequest())
        {
            return Array.Empty<string>();
        }
        
        using var memoryStream = new MemoryStream();
        content.CopyToAsync(memoryStream).Wait(); 
        memoryStream.Position = 0;
        
        var document = XDocument.Load(memoryStream);

        var requestTypes = document.Descendants()
            .Where(x => x.Parent != null && x.Parent.Name.LocalName == "Body")
            .Select(x => x.Name.LocalName)
            .ToArray();

        return requestTypes;
    } 
    
    public static bool IsSoapRequest(this HttpContent content)
    {
        if (content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Xml
            && content.Headers.ContentType?.MediaType != MediaTypeNames.Text.Xml)
        {
            return false;
        }

        return true;
    }
    
    public static bool IsSoapRequest(this HttpContent content, Type requestType)
    {
        if (!content.IsSoapRequest())
        {
            return false;
        }
        
        var name = requestType.GetDataContractOrXmlTypeName();

        using var memoryStream = new MemoryStream();
        content.CopyToAsync(memoryStream).Wait(); 
        memoryStream.Position = 0;
        
        var document = XDocument.Load(memoryStream);
        
        var soapRequestXElement = document.XPathSelectElement($"//*[local-name()='{name}']");
        
        return soapRequestXElement is not null;
    }
    
    public static async Task<T?> ReadFromXmlSoapBodyAsync<T>(this HttpContent httpContent, MessageVersion soapVersion)
    {
        var stream = await httpContent.ReadAsStreamAsync();
        
        var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
        var receivedMessage = Message.CreateMessage(reader, int.MaxValue, soapVersion);

        return await receivedMessage.GetMessageBody<T>();
    }

    private static async Task<T?> GetMessageBody<T>(this Message receivedMessage)
    {
        using var ms = new MemoryStream();
        await using var dictionaryWriter = XmlDictionaryWriter.CreateTextWriter(ms);
        {
            receivedMessage.WriteBodyContents(dictionaryWriter);
            await dictionaryWriter.FlushAsync();
        }
        ms.Position = 0;

        using var bodyStreamReader = new StreamReader(ms);
        var bodyContent = await bodyStreamReader.ReadToEndAsync();

        var type = typeof(T);
        var defaultNamespace = type.GetDataContractOrXmlTypeNamespace();
        var xmlSerializer = new XmlSerializer(type, defaultNamespace);
        using var sr = new StringReader(bodyContent);
        return (T?) xmlSerializer.Deserialize(sr);
    }


    public static async Task<MessageVersion> GetSoapMessageVersion(this HttpContent httpContent)
    {
        var xmlDocument = await httpContent.ReadAsXmlDocument();
        
        var namespaces = new XmlNamespaceManager(xmlDocument.NameTable);
        namespaces.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        namespaces.AddNamespace("soap12", "http://www.w3.org/2003/05/soap-envelope");
        
        if (xmlDocument.SelectSingleNode("//soap:Envelope", namespaces) != null)
        {
            return MessageVersion.Soap11; // SOAP 1.1
        }

        if (xmlDocument.SelectSingleNode("//soap12:Envelope", namespaces) != null)
        {
            return MessageVersion.Soap12WSAddressing10; // SOAP 1.2
        }

        return MessageVersion.None;
    }

    private static async Task<XmlDocument> ReadAsXmlDocument(this HttpContent httpContent)
    {
        var xmlAsString = await httpContent.ReadAsStringAsync();
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlAsString);
        return xmlDocument;
    }
}