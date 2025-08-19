// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XmlElementWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Xml;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XmlElementWrapper : XmlNodeWrapper, IXmlElement, IXmlNode
{
  private readonly XmlElement _element;

  public XmlElementWrapper(XmlElement element)
    : base((XmlNode) element)
  {
    this._element = element;
  }

  public void SetAttributeNode(IXmlNode attribute)
  {
    this._element.SetAttributeNode((XmlAttribute) ((XmlNodeWrapper) attribute).WrappedNode);
  }

  public string GetPrefixOfNamespace(string namespaceUri)
  {
    return this._element.GetPrefixOfNamespace(namespaceUri);
  }

  public bool IsEmpty => this._element.IsEmpty;
}
