// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceStringContentProvider
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

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
