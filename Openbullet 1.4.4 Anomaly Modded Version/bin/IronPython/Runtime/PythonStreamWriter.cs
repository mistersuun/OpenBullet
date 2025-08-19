// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonStreamWriter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal abstract class PythonStreamWriter
{
  protected Encoding _encoding;

  public Encoding Encoding => this._encoding;

  public abstract TextWriter TextWriter { get; }

  public PythonStreamWriter(Encoding encoding) => this._encoding = encoding;

  public abstract int Write(string data);

  public abstract int WriteBytes(IList<byte> data);

  public abstract void Flush();

  public abstract void FlushToDisk();

  public void FlushToDiskWorker(Stream stream)
  {
    if (!(stream is FileStream fileStream))
      return;
    fileStream.Flush(true);
  }
}
