// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.XCommentWrapper
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Xml.Linq;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class XCommentWrapper(XComment text) : XObjectWrapper((XObject) text)
{
  private XComment Text => (XComment) this.WrappedNode;

  public override string Value
  {
    get => this.Text.Value;
    set => this.Text.Value = value;
  }

  public override IXmlNode ParentNode
  {
    get
    {
      return this.Text.Parent == null ? (IXmlNode) null : XContainerWrapper.WrapNode((XObject) this.Text.Parent);
    }
  }
}
