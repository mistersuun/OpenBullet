// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.StringBuffer
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal struct StringBuffer
{
  private char[] _buffer;
  private int _position;

  public int Position
  {
    get => this._position;
    set => this._position = value;
  }

  public bool IsEmpty => this._buffer == null;

  public StringBuffer(IArrayPool<char> bufferPool, int initalSize)
    : this(BufferUtils.RentBuffer(bufferPool, initalSize))
  {
  }

  private StringBuffer(char[] buffer)
  {
    this._buffer = buffer;
    this._position = 0;
  }

  public void Append(IArrayPool<char> bufferPool, char value)
  {
    if (this._position == this._buffer.Length)
      this.EnsureSize(bufferPool, 1);
    this._buffer[this._position++] = value;
  }

  public void Append(IArrayPool<char> bufferPool, char[] buffer, int startIndex, int count)
  {
    if (this._position + count >= this._buffer.Length)
      this.EnsureSize(bufferPool, count);
    Array.Copy((Array) buffer, startIndex, (Array) this._buffer, this._position, count);
    this._position += count;
  }

  public void Clear(IArrayPool<char> bufferPool)
  {
    if (this._buffer != null)
    {
      BufferUtils.ReturnBuffer(bufferPool, this._buffer);
      this._buffer = (char[]) null;
    }
    this._position = 0;
  }

  private void EnsureSize(IArrayPool<char> bufferPool, int appendLength)
  {
    char[] destinationArray = BufferUtils.RentBuffer(bufferPool, (this._position + appendLength) * 2);
    if (this._buffer != null)
    {
      Array.Copy((Array) this._buffer, (Array) destinationArray, this._position);
      BufferUtils.ReturnBuffer(bufferPool, this._buffer);
    }
    this._buffer = destinationArray;
  }

  public override string ToString() => this.ToString(0, this._position);

  public string ToString(int start, int length) => new string(this._buffer, start, length);

  public char[] InternalBuffer => this._buffer;
}
