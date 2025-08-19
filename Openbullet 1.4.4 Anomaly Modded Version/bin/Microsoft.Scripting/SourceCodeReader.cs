// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceCodeReader
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Scripting;

public class SourceCodeReader : TextReader
{
  public static readonly SourceCodeReader Null = new SourceCodeReader(TextReader.Null, (Encoding) null);

  public SourceCodeReader(TextReader textReader, Encoding encoding)
  {
    ContractUtils.RequiresNotNull((object) textReader, nameof (textReader));
    this.Encoding = encoding;
    this.BaseReader = textReader;
  }

  public Encoding Encoding { get; }

  public TextReader BaseReader { get; }

  public override string ReadLine() => this.BaseReader.ReadLine();

  public virtual bool SeekLine(int line)
  {
    if (line < 1)
      throw new ArgumentOutOfRangeException(nameof (line));
    if (line == 1)
      return true;
    int num1 = 1;
    int num2;
    do
    {
      num2 = this.BaseReader.Read();
      if (num2 == 13)
      {
        if (this.BaseReader.Peek() == 10)
          this.BaseReader.Read();
        ++num1;
        if (num1 == line)
          return true;
      }
      else if (num2 == 10)
      {
        ++num1;
        if (num1 == line)
          return true;
      }
    }
    while (num2 != -1);
    return false;
  }

  public override string ReadToEnd() => this.BaseReader.ReadToEnd();

  public override int Read(char[] buffer, int index, int count)
  {
    return this.BaseReader.Read(buffer, index, count);
  }

  public override int Peek() => this.BaseReader.Peek();

  public override int Read() => this.BaseReader.Read();

  protected override void Dispose(bool disposing)
  {
    this.BaseReader.Dispose();
    base.Dispose(disposing);
  }
}
