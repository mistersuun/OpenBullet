// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTextCRLFReader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class PythonTextCRLFReader(TextReader reader, Encoding encoding, long position) : 
  PythonTextReader(reader, encoding, position)
{
  private char[] _buffer = new char[160 /*0xA0*/];
  private int _bufPos;
  private int _bufLen;

  private int Read()
  {
    if (this._bufPos >= this._bufLen && this.ReadBuffer() == 0)
      return -1;
    ++this._position;
    return (int) this._buffer[this._bufPos++];
  }

  private int Peek()
  {
    return this._bufPos >= this._bufLen && this.ReadBuffer() == 0 ? -1 : (int) this._buffer[this._bufPos];
  }

  private int ReadBuffer()
  {
    this._bufLen = this._reader.Read(this._buffer, 0, this._buffer.Length);
    this._bufPos = 0;
    return this._bufLen;
  }

  public override string Read(int size)
  {
    if (size == 1)
    {
      int num = this.Read();
      switch (num)
      {
        case -1:
          return string.Empty;
        case 13:
          if (this.Peek() == 10)
          {
            num = this.Read();
            break;
          }
          break;
      }
      return ScriptingRuntimeHelpers.CharToString((char) num);
    }
    StringBuilder stringBuilder = new StringBuilder(size);
    while (size-- > 0)
    {
      int num = this.Read();
      switch (num)
      {
        case -1:
          goto label_12;
        case 13:
          if (this.Peek() == 10)
          {
            num = this.Read();
            break;
          }
          break;
      }
      stringBuilder.Append((char) num);
    }
label_12:
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override string ReadToEnd()
  {
    StringBuilder stringBuilder = new StringBuilder();
    while (true)
    {
      int num = this.Read();
      switch (num)
      {
        case -1:
          goto label_5;
        case 13:
          if (this.Peek() == 10)
          {
            num = this.Read();
            break;
          }
          break;
      }
      stringBuilder.Append((char) num);
    }
label_5:
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override string ReadLine() => this.ReadLine(int.MaxValue);

  public override string ReadLine(int size)
  {
    StringBuilder sb = (StringBuilder) null;
    if (this._bufPos >= this._bufLen)
      this.ReadBuffer();
    if (this._bufLen == 0)
      return string.Empty;
    int curIndex = this._bufPos;
    int num = 0;
    int lenAdj = 0;
    char ch;
    do
    {
      if (curIndex >= this._bufLen)
      {
        if (sb == null)
          sb = new StringBuilder((curIndex - this._bufPos) * 2);
        sb.Append(this._buffer, this._bufPos, curIndex - this._bufPos);
        if (this.ReadBuffer() == 0)
          return sb.ToString();
        curIndex = 0;
      }
      ch = this._buffer[curIndex++];
      if (ch == '\r')
      {
        if (curIndex < this._bufLen)
        {
          if (this._buffer[curIndex] == '\n')
          {
            ++this._position;
            ch = this._buffer[curIndex++];
            lenAdj = 2;
          }
        }
        else if (this._reader.Peek() == 10)
        {
          ch = (char) this._reader.Read();
          lenAdj = 1;
        }
      }
      ++this._position;
    }
    while (ch != '\n' && ++num < size);
    return this.FinishString(sb, curIndex, lenAdj);
  }

  private string FinishString(StringBuilder sb, int curIndex, int lenAdj)
  {
    int num = curIndex - this._bufPos;
    int bufPos = this._bufPos;
    this._bufPos = curIndex;
    if (sb != null)
    {
      if (lenAdj != 0)
      {
        sb.Append(this._buffer, bufPos, num - lenAdj);
        sb.Append('\n');
      }
      else
        sb.Append(this._buffer, bufPos, num);
      return sb.ToString();
    }
    return lenAdj != 0 ? new string(this._buffer, bufPos, num - lenAdj) + "\n" : new string(this._buffer, bufPos, num);
  }

  public override void DiscardBufferedData()
  {
    this._bufPos = this._bufLen = 0;
    base.DiscardBufferedData();
  }
}
