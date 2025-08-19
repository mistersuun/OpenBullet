// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XObjectWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XObjectWrapper : IXmlNode
{
  private readonly XObject _xmlObject;

  public XObjectWrapper(XObject xmlObject) => this._xmlObject = xmlObject;

  public object WrappedNode => (object) this._xmlObject;

  public virtual XmlNodeType NodeType => this._xmlObject.NodeType;

  public virtual string LocalName => (string) null;

  public virtual List<IXmlNode> ChildNodes => XmlNodeConverter.EmptyChildNodes;

  public virtual List<IXmlNode> Attributes => XmlNodeConverter.EmptyChildNodes;

  public virtual IXmlNode ParentNode => (IXmlNode) null;

  public virtual string Value
  {
    get => (string) null;
    set => throw new InvalidOperationException();
  }

  public virtual IXmlNode AppendChild(IXmlNode newChild) => throw new InvalidOperationException();

  public virtual string NamespaceUri => (string) null;
}
