// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonBinaryReader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class PythonBinaryReader : PythonStreamReader
{
  private readonly Stream _stream;
  private const int BufferSize = 4096 /*0x1000*/;
  private byte[] _buffer;

  public override TextReader TextReader => (TextReader) null;

  public PythonBinaryReader(Stream stream)
    : base((Encoding) null)
  {
    this._stream = stream;
  }

  public override string Read(int size)
  {
    if (size == 0)
      return string.Empty;
    byte[] numArray;
    if (size <= 4096 /*0x1000*/)
    {
      if (this._buffer == null)
        this._buffer = new byte[4096 /*0x1000*/];
      numArray = this._buffer;
    }
    else
      numArray = new byte[size];
    int count = size;
    int offset = 0;
    while (true)
    {
      int num = this._stream.Read(numArray, offset, count);
      if (num > 0)
      {
        count -= num;
        if (count > 0)
          offset += num;
        else
          break;
      }
      else
        break;
    }
    return PythonBinaryReader.PackDataIntoString(numArray, size - count);
  }

  public override string ReadToEnd()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this._buffer == null)
      this._buffer = new byte[4096 /*0x1000*/];
    while (true)
    {
      int count = this._stream.Read(this._buffer, 0, 4096 /*0x1000*/);
      if (count != 0)
        stringBuilder.Append(PythonBinaryReader.PackDataIntoString(this._buffer, count));
      else
        break;
    }
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override string ReadLine()
  {
    StringBuilder stringBuilder = new StringBuilder(80 /*0x50*/);
    int num;
    do
    {
      num = this._stream.ReadByte();
      if (num != -1)
        stringBuilder.Append((char) num);
      else
        break;
    }
    while (num != 10);
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override string ReadLine(int size)
  {
    StringBuilder stringBuilder = new StringBuilder(80 /*0x50*/);
    while (size-- > 0)
    {
      int num = this._stream.ReadByte();
      if (num != -1)
      {
        stringBuilder.Append((char) num);
        if (num == 10)
          break;
      }
      else
        break;
    }
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override void DiscardBufferedData()
  {
  }

  public override long Position
  {
    get => this._stream.Position;
    internal set
    {
    }
  }

  internal static string PackDataIntoString(byte[] data, int count)
  {
    if (count == 1)
      return ScriptingRuntimeHelpers.CharToString((char) data[0]);
    StringBuilder stringBuilder = new StringBuilder(count);
    for (int index = 0; index < count; ++index)
      stringBuilder.Append((char) data[index]);
    return stringBuilder.ToString();
  }
}
