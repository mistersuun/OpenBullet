// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTextWriter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class PythonTextWriter : PythonStreamWriter
{
  private TextWriter _writer;
  private readonly string _eoln;

  public override TextWriter TextWriter => this._writer;

  public PythonTextWriter(TextWriter writer, string eoln)
    : base(writer.Encoding)
  {
    this._writer = writer;
    this._eoln = eoln;
  }

  public override int Write(string data)
  {
    if (this._eoln != null)
      data = data.Replace("\n", this._eoln);
    this._writer.Write(data);
    return data.Length;
  }

  public override int WriteBytes(IList<byte> data)
  {
    int count = data.Count;
    StringBuilder stringBuilder = new StringBuilder(this._eoln.Length > 1 ? (int) ((double) count * 1.2) : count);
    for (int index = 0; index < count; ++index)
    {
      char ch = (char) data[index];
      if (ch == '\n')
        stringBuilder.Append(this._eoln);
      else
        stringBuilder.Append(ch);
    }
    this._writer.Write(stringBuilder.ToString());
    return count;
  }

  public override void Flush() => this._writer.Flush();

  public override void FlushToDisk()
  {
    if (!(this._writer is StreamWriter writer))
      return;
    writer.Flush();
    this.FlushToDiskWorker(writer.BaseStream);
  }
}
