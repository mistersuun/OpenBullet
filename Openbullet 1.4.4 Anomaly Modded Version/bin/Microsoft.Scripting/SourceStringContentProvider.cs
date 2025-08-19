// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceStringContentProvider
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.IO;
using System.Text;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
internal sealed class SourceStringContentProvider : TextContentProvider
{
  private readonly string _code;

  internal SourceStringContentProvider(string code)
  {
    ContractUtils.RequiresNotNull((object) code, nameof (code));
    this._code = code;
  }

  public override SourceCodeReader GetReader()
  {
    return new SourceCodeReader((TextReader) new StringReader(this._code), (Encoding) null);
  }
}
