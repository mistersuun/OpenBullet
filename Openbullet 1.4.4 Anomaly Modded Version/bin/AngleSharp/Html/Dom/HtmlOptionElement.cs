// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlOptionElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlOptionElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Option, prefix, NodeFlags.ImplicitelyClosed | NodeFlags.ImpliedEnd | NodeFlags.HtmlSelectScoped),
  IHtmlOptionElement,
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
  private bool? _selected;

  public bool IsDisabled
  {
    get => this.GetBoolAttribute(AttributeNames.Disabled);
    set => this.SetBoolAttribute(AttributeNames.Disabled, value);
  }

  public IHtmlFormElement Form => this.GetAssignedForm();

  public string Label
  {
    get => this.GetOwnAttribute(AttributeNames.Label) ?? this.Text;
    set => this.SetOwnAttribute(AttributeNames.Label, value);
  }

  public string Value
  {
    get => this.GetOwnAttribute(AttributeNames.Value) ?? this.Text;
    set => this.SetOwnAttribute(AttributeNames.Value, value);
  }

  public int Index
  {
    get
    {
      if (this.Parent is HtmlOptionsGroupElement parent)
      {
        int index = 0;
        foreach (INode childNode in parent.ChildNodes)
        {
          if (childNode == this)
            return index;
          ++index;
        }
      }
      return 0;
    }
  }

  public string Text
  {
    get => this.TextContent.CollapseAndStrip();
    set => this.TextContent = value;
  }

  public bool IsDefaultSelected
  {
    get => this.GetBoolAttribute(AttributeNames.Selected);
    set => this.SetBoolAttribute(AttributeNames.Selected, value);
  }

  public bool IsSelected
  {
    get => !this._selected.HasValue ? this.IsDefaultSelected : this._selected.Value;
    set => this._selected = new bool?(value);
  }
}
