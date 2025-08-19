// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XAttributeWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XAttributeWrapper(XAttribute attribute) : XObjectWrapper((XObject) attribute)
{
  private XAttribute Attribute => (XAttribute) this.WrappedNode;

  public override string Value
  {
    get => this.Attribute.Value;
    set => this.Attribute.Value = value;
  }

  public override string LocalName => this.Attribute.Name.LocalName;

  public override string NamespaceUri => this.Attribute.Name.NamespaceName;

  public override IXmlNode ParentNode
  {
    get
    {
      return this.Attribute.Parent == null ? (IXmlNode) null : XContainerWrapper.WrapNode((XObject) this.Attribute.Parent);
    }
  }
}
