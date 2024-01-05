using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace sas.simulators.soap.Soap;

public class SoapXmlContent : HttpContent
{
    private readonly object? _content;
    private readonly MessageVersion _soapVersion;

    public SoapXmlContent(object? content, MessageVersion soapVersion)
    {
        _content = content;
        _soapVersion = soapVersion;
        Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Xml)
        {
            CharSet = "utf-8"
        };
    }
    
    private static XmlElement? SerializeToXmlElement(object? obj)
    {
        if (obj is null)
        {
            return null;
        }
        var xmlDoc = new XmlDocument();
        var type = obj.GetType();
        var defaultNamespace = type.GetDataContractOrXmlTypeNamespace();

        var xmlSerializer = new XmlSerializer(type, defaultNamespace);
        
        using var xmlStream = new MemoryStream();
        xmlSerializer.Serialize(xmlStream, obj);
        xmlStream.Position = 0;
        xmlDoc.Load(xmlStream);
        return xmlDoc.DocumentElement!;
    }
    
    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var contentAsXml = SerializeToXmlElement(_content);
        
        var message = Message.CreateMessage(_soapVersion, null, contentAsXml);
        
        await using var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = Encoding.UTF8, Async = true });
        
        message.WriteMessage(writer);
        await writer.FlushAsync();
    }

    protected override bool TryComputeLength(out long length)
    {
        length = 0;
        return false;
    }
}