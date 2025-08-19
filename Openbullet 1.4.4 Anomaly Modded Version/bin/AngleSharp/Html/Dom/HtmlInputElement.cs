// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlInputElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.InputTypes;
using AngleSharp.Io.Dom;
using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlInputElement(Document owner, string prefix = null) : 
  HtmlTextFormControlElement(owner, TagNames.Input, prefix, NodeFlags.SelfClosing),
  IHtmlInputElement,
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
  private BaseInputType _type;
  private bool? _checked;

  public override string DefaultValue
  {
    get => this.GetOwnAttribute(AttributeNames.Value) ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.Value, value);
  }

  public bool IsDefaultChecked
  {
    get => this.GetBoolAttribute(AttributeNames.Checked);
    set => this.SetBoolAttribute(AttributeNames.Checked, value);
  }

  public bool IsChecked
  {
    get => !this._checked.HasValue ? this.IsDefaultChecked : this._checked.Value;
    set => this._checked = new bool?(value);
  }

  public string Type
  {
    get => this._type.Name;
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public bool IsIndeterminate { get; set; }

  public bool IsMultiple
  {
    get => this.GetBoolAttribute(AttributeNames.Multiple);
    set => this.SetBoolAttribute(AttributeNames.Multiple, value);
  }

  public DateTime? ValueAsDate
  {
    get => this._type.ConvertToDate(this.Value);
    set
    {
      if (!value.HasValue)
        this.Value = string.Empty;
      else
        this.Value = this._type.ConvertFromDate(value.Value);
    }
  }

  public double ValueAsNumber
  {
    get => this._type.ConvertToNumber(this.Value) ?? double.NaN;
    set
    {
      if (double.IsInfinity(value))
        throw new DomException(DomError.TypeMismatch);
      if (double.IsNaN(value))
        this.Value = string.Empty;
      else
        this.Value = this._type.ConvertFromNumber(value);
    }
  }

  public string FormAction
  {
    get
    {
      string ownAttribute = this.GetOwnAttribute(AttributeNames.FormAction);
      if (ownAttribute != null)
        return ownAttribute;
      return this.Owner?.DocumentUri;
    }
    set => this.SetOwnAttribute(AttributeNames.FormAction, value);
  }

  public string FormEncType
  {
    get => this.GetOwnAttribute(AttributeNames.FormEncType).ToEncodingType() ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.FormEncType, value);
  }

  public string FormMethod
  {
    get => this.GetOwnAttribute(AttributeNames.FormMethod).ToFormMethod() ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.FormMethod, value);
  }

  public bool FormNoValidate
  {
    get => this.GetBoolAttribute(AttributeNames.FormNoValidate);
    set => this.SetBoolAttribute(AttributeNames.FormNoValidate, value);
  }

  public string FormTarget
  {
    get => this.GetOwnAttribute(AttributeNames.FormTarget) ?? string.Empty;
    set => this.SetOwnAttribute(AttributeNames.FormTarget, value);
  }

  public string Accept
  {
    get => this.GetOwnAttribute(AttributeNames.Accept);
    set => this.SetOwnAttribute(AttributeNames.Accept, value);
  }

  public Alignment Align
  {
    get => this.GetOwnAttribute(AttributeNames.Align).ToEnum<Alignment>(Alignment.Left);
    set => this.SetOwnAttribute(AttributeNames.Align, value.ToString());
  }

  public string AlternativeText
  {
    get => this.GetOwnAttribute(AttributeNames.Alt);
    set => this.SetOwnAttribute(AttributeNames.Alt, value);
  }

  public string Autocomplete
  {
    get => this.GetOwnAttribute(AttributeNames.AutoComplete);
    set => this.SetOwnAttribute(AttributeNames.AutoComplete, value);
  }

  public IFileList Files
  {
    get => !(this._type is FileInputType type) ? (IFileList) null : (IFileList) type.Files;
  }

  public IHtmlDataListElement List
  {
    get
    {
      return this.Owner?.GetElementById(this.GetOwnAttribute(AttributeNames.List)) as IHtmlDataListElement;
    }
  }

  public string Maximum
  {
    get => this.GetOwnAttribute(AttributeNames.Max);
    set => this.SetOwnAttribute(AttributeNames.Max, value);
  }

  public string Minimum
  {
    get => this.GetOwnAttribute(AttributeNames.Min);
    set => this.SetOwnAttribute(AttributeNames.Min, value);
  }

  public string Pattern
  {
    get => this.GetOwnAttribute(AttributeNames.Pattern);
    set => this.SetOwnAttribute(AttributeNames.Pattern, value);
  }

  public int Size
  {
    get => this.GetOwnAttribute(AttributeNames.Size).ToInteger(20);
    set => this.SetOwnAttribute(AttributeNames.Size, value.ToString());
  }

  public string Source
  {
    get => this.GetOwnAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string Step
  {
    get => this.GetOwnAttribute(AttributeNames.Step);
    set => this.SetOwnAttribute(AttributeNames.Step, value);
  }

  public string UseMap
  {
    get => this.GetOwnAttribute(AttributeNames.UseMap);
    set => this.SetOwnAttribute(AttributeNames.UseMap, value);
  }

  public int DisplayWidth
  {
    get => this.GetOwnAttribute(AttributeNames.Width).ToInteger(this.OriginalWidth);
    set => this.SetOwnAttribute(AttributeNames.Width, value.ToString());
  }

  public int DisplayHeight
  {
    get => this.GetOwnAttribute(AttributeNames.Height).ToInteger(this.OriginalHeight);
    set => this.SetOwnAttribute(AttributeNames.Height, value.ToString());
  }

  public int OriginalWidth => !(this._type is ImageInputType type) ? 0 : type.Width;

  public int OriginalHeight => !(this._type is ImageInputType type) ? 0 : type.Height;

  internal bool IsVisited { get; set; }

  internal bool IsActive { get; set; }

  public sealed override Node Clone(Document owner, bool deep)
  {
    HtmlInputElement htmlInputElement = (HtmlInputElement) base.Clone(owner, deep);
    htmlInputElement._checked = this._checked;
    htmlInputElement.UpdateType(this._type.Name);
    return (Node) htmlInputElement;
  }

  public override async void DoClick()
  {
    HtmlInputElement htmlInputElement = this;
    int num = await htmlInputElement.IsClickedCancelled().ConfigureAwait(false) ? 1 : 0;
    IHtmlFormElement form = htmlInputElement.Form;
    if (num != 0 || form == null)
      return;
    string type = htmlInputElement.Type;
    if (type.Is(InputTypeNames.Submit))
    {
      IDocument document = await form.SubmitAsync().ConfigureAwait(false);
    }
    else
    {
      if (!type.Is(InputTypeNames.Reset))
        return;
      form.Reset();
    }
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

  public void StepUp(int n = 1) => this._type.DoStep(n);

  public void StepDown(int n = 1) => this._type.DoStep(-n);

  internal bool IsMutable => !this.IsDisabled && !this.IsReadOnly;

  internal override void SetupElement()
  {
    base.SetupElement();
    this.UpdateType(this.GetOwnAttribute(AttributeNames.Type));
  }

  internal void UpdateType(string value)
  {
    this._type = this.Context.GetFactory<IInputTypeFactory>().Create((IHtmlInputElement) this, value);
  }

  internal override void ConstructDataSet(FormDataSet dataSet, IHtmlElement submitter)
  {
    if (!this._type.IsAppendingData(submitter))
      return;
    this._type.ConstructDataSet(dataSet);
  }

  internal override void Reset()
  {
    base.Reset();
    this._checked = new bool?();
    this.UpdateType(this.Type);
  }

  protected override void Check(ValidityState state)
  {
    base.Check(state);
    ValidationErrors err = this._type.Check((IValidityState) state);
    state.Reset(err);
  }

  protected override bool CanBeValidated() => this._type.CanBeValidated && base.CanBeValidated();
}
