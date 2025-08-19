// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.Tokens.HtmlAttributeToken
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Parser.Tokens;

public struct HtmlAttributeToken(TextPosition position, string name, string value)
{
  public string Name { get; } = name;

  public string Value { get; } = value;

  public TextPosition Position { get; } = position;
}
