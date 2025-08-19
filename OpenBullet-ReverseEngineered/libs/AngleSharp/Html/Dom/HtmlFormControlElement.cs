// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlFormControlElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;
using System.Linq;

#nullable disable
namespace AngleSharp.Html.Dom;

internal abstract class HtmlFormControlElement : HtmlElement, ILabelabelElement, IValidation
{
  private readonly NodeList _labels;
  private readonly ValidityState _vstate;
  private string _error;

  public HtmlFormControlElement(Document owner, string name, string prefix, NodeFlags flags = NodeFlags.None)
    : base(owner, name, prefix, flags | NodeFlags.Special)
  {
    this._vstate = new ValidityState();
    this._labels = new NodeList();
  }

  public string Name
  {
    get => this.GetOwnAttribute(AttributeNames.Name);
    set => this.SetOwnAttribute(AttributeNames.Name, value);
  }

  public IHtmlFormElement Form => this.GetAssignedForm();

  public bool IsDisabled
  {
    get => this.GetBoolAttribute(AttributeNames.Disabled) || this.IsFieldsetDisabled();
    set => this.SetBoolAttribute(AttributeNames.Disabled, value);
  }

  public bool Autofocus
  {
    get => this.GetBoolAttribute(AttributeNames.AutoFocus);
    set => this.SetBoolAttribute(AttributeNames.AutoFocus, value);
  }

  public INodeList Labels => (INodeList) this._labels;

  public string ValidationMessage => !this._vstate.IsCustomError ? string.Empty : this._error;

  public bool WillValidate => !this.IsDisabled && this.CanBeValidated();

  public IValidityState Validity
  {
    get
    {
      this.Check(this._vstate);
      return (IValidityState) this._vstate;
    }
  }

  public override Node Clone(Document owner, bool deep)
  {
    HtmlFormControlElement formControlElement = (HtmlFormControlElement) base.Clone(owner, deep);
    formControlElement.SetCustomValidity(this._error);
    return (Node) formControlElement;
  }

  public bool CheckValidity() => this.WillValidate && this.Validity.IsValid;

  public void SetCustomValidity(string error)
  {
    this._error = error;
    this.ResetValidity(this._vstate);
  }

  protected virtual bool IsFieldsetDisabled()
  {
    foreach (IHtmlFieldSetElement htmlFieldSetElement in this.GetAncestors().OfType<IHtmlFieldSetElement>())
    {
      if (htmlFieldSetElement.IsDisabled)
        return !this.IsDescendantOf(htmlFieldSetElement.ChildNodes.FirstOrDefault<INode>((Func<INode, bool>) (m => m is IHtmlLegendElement)));
    }
    return false;
  }

  internal virtual void ConstructDataSet(FormDataSet dataSet, IHtmlElement submitter)
  {
  }

  internal virtual void Reset()
  {
  }

  protected virtual void Check(ValidityState state) => this.ResetValidity(state);

  protected void ResetValidity(ValidityState state)
  {
    state.IsCustomError = !string.IsNullOrEmpty(this._error);
  }

  protected abstract bool CanBeValidated();
}
