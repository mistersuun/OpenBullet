// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XmlElementWrapper
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

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
