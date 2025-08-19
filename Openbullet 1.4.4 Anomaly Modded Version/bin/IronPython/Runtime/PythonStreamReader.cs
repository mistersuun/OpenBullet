// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonStreamReader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal abstract class PythonStreamReader
{
  protected Encoding _encoding;

  public Encoding Encoding => this._encoding;

  public abstract TextReader TextReader { get; }

  public PythonStreamReader(Encoding encoding) => this._encoding = encoding;

  public abstract string Read(int size);

  public abstract string ReadToEnd();

  public abstract string ReadLine();

  public abstract string ReadLine(int size);

  public abstract void DiscardBufferedData();

  public abstract long Position { get; internal set; }
}
