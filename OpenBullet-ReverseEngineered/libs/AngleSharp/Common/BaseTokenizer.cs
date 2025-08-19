// Decompiled with JetBrains decompiler
// Type: AngleSharp.Common.BaseTokenizer
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Common;

public abstract class BaseTokenizer : IDisposable
{
  private readonly Stack<ushort> _columns;
  private readonly TextSource _source;
  private ushort _column;
  private ushort _row;
  private char _current;
  private StringBuilder _buffer;
  private bool _normalized;

  public BaseTokenizer(TextSource source)
  {
    this._buffer = StringBuilderPool.Obtain();
    this._columns = new Stack<ushort>();
    this._source = source;
    this._current = char.MinValue;
    this._column = (ushort) 0;
    this._row = (ushort) 1;
  }

  public int InsertionPoint
  {
    get => this._source.Index;
    protected set
    {
      int num;
      for (num = this._source.Index - value; num > 0; --num)
        this.BackUnsafe();
      for (; num < 0; ++num)
        this.AdvanceUnsafe();
    }
  }

  public int Position => this._source.Index - (this._normalized ? 1 : 0);

  protected char Current => this._current;

  protected StringBuilder StringBuffer => this._buffer;

  protected bool IsNormalized => this._normalized;

  public string FlushBuffer() => this.FlushBuffer((Func<StringBuilder, string>) null);

  internal string FlushBuffer(Func<StringBuilder, string> stringResolver)
  {
    string str = (stringResolver != null ? stringResolver(this.StringBuffer) : (string) null) ?? this.StringBuffer.ToString();
    this.StringBuffer.Clear();
    return str;
  }

  public void Dispose()
  {
    if (this.StringBuffer == null)
      return;
    this._source?.Dispose();
    this.StringBuffer.Clear().ToPool();
    this._buffer = (StringBuilder) null;
  }

  public TextPosition GetCurrentPosition()
  {
    return new TextPosition(this._row, this._column, this.Position);
  }

  protected bool ContinuesWithInsensitive(string s)
  {
    string current = this.PeekString(s.Length);
    return current.Length == s.Length && current.Isi(s);
  }

  protected bool ContinuesWithSensitive(string s)
  {
    string current = this.PeekString(s.Length);
    return current.Length == s.Length && current.Is(s);
  }

  protected string PeekString(int length)
  {
    int index = this._source.Index;
    --this._source.Index;
    string str = this._source.ReadCharacters(length);
    this._source.Index = index;
    return str;
  }

  protected char SkipSpaces()
  {
    char next = this.GetNext();
    while (next.IsSpaceCharacter())
      next = this.GetNext();
    return next;
  }

  protected char GetNext()
  {
    this.Advance();
    return this._current;
  }

  protected char GetPrevious()
  {
    this.Back();
    return this._current;
  }

  protected void Advance()
  {
    if (this._current == char.MaxValue)
      return;
    this.AdvanceUnsafe();
  }

  protected void Advance(int n)
  {
    while (n-- > 0 && this._current != char.MaxValue)
      this.AdvanceUnsafe();
  }

  protected void Back()
  {
    if (this.InsertionPoint <= 0)
      return;
    this.BackUnsafe();
  }

  protected void Back(int n)
  {
    while (n-- > 0 && this.InsertionPoint > 0)
      this.BackUnsafe();
  }

  private void AdvanceUnsafe()
  {
    if (this._current == '\n')
    {
      this._columns.Push(this._column);
      this._column = (ushort) 1;
      ++this._row;
    }
    else
      ++this._column;
    this._current = this.NormalizeForward(this._source.ReadCharacter());
  }

  private void BackUnsafe()
  {
    --this._source.Index;
    if (this._source.Index == 0)
    {
      this._column = (ushort) 0;
      this._current = char.MinValue;
    }
    else
    {
      char ch = this.NormalizeBackward(this._source[this._source.Index - 1]);
      switch (ch)
      {
        case char.MinValue:
          break;
        case '\n':
          this._column = this._columns.Count != 0 ? this._columns.Pop() : (ushort) 1;
          --this._row;
          this._current = ch;
          break;
        default:
          this._current = ch;
          --this._column;
          break;
      }
    }
  }

  private char NormalizeForward(char p)
  {
    if (p != '\r')
    {
      this._normalized = false;
      return p;
    }
    if (this._source.ReadCharacter() != '\n')
      --this._source.Index;
    else
      this._normalized = true;
    return '\n';
  }

  private char NormalizeBackward(char p)
  {
    if (p != '\r')
    {
      this._normalized = false;
      return p;
    }
    if (this._source.Index < this._source.Length && this._source[this._source.Index] == '\n')
    {
      this._normalized = false;
      this.BackUnsafe();
      return char.MinValue;
    }
    this._normalized = true;
    return '\n';
  }
}
