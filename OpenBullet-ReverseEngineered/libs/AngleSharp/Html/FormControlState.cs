// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.FormControlState
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Html;

internal sealed class FormControlState
{
  private readonly string _name;
  private readonly string _type;
  private readonly string _value;

  public FormControlState(string name, string type, string value)
  {
    this._name = name;
    this._type = type;
    this._value = value;
  }

  public string Name => this._name;

  public string Value => this._value;

  public string Type => this._type;
}
