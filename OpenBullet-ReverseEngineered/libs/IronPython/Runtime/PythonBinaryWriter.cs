// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonBinaryWriter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class PythonBinaryWriter : PythonStreamWriter
{
  private Stream _stream;

  public override TextWriter TextWriter => (TextWriter) null;

  public PythonBinaryWriter(Stream stream)
    : base((Encoding) null)
  {
    this._stream = stream;
  }

  public override int Write(string data)
  {
    byte[] bytes = PythonAsciiEncoding.Instance.GetBytes(data);
    this._stream.Write(bytes, 0, bytes.Length);
    return bytes.Length;
  }

  public override int WriteBytes(IList<byte> data)
  {
    int count = data.Count;
    for (int index = 0; index < count; ++index)
      this._stream.WriteByte(data[index]);
    return count;
  }

  public override void Flush() => this._stream.Flush();

  public override void FlushToDisk() => this.FlushToDiskWorker(this._stream);
}
