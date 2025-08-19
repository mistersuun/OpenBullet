// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.CreateDocumentOptions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io;
using AngleSharp.Text;
using System.Text;

#nullable disable
namespace AngleSharp.Dom;

public sealed class CreateDocumentOptions
{
  private readonly IResponse _response;
  private readonly MimeType _contentType;
  private readonly TextSource _source;
  private readonly IDocument _ancestor;

  public CreateDocumentOptions(IResponse response, Encoding encoding = null, IDocument ancestor = null)
  {
    MimeType contentType = response.GetContentType(MimeTypeNames.Html);
    string parameter = contentType.GetParameter(AttributeNames.Charset);
    Encoding encoding1 = encoding ?? Encoding.UTF8;
    TextSource textSource = new TextSource(response.Content, encoding1);
    if (!string.IsNullOrEmpty(parameter) && TextEncoding.IsSupported(parameter))
      textSource.CurrentEncoding = TextEncoding.Resolve(parameter);
    this._source = textSource;
    this._contentType = contentType;
    this._response = response;
    this._ancestor = ancestor;
  }

  public IResponse Response => this._response;

  public MimeType ContentType => this._contentType;

  public TextSource Source => this._source;

  public IDocument ImportAncestor => this._ancestor;
}
