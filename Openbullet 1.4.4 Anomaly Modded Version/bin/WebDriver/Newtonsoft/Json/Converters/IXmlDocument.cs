// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.IXmlDocument
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Converters;

internal interface IXmlDocument : IXmlNode
{
  IXmlNode CreateComment(string text);

  IXmlNode CreateTextNode(string text);

  IXmlNode CreateCDataSection(string data);

  IXmlNode CreateWhitespace(string text);

  IXmlNode CreateSignificantWhitespace(string text);

  IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone);

  IXmlNode CreateXmlDocumentType(
    string name,
    string publicId,
    string systemId,
    string internalSubset);

  IXmlNode CreateProcessingInstruction(string target, string data);

  IXmlElement CreateElement(string elementName);

  IXmlElement CreateElement(string qualifiedName, string namespaceUri);

  IXmlNode CreateAttribute(string name, string value);

  IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value);

  IXmlElement DocumentElement { get; }
}
