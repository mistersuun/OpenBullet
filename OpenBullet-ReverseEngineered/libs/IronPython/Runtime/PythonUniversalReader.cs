// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonUniversalReader
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class PythonUniversalReader : PythonTextReader
{
  private int _lastChar = -1;
  private PythonUniversalReader.TerminatorStyles _terminators;

  public PythonUniversalReader(TextReader reader, Encoding encoding, long position)
    : base(reader, encoding, position)
  {
    this._terminators = PythonUniversalReader.TerminatorStyles.None;
  }

  private int ReadOne()
  {
    if (this._lastChar == -1)
      return this._reader.Read();
    int lastChar = this._lastChar;
    this._lastChar = -1;
    return lastChar;
  }

  private int ReadChar()
  {
    int num1 = this.ReadOne();
    if (num1 != -1)
      ++this._position;
    if (num1 == 13)
    {
      int num2 = this._reader.Read();
      if (num2 == 10)
      {
        ++this._position;
        this._terminators |= PythonUniversalReader.TerminatorStyles.CrLf;
      }
      else
      {
        this._lastChar = num2;
        this._terminators |= PythonUniversalReader.TerminatorStyles.Cr;
      }
      num1 = 10;
    }
    else if (num1 == 10)
      this._terminators |= PythonUniversalReader.TerminatorStyles.Lf;
    return num1;
  }

  public override string Read(int size)
  {
    if (size == 1)
    {
      int num = this.ReadChar();
      return num == -1 ? string.Empty : ScriptingRuntimeHelpers.CharToString((char) num);
    }
    StringBuilder stringBuilder = new StringBuilder(size);
    while (size-- > 0)
    {
      int num = this.ReadChar();
      if (num != -1)
        stringBuilder.Append((char) num);
      else
        break;
    }
    return stringBuilder.Length == 0 ? string.Empty : stringBuilder.ToString();
  }

  public override string ReadToEnd()
  {
    StringBuilder stringBuilder = new StringBuilder();
    while (true)
    {
      int num = this.ReadChar();
      if (num != -1)
        stringBuilder.Append((char) num);
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
      num = this.ReadChar();
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
      int num = this.ReadChar();
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

  public PythonUniversalReader.TerminatorStyles Terminators => this._terminators;

  public enum TerminatorStyles
  {
    None = 0,
    CrLf = 1,
    Cr = 2,
    Lf = 4,
  }
}
