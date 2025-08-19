// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlMenuItemElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlMenuItemElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.MenuItem, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlMenuItemElement,
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
  internal bool IsVisited { get; set; }

  internal bool IsActive { get; set; }

  public IHtmlElement Command
  {
    get
    {
      string ownAttribute = this.GetOwnAttribute(AttributeNames.Command);
      return !string.IsNullOrEmpty(ownAttribute) ? this.Owner?.GetElementById(ownAttribute) as IHtmlElement : (IHtmlElement) null;
    }
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public string Label
  {
    get => this.GetOwnAttribute(AttributeNames.Label);
    set => this.SetOwnAttribute(AttributeNames.Label, value);
  }

  public string Icon
  {
    get => this.GetOwnAttribute(AttributeNames.Icon);
    set => this.SetOwnAttribute(AttributeNames.Icon, value);
  }

  public bool IsDisabled
  {
    get => this.GetBoolAttribute(AttributeNames.Disabled);
    set => this.SetBoolAttribute(AttributeNames.Disabled, value);
  }

  public bool IsChecked
  {
    get => this.GetBoolAttribute(AttributeNames.Checked);
    set => this.SetBoolAttribute(AttributeNames.Checked, value);
  }

  public bool IsDefault
  {
    get => this.GetBoolAttribute(AttributeNames.Default);
    set => this.SetBoolAttribute(AttributeNames.Default, value);
  }

  public string RadioGroup
  {
    get => this.GetOwnAttribute(AttributeNames.Radiogroup);
    set => this.SetOwnAttribute(AttributeNames.Radiogroup, value);
  }
}
