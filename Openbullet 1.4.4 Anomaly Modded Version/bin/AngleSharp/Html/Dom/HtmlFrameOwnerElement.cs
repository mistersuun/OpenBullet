// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlFrameOwnerElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal abstract class HtmlFrameOwnerElement : HtmlElement
{
  public HtmlFrameOwnerElement(Document owner, string name, string prefix, NodeFlags flags = NodeFlags.None)
    : base(owner, name, prefix, flags)
  {
  }

  public bool CanContainRangeEndpoint { get; private set; }

  public int DisplayWidth
  {
    get => this.GetOwnAttribute(AttributeNames.Width).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.Width, value.ToString());
  }

  public int DisplayHeight
  {
    get => this.GetOwnAttribute(AttributeNames.Height).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.Height, value.ToString());
  }

  public int MarginWidth
  {
    get => this.GetOwnAttribute(AttributeNames.MarginWidth).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.MarginWidth, value.ToString());
  }

  public int MarginHeight
  {
    get => this.GetOwnAttribute(AttributeNames.MarginHeight).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.MarginHeight, value.ToString());
  }
}
