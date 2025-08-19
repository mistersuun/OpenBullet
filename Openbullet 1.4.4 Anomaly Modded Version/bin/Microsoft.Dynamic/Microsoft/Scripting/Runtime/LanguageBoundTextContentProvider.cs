// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.LanguageBoundTextContentProvider
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Text;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal sealed class LanguageBoundTextContentProvider : TextContentProvider
{
  private readonly LanguageContext _context;
  private readonly StreamContentProvider _streamProvider;
  private readonly Encoding _defaultEncoding;
  private readonly string _path;

  public LanguageBoundTextContentProvider(
    LanguageContext context,
    StreamContentProvider streamProvider,
    Encoding defaultEncoding,
    string path)
  {
    this._context = context;
    this._streamProvider = streamProvider;
    this._defaultEncoding = defaultEncoding;
    this._path = path;
  }

  public override SourceCodeReader GetReader()
  {
    return this._context.GetSourceReader(this._streamProvider.GetStream(), this._defaultEncoding, this._path);
  }
}
