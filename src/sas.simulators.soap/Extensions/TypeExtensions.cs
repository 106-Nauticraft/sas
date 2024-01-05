#nullable enable
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace System;

public class XmlNamespaceNotFoundException : Exception
{
    public XmlNamespaceNotFoundException(Type type) : base($"Could not find any xml namespace for type {type}. It is missing a {nameof(DataContractAttribute)} or {nameof(XmlTypeAttribute)}")
    {
            
    }
}  


public static class TypeExtensions
{
    public static string GetDataContractOrXmlTypeNamespace(this Type type)
    {
        string? ns = null; 
        
        // Check for DataContract attribute first
        
        var dataContractAttr = type.GetCustomAttribute<DataContractAttribute>(false);
        if (dataContractAttr is not null)
        {
            ns = dataContractAttr.Namespace;
        }
        else
        {
            // Then check for XmlType attribute
            var xmlTypeAttr = type.GetCustomAttribute<XmlTypeAttribute>(false);
            if (xmlTypeAttr is not null)
            {
                ns = xmlTypeAttr.Namespace;
            }
        }

        return ns ?? type.Namespace ?? throw new XmlNamespaceNotFoundException(type);
    }
    
    public static string GetDataContractOrXmlTypeName(this Type type)
    {
        string? name = null; 
        
        // Check for DataContract attribute first
        
        var dataContractAttr = type.GetCustomAttribute<DataContractAttribute>(false);
        if (dataContractAttr is not null)
        {
            name = dataContractAttr.Name;
        }
        else
        {
            // Then check for XmlType attribute
            var xmlTypeAttr = type.GetCustomAttribute<XmlTypeAttribute>(false);
            if (xmlTypeAttr is not null)
            {
                name = xmlTypeAttr.TypeName;
            }
        }

        return string.IsNullOrEmpty(name) ? type.Name : name;
    }
}