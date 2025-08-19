// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTextLFReader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class PythonTextLFReader(TextReader reader, Encoding encoding, long position) : 
  PythonTextReader(reader, encoding, position)
{
  public override string Read(int size)
  {
    if (size == 1)
    {
      int num = this._reader.Read();
      if (num == -1)
        return string.Empty;
      ++this._position;
      return ScriptingRuntimeHelpers.CharToString((char) num);
    }
    StringBuilder stringBuilder = new StringBuilder(size);
    while (size-- > 0)
    {
      int num = this._reader.Read();
      if (num != -1)
      {
        ++this._position;
        stringBuilder.Append((char) num);
      }
      else
        break;
    }
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override string ReadToEnd() => this._reader.ReadToEnd();

  public override string ReadLine()
  {
    StringBuilder stringBuilder = new StringBuilder(80 /*0x50*/);
    int num;
    do
    {
      num = this._reader.Read();
      if (num != -1)
      {
        ++this._position;
        stringBuilder.Append((char) num);
      }
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
      int num = this._reader.Read();
      if (num != -1)
      {
        ++this._position;
        stringBuilder.Append((char) num);
        if (num == 10)
          break;
      }
      else
        break;
    }
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }
}
