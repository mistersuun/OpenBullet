// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.NoLineFeedSourceContentProvider
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Utils;
using System.IO;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal sealed class NoLineFeedSourceContentProvider : TextContentProvider
{
  private readonly string _code;

  public NoLineFeedSourceContentProvider(string code) => this._code = code;

  public override SourceCodeReader GetReader()
  {
    return (SourceCodeReader) new NoLineFeedSourceContentProvider.Reader(this._code);
  }

  internal sealed class Reader : SourceCodeReader
  {
    internal Reader(string s)
      : base((TextReader) new StringReader(s), (Encoding) null)
    {
    }

    public override string ReadLine() => IOUtils.ReadTo((TextReader) this, '\n');

    public override bool SeekLine(int line)
    {
      for (int index = 1; index != line; ++index)
      {
        if (!IOUtils.SeekTo((TextReader) this, '\n'))
          return false;
      }
      return true;
    }
  }
}
