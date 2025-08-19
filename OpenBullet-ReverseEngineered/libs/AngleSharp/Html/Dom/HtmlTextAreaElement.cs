// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlTextAreaElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlTextAreaElement(Document owner, string prefix = null) : 
  HtmlTextFormControlElement(owner, TagNames.Textarea, prefix, NodeFlags.LineTolerance),
  IHtmlTextAreaElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IValidation
{
  public string Wrap
  {
    get => this.GetOwnAttribute(AttributeNames.Wrap);
    set => this.SetOwnAttribute(AttributeNames.Wrap, value);
  }

  public override string DefaultValue
  {
    get => this.TextContent;
    set => this.TextContent = value;
  }

  public int TextLength => this.Value.Length;

  public int Rows
  {
    get => this.GetOwnAttribute(AttributeNames.Rows).ToInteger(2);
    set => this.SetOwnAttribute(AttributeNames.Rows, value.ToString());
  }

  public int Columns
  {
    get => this.GetOwnAttribute(AttributeNames.Cols).ToInteger(20);
    set => this.SetOwnAttribute(AttributeNames.Cols, value.ToString());
  }

  public string Type => TagNames.Textarea;

  internal bool IsMutable => !this.IsDisabled && !this.IsReadOnly;

  internal override void ConstructDataSet(FormDataSet dataSet, IHtmlElement submitter)
  {
    this.ConstructDataSet(dataSet, this.Type);
  }

  internal override FormControlState SaveControlState()
  {
    return new FormControlState(this.Name, this.Type, this.Value);
  }

  internal override void RestoreFormControlState(FormControlState state)
  {
    if (!state.Type.Is(this.Type) || !state.Name.Is(this.Name))
      return;
    this.Value = state.Value;
  }
}
