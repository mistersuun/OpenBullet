// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.TokenizerBuffer
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Diagnostics;
using System.IO;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class TokenizerBuffer
{
  private const int FirstColumn = 1;
  public const int EndOfFile = -1;
  public const int InvalidCharacter = -2;
  private bool _multiEolns;
  private TextReader _reader;
  private char[] _buffer;
  private bool _bufferResized;
  private int _position;
  private SourceLocation _tokenStartLocation;
  private SourceLocation _tokenEndLocation;
  private int _start;
  private int _end;
  private int _tokenEnd;

  public TextReader Reader => this._reader;

  public bool AtBeginning => this._position == 0 && !this._bufferResized;

  public int TokenLength => this._tokenEnd - this._start;

  public int TokenRelativePosition => this._position - this._start;

  public int Position => this._position;

  public SourceSpan TokenSpan => new SourceSpan(this.TokenStart, this.TokenEnd);

  public SourceLocation TokenStart => this._tokenStartLocation;

  public SourceLocation TokenEnd => this._tokenEndLocation;

  public TokenizerBuffer(
    TextReader reader,
    SourceLocation initialLocation,
    int initialCapacity,
    bool multiEolns)
  {
    this.Initialize(reader, initialLocation, initialCapacity, multiEolns);
  }

  public void Initialize(
    TextReader reader,
    SourceLocation initialLocation,
    int initialCapacity,
    bool multiEolns)
  {
    ContractUtils.RequiresNotNull((object) reader, nameof (reader));
    ContractUtils.Requires(initialCapacity > 0, nameof (initialCapacity));
    this._reader = reader;
    if (this._buffer == null || this._buffer.Length < initialCapacity)
      this._buffer = new char[initialCapacity];
    this._tokenEnd = -1;
    this._multiEolns = multiEolns;
    this._tokenEndLocation = SourceLocation.Invalid;
    this._tokenStartLocation = initialLocation;
    this._start = this._end = 0;
    this._position = 0;
  }

  public int Read()
  {
    int num = this.Peek();
    ++this._position;
    return num;
  }

  public bool Read(int ch)
  {
    if (this.Peek() != ch)
      return false;
    ++this._position;
    return true;
  }

  public bool Read(string str)
  {
    int position = this._position;
    this.SeekRelative(str.Length - 1);
    if (this.Read() == -1)
    {
      this.Seek(position);
      return false;
    }
    int index = 0;
    while (index < str.Length && (int) this._buffer[index] == (int) str[index])
      ++index;
    if (index == str.Length)
      return true;
    this.Seek(position);
    return false;
  }

  public int Peek()
  {
    if (this._position >= this._end)
    {
      this.RefillBuffer();
      if (this._position >= this._end)
        return -1;
    }
    return (int) this._buffer[this._position];
  }

  private void RefillBuffer()
  {
    if (this._end == this._buffer.Length)
    {
      TokenizerBuffer.ResizeInternal(ref this._buffer, Math.Max(Math.Max((this._end - this._start) * 2, this._buffer.Length), this._position), this._start, this._end - this._start);
      this._end -= this._start;
      this._position -= this._start;
      this._start = 0;
      this._bufferResized = true;
    }
    this._end += this._reader.Read(this._buffer, this._end, this._buffer.Length - this._end);
  }

  public void Back() => this.SeekRelative(-1);

  public void Seek(int offset) => this._position = this._start + offset;

  public void SeekRelative(int disp) => this._position += disp;

  public void MarkMultiLineTokenEnd()
  {
    this._tokenEnd = Math.Min(this._position, this._end);
    this._tokenEndLocation = this._multiEolns ? this.GetTokenEndMultiEolns() : this.GetTokenEndSingleEoln();
  }

  public void MarkSingleLineTokenEnd()
  {
    this._tokenEnd = Math.Min(this._position, this._end);
    int num = this._tokenEnd - this._start;
    this._tokenEndLocation = new SourceLocation(this._tokenStartLocation.Index + num, this._tokenStartLocation.Line, this._tokenStartLocation.Column + num);
  }

  public void MarkMultiLineTokenEnd(int disp)
  {
    this.SeekRelative(disp);
    this.MarkMultiLineTokenEnd();
  }

  public void MarkSingleLineTokenEnd(int disp)
  {
    this.SeekRelative(disp);
    this.MarkSingleLineTokenEnd();
  }

  public void MarkTokenEnd(bool isMultiLine)
  {
    if (isMultiLine)
      this.MarkMultiLineTokenEnd();
    else
      this.MarkSingleLineTokenEnd();
  }

  public void DiscardToken()
  {
    if (this._tokenEnd == -1)
      this.MarkMultiLineTokenEnd();
    this._tokenStartLocation = this._tokenEndLocation;
    this._start = this._tokenEnd;
    this._tokenEnd = -1;
  }

  public char GetChar(int offset) => this._buffer[this._start + offset];

  public char GetCharRelative(int disp) => this._buffer[this._position + disp];

  public string GetTokenString()
  {
    return new string(this._buffer, this._start, this._tokenEnd - this._start);
  }

  public string GetTokenSubstring(int offset)
  {
    return this.GetTokenSubstring(offset, this._tokenEnd - this._start - offset);
  }

  public string GetTokenSubstring(int offset, int length)
  {
    return new string(this._buffer, this._start + offset, length);
  }

  private SourceLocation GetTokenEndSingleEoln()
  {
    int line = this._tokenStartLocation.Line;
    int column = this._tokenStartLocation.Column;
    for (int start = this._start; start < this._tokenEnd; ++start)
    {
      if (this._buffer[start] == '\n')
      {
        column = 1;
        ++line;
      }
      else
        ++column;
    }
    return new SourceLocation(this._tokenStartLocation.Index + this._tokenEnd - this._start, line, column);
  }

  private SourceLocation GetTokenEndMultiEolns()
  {
    int line = this._tokenStartLocation.Line;
    int column = this._tokenStartLocation.Column;
    int start;
    for (start = this._start; start < this._tokenEnd - 1; ++start)
    {
      if (this._buffer[start] == '\n')
      {
        column = 1;
        ++line;
      }
      else if (this._buffer[start] == '\r')
      {
        column = 1;
        ++line;
        if (this._buffer[start + 1] == '\n')
          ++start;
      }
      else
        ++column;
    }
    if (start < this._tokenEnd)
    {
      if (this._buffer[start] == '\n')
      {
        column = 1;
        ++line;
      }
      else if (this._buffer[start] == '\r')
      {
        column = 1;
        ++line;
      }
      else
        ++column;
    }
    return new SourceLocation(this._tokenStartLocation.Index + this._tokenEnd - this._start, line, column);
  }

  public bool IsEoln(int current)
  {
    switch (current)
    {
      case 10:
        return true;
      case 13:
        if (this._multiEolns)
        {
          this.Peek();
          return true;
        }
        break;
    }
    return false;
  }

  public int ReadEolnOpt(int current)
  {
    switch (current)
    {
      case 10:
        return 1;
      case 13:
        if (this._multiEolns)
        {
          if (this.Peek() != 10)
            return 1;
          this.SeekRelative(1);
          return 2;
        }
        break;
    }
    return 0;
  }

  public int ReadLine()
  {
    int current;
    do
    {
      current = this.Read();
    }
    while (current != -1 && !this.IsEoln(current));
    this.Back();
    return current;
  }

  private static void ResizeInternal(ref char[] array, int newSize, int start, int count)
  {
    char[] dst = newSize != array.Length ? new char[newSize] : array;
    Buffer.BlockCopy((Array) array, start * 2, (Array) dst, 0, count * 2);
    array = dst;
  }

  [Conditional("DEBUG")]
  private void ClearInvalidChars()
  {
    for (int index = 0; index < this._start; ++index)
      this._buffer[index] = char.MinValue;
    for (int end = this._end; end < this._buffer.Length; ++end)
      this._buffer[end] = char.MinValue;
  }

  [Conditional("DEBUG")]
  private void CheckInvariants()
  {
  }

  [Conditional("DUMP_TOKENS")]
  private void DumpToken()
  {
  }
}
