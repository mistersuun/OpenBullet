// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlSelectElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlSelectElement(Document owner, string prefix = null) : 
  HtmlFormControlElementWithState(owner, TagNames.Select, prefix),
  IHtmlSelectElement,
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
  private OptionsCollection _options;
  private HtmlCollection<IHtmlOptionElement> _selected;

  public IHtmlOptionElement this[int index]
  {
    get => this.Options.GetOptionAt(index);
    set => this.Options.SetOptionAt(index, value);
  }

  public int Size
  {
    get => this.GetOwnAttribute(AttributeNames.Size).ToInteger(0);
    set => this.SetOwnAttribute(AttributeNames.Size, value.ToString());
  }

  public bool IsRequired
  {
    get => this.GetBoolAttribute(AttributeNames.Required);
    set => this.SetBoolAttribute(AttributeNames.Required, value);
  }

  public IHtmlCollection<IHtmlOptionElement> SelectedOptions
  {
    get
    {
      return (IHtmlCollection<IHtmlOptionElement>) this._selected ?? (IHtmlCollection<IHtmlOptionElement>) (this._selected = new HtmlCollection<IHtmlOptionElement>(this.Options.Where<IHtmlOptionElement>((Func<IHtmlOptionElement, bool>) (m => m.IsSelected))));
    }
  }

  public int SelectedIndex => this.Options.SelectedIndex;

  public string Value
  {
    get
    {
      foreach (IHtmlOptionElement option in (IEnumerable<IHtmlOptionElement>) this.Options)
      {
        if (option.IsSelected)
          return option.Value;
      }
      return (string) null;
    }
    set => this.UpdateValue(value);
  }

  public int Length => this.Options.Length;

  public bool IsMultiple
  {
    get => this.GetBoolAttribute(AttributeNames.Multiple);
    set => this.SetBoolAttribute(AttributeNames.Multiple, value);
  }

  public IHtmlOptionsCollection Options
  {
    get
    {
      return (IHtmlOptionsCollection) this._options ?? (IHtmlOptionsCollection) (this._options = new OptionsCollection((IElement) this));
    }
  }

  public string Type => !this.IsMultiple ? InputTypeNames.SelectOne : InputTypeNames.SelectMultiple;

  public void AddOption(IHtmlOptionElement element, IHtmlElement before = null)
  {
    this.Options.Add(element, before);
  }

  public void AddOption(IHtmlOptionsGroupElement element, IHtmlElement before = null)
  {
    this.Options.Add(element, before);
  }

  public void RemoveOptionAt(int index) => this.Options.Remove(index);

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

  internal override void ConstructDataSet(FormDataSet dataSet, IHtmlElement submitter)
  {
    IHtmlOptionsCollection options = this.Options;
    for (int index = 0; index < options.Length; ++index)
    {
      IHtmlOptionElement optionAt = options.GetOptionAt(index);
      if (optionAt.IsSelected && !optionAt.IsDisabled)
        dataSet.Append(this.Name, optionAt.Value, this.Type);
    }
  }

  internal override void SetupElement()
  {
    base.SetupElement();
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Value);
    if (ownAttribute == null)
      return;
    this.UpdateValue(ownAttribute);
  }

  internal override void Reset()
  {
    IHtmlOptionsCollection options = this.Options;
    int num = 0;
    int index1 = 0;
    for (int index2 = 0; index2 < options.Length; ++index2)
    {
      IHtmlOptionElement optionAt = options.GetOptionAt(index2);
      optionAt.IsSelected = optionAt.IsDefaultSelected;
      if (optionAt.IsSelected)
      {
        index1 = index2;
        ++num;
      }
    }
    if (num == 1 || this.IsMultiple || options.Length <= 0)
      return;
    foreach (IHtmlOptionElement htmlOptionElement in (IEnumerable<IHtmlOptionElement>) options)
      htmlOptionElement.IsSelected = false;
    options[index1].IsSelected = true;
  }

  internal void UpdateValue(string value)
  {
    foreach (IHtmlOptionElement option in (IEnumerable<IHtmlOptionElement>) this.Options)
      option.IsSelected = option.Value.Isi(value);
  }

  protected override bool CanBeValidated() => !this.HasDataListAncestor();

  protected override void Check(ValidityState state)
  {
    base.Check(state);
    state.IsValueMissing = this.IsRequired && string.IsNullOrEmpty(this.Value);
  }
}
