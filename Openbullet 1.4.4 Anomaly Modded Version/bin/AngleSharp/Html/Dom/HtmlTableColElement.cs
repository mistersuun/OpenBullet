// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTableColElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTableColElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Col, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlTableColumnElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  public HorizontalAlignment Align
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Align).ToEnum<HorizontalAlignment>(HorizontalAlignment.Center);
    }
    set => this.SetOwnAttribute(AttributeNames.Align, value.ToString());
  }

  public int Span
  {
    get => this.GetOwnAttribute(AttributeNames.Span).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.Span, value.ToString());
  }

  public VerticalAlignment VAlign
  {
    get
    {
      return this.GetOwnAttribute(AttributeNames.Valign).ToEnum<VerticalAlignment>(VerticalAlignment.Middle);
    }
    set => this.SetOwnAttribute(AttributeNames.Valign, value.ToString());
  }

  public string Width
  {
    get => this.GetOwnAttribute(AttributeNames.Width);
    set => this.SetOwnAttribute(AttributeNames.Width, value);
  }
}
