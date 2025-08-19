// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.AttrNotMatchSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Css.Dom;

internal sealed class AttrNotMatchSelector : BaseAttrSelector, ISelector
{
  private readonly string _value;
  private readonly StringComparison _comparison;

  public AttrNotMatchSelector(string name, string value, string prefix = null, bool insensitive = false)
    : base(name, prefix)
  {
    this._value = value;
    this._comparison = insensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
  }

  public string Text => $"[{this.Attribute}!={this._value.CssString()}]";

  public void Accept(ISelectorVisitor visitor)
  {
    visitor.Attribute(this.Attribute, "!=", this._value);
  }

  public bool Match(IElement element, IElement scope)
  {
    return !string.Equals(element.GetAttribute(this.Name), this._value, this._comparison);
  }
}
