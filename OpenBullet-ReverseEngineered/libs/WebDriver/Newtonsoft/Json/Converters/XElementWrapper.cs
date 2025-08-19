// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XElementWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XElementWrapper(XElement element) : XContainerWrapper((XContainer) element), IXmlElement, IXmlNode
{
  private List<IXmlNode> _attributes;

  private XElement Element => (XElement) this.WrappedNode;

  public void SetAttributeNode(IXmlNode attribute)
  {
    this.Element.Add(((XObjectWrapper) attribute).WrappedNode);
    this._attributes = (List<IXmlNode>) null;
  }

  public override List<IXmlNode> Attributes
  {
    get
    {
      if (this._attributes == null)
      {
        if (!this.Element.HasAttributes && !this.HasImplicitNamespaceAttribute(this.NamespaceUri))
        {
          this._attributes = XmlNodeConverter.EmptyChildNodes;
        }
        else
        {
          this._attributes = new List<IXmlNode>();
          foreach (XAttribute attribute in this.Element.Attributes())
            this._attributes.Add((IXmlNode) new XAttributeWrapper(attribute));
          string namespaceUri = this.NamespaceUri;
          if (this.HasImplicitNamespaceAttribute(namespaceUri))
            this._attributes.Insert(0, (IXmlNode) new XAttributeWrapper(new XAttribute((XName) "xmlns", (object) namespaceUri)));
        }
      }
      return this._attributes;
    }
  }

  private bool HasImplicitNamespaceAttribute(string namespaceUri)
  {
    if (!string.IsNullOrEmpty(namespaceUri) && namespaceUri != this.ParentNode?.NamespaceUri && string.IsNullOrEmpty(this.GetPrefixOfNamespace(namespaceUri)))
    {
      bool flag = false;
      if (this.Element.HasAttributes)
      {
        foreach (XAttribute attribute in this.Element.Attributes())
        {
          if (attribute.Name.LocalName == "xmlns" && string.IsNullOrEmpty(attribute.Name.NamespaceName) && attribute.Value == namespaceUri)
            flag = true;
        }
      }
      if (!flag)
        return true;
    }
    return false;
  }

  public override IXmlNode AppendChild(IXmlNode newChild)
  {
    IXmlNode xmlNode = base.AppendChild(newChild);
    this._attributes = (List<IXmlNode>) null;
    return xmlNode;
  }

  public override string Value
  {
    get => this.Element.Value;
    set => this.Element.Value = value;
  }

  public override string LocalName => this.Element.Name.LocalName;

  public override string NamespaceUri => this.Element.Name.NamespaceName;

  public string GetPrefixOfNamespace(string namespaceUri)
  {
    return this.Element.GetPrefixOfNamespace((XNamespace) namespaceUri);
  }

  public bool IsEmpty => this.Element.IsEmpty;
}
