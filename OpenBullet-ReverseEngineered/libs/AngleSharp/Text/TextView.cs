// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.TextView
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Text;

public class TextView
{
  private readonly TextSource _source;
  private readonly TextRange _range;

  public TextView(TextSource source, TextRange range)
  {
    this._source = source;
    this._range = range;
  }

  public TextRange Range => this._range;

  public string Text
  {
    get
    {
      int startIndex = Math.Max(this._range.Start.Position - 1, 0);
      TextPosition textPosition = this._range.End;
      int num = textPosition.Position + 1;
      textPosition = this._range.Start;
      int position = textPosition.Position;
      int length = num - position;
      string text = this._source.Text;
      if (startIndex + length > text.Length)
        length = text.Length - startIndex;
      return text.Substring(startIndex, length);
    }
  }
}
