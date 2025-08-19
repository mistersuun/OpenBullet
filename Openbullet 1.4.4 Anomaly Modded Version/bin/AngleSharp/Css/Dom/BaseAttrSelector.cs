// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Dom.BaseAttrSelector
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Css.Dom;

internal abstract class BaseAttrSelector
{
  private readonly string _name;
  private readonly string _prefix;
  private readonly string _attr;

  public BaseAttrSelector(string name, string prefix)
  {
    this._name = name;
    this._prefix = prefix;
    if (!string.IsNullOrEmpty(prefix) && !prefix.Is("*"))
      this._attr = $"{prefix}:{name}";
    else
      this._attr = name;
  }

  public Priority Specificity => Priority.OneClass;

  protected string Attribute
  {
    get => string.IsNullOrEmpty(this._prefix) ? this._name : $"{this._prefix}|{this._name}";
  }

  protected string Name => this._attr;
}
