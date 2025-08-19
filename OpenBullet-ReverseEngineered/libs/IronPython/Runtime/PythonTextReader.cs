// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTextReader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal abstract class PythonTextReader : PythonStreamReader
{
  protected readonly TextReader _reader;
  protected long _position;

  public override TextReader TextReader => this._reader;

  public override long Position
  {
    get => this._position;
    internal set => this._position = value;
  }

  public PythonTextReader(TextReader reader, Encoding encoding, long position)
    : base(encoding)
  {
    this._reader = reader;
    this._position = position;
  }

  public override void DiscardBufferedData()
  {
    if (!(this._reader is StreamReader reader))
      return;
    reader.DiscardBufferedData();
  }
}
