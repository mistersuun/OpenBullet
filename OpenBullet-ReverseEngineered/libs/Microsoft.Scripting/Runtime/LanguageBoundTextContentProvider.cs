// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.LanguageBoundTextContentProvider
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System.IO;
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
    Stream stream = this._streamProvider.GetStream();
    try
    {
      return this._context.GetSourceReader(stream, this._defaultEncoding, this._path);
    }
    catch
    {
      stream.Dispose();
      throw;
    }
  }
}
