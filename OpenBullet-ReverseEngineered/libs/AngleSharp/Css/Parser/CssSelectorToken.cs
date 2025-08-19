// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Parser.CssSelectorToken
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Css.Parser;

internal struct CssSelectorToken(CssTokenType type, string data)
{
  private readonly CssTokenType _type = type;
  private readonly string _data = data;
  public static readonly CssSelectorToken Whitespace = new CssSelectorToken(CssTokenType.Whitespace, " ");

  public CssTokenType Type => this._type;

  public string Data => this._data;
}
