// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlFrameElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlFrameElement(Document owner, string prefix = null) : HtmlFrameElementBase(owner, TagNames.Frame, prefix, NodeFlags.SelfClosing)
{
  public bool NoResize
  {
    get => this.GetOwnAttribute(AttributeNames.NoResize).ToBoolean();
    set => this.SetOwnAttribute(AttributeNames.NoResize, value.ToString());
  }
}
