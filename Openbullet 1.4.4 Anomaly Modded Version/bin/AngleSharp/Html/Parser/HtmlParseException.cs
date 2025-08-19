// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlParseException
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Parser;

public class HtmlParseException : Exception
{
  public HtmlParseException(int code, string message, TextPosition position)
    : base(message)
  {
    this.Code = code;
    this.Position = position;
  }

  public TextPosition Position { get; private set; }

  public int Code { get; private set; }
}
