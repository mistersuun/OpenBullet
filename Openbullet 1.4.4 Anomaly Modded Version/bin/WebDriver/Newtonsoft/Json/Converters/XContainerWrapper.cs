// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XContainerWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XContainerWrapper(XContainer container) : XObjectWrapper((XObject) container)
{
  private List<IXmlNode> _childNodes;

  private XContainer Container => (XContainer) this.WrappedNode;

  public override List<IXmlNode> ChildNodes
  {
    get
    {
      if (this._childNodes == null)
      {
        if (!this.HasChildNodes)
        {
          this._childNodes = XmlNodeConverter.EmptyChildNodes;
        }
        else
        {
          this._childNodes = new List<IXmlNode>();
          foreach (XObject node in this.Container.Nodes())
            this._childNodes.Add(XContainerWrapper.WrapNode(node));
        }
      }
      return this._childNodes;
    }
  }

  protected virtual bool HasChildNodes => this.Container.LastNode != null;

  public override IXmlNode ParentNode
  {
    get
    {
      return this.Container.Parent == null ? (IXmlNode) null : XContainerWrapper.WrapNode((XObject) this.Container.Parent);
    }
  }

  internal static IXmlNode WrapNode(XObject node)
  {
    switch (node)
    {
      case XDocument document:
        return (IXmlNode) new XDocumentWrapper(document);
      case XElement element:
        return (IXmlNode) new XElementWrapper(element);
      case XContainer container:
        return (IXmlNode) new XContainerWrapper(container);
      case XProcessingInstruction processingInstruction:
        return (IXmlNode) new XProcessingInstructionWrapper(processingInstruction);
      case XText text1:
        return (IXmlNode) new XTextWrapper(text1);
      case XComment text2:
        return (IXmlNode) new XCommentWrapper(text2);
      case XAttribute attribute:
        return (IXmlNode) new XAttributeWrapper(attribute);
      case XDocumentType documentType:
        return (IXmlNode) new XDocumentTypeWrapper(documentType);
      default:
        return (IXmlNode) new XObjectWrapper(node);
    }
  }

  public override IXmlNode AppendChild(IXmlNode newChild)
  {
    this.Container.Add(newChild.WrappedNode);
    this._childNodes = (List<IXmlNode>) null;
    return newChild;
  }
}
