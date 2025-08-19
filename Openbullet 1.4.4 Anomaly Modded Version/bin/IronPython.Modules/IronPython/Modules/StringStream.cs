// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.StringStream
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Modules;

internal class StringStream
{
  private StringBuilder _data;
  private string _lastValue;
  private int _position;

  public StringStream(string data)
  {
    this._data = new StringBuilder(this._lastValue = data);
    this._position = 0;
  }

  public bool EOF => this._position >= this._data.Length;

  public int Position => this._position;

  public string Data
  {
    get
    {
      if (this._lastValue == null)
        this._lastValue = this._data.ToString();
      return this._lastValue;
    }
  }

  public string Prefix => this._data.ToString(0, this._position);

  public string Read(int i)
  {
    if (this._position + i > this._data.Length)
      i = this._data.Length - this._position;
    string str = this._data.ToString(this._position, i);
    this._position += i;
    return str;
  }

  public string ReadLine(int size)
  {
    if (size < 0)
      size = int.MaxValue;
    int position = this._position;
    for (int index1 = 0; position < this._data.Length && index1 < size; ++index1)
    {
      char ch = this._data[position];
      switch (ch)
      {
        case '\n':
        case '\r':
          int index2 = position + 1;
          if (ch == '\r' && this._position < this._data.Length && this._data[index2] == '\n')
            ++index2;
          string str = this._data.ToString(this._position, index2 - this._position);
          this._position = index2;
          return str;
        default:
          ++position;
          continue;
      }
    }
    if (position <= this._position)
      return "";
    string str1 = this._data.ToString(this._position, position - this._position);
    this._position = position;
    return str1;
  }

  public string ReadToEnd()
  {
    if (this._position >= this._data.Length)
      return string.Empty;
    string end = this._data.ToString(this._position, this._data.Length - this._position);
    this._position = this._data.Length;
    return end;
  }

  public void Reset() => this._position = 0;

  public int Seek(int offset, SeekOrigin origin)
  {
    switch (origin)
    {
      case SeekOrigin.Begin:
        this._position = offset;
        break;
      case SeekOrigin.Current:
        this._position += offset;
        break;
      case SeekOrigin.End:
        this._position = this._data.Length + offset;
        break;
      default:
        throw new ValueErrorException(nameof (origin));
    }
    return this._position;
  }

  public void Truncate()
  {
    this._lastValue = (string) null;
    this._data.Length = this._position;
  }

  public void Truncate(int size)
  {
    this._lastValue = (string) null;
    if (size > this._data.Length)
      size = this._data.Length;
    else if (size < 0)
      throw PythonOps.IOError("(22, 'Negative size not allowed')");
    this._data.Length = size;
    this._position = size;
  }

  internal void Write(string s)
  {
    if (this._data.Length < this._position)
      this._data.Length = this._position;
    this._lastValue = (string) null;
    if (this._position == this._data.Length)
    {
      this._data.Append(s);
    }
    else
    {
      this._data.Remove(this._position, Math.Min(s.Length, this._data.Length - this._position));
      this._data.Insert(this._position, s);
    }
    this._position += s.Length;
  }
}
