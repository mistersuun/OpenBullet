// Decompiled with JetBrains decompiler
// Type: AngleSharp.Text.StringSource
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Text;

public sealed class StringSource
{
  private readonly string _content;
  private readonly int _last;
  private int _index;
  private char _current;

  public StringSource(string content)
  {
    this._content = content ?? string.Empty;
    this._last = this._content.Length - 1;
    this._index = 0;
    this._current = this._last == -1 ? char.MaxValue : content[0];
  }

  public char Current => this._current;

  public bool IsDone => this._current == char.MaxValue;

  public int Index => this._index;

  public string Content => this._content;

  public char Next()
  {
    if (this._index == this._last)
    {
      this._current = char.MaxValue;
      this._index = this._content.Length;
    }
    else if (this._index < this._content.Length)
      this._current = this._content[++this._index];
    return this._current;
  }

  public char Back()
  {
    if (this._index > 0)
      this._current = this._content[--this._index];
    return this._current;
  }
}
