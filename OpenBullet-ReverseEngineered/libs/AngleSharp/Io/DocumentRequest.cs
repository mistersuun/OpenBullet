// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.DocumentRequest
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace AngleSharp.Io;

public class DocumentRequest
{
  public DocumentRequest(Url target)
  {
    this.Target = target ?? throw new ArgumentNullException(nameof (target));
    this.Headers = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    this.Method = HttpMethod.Get;
    this.Body = Stream.Null;
  }

  public static DocumentRequest Get(Url target, INode source = null, string referer = null)
  {
    return new DocumentRequest(target)
    {
      Method = HttpMethod.Get,
      Referer = referer,
      Source = source
    };
  }

  public static DocumentRequest Post(
    Url target,
    Stream body,
    string type,
    INode source = null,
    string referer = null)
  {
    DocumentRequest documentRequest = new DocumentRequest(target);
    documentRequest.Method = HttpMethod.Post;
    documentRequest.Body = body ?? throw new ArgumentNullException(nameof (body));
    documentRequest.MimeType = type ?? throw new ArgumentNullException(nameof (type));
    documentRequest.Referer = referer;
    documentRequest.Source = source;
    return documentRequest;
  }

  public static DocumentRequest PostAsPlaintext(Url target, IDictionary<string, string> fields)
  {
    FormDataSet formDataSet = new FormDataSet();
    fields = fields ?? throw new ArgumentNullException(nameof (fields));
    foreach (KeyValuePair<string, string> field in (IEnumerable<KeyValuePair<string, string>>) fields)
      formDataSet.Append(field.Key, field.Value, InputTypeNames.Text);
    return DocumentRequest.Post(target, formDataSet.AsPlaintext(), MimeTypeNames.Plain);
  }

  public static DocumentRequest PostAsUrlencoded(Url target, IDictionary<string, string> fields)
  {
    FormDataSet formDataSet = new FormDataSet();
    fields = fields ?? throw new ArgumentNullException(nameof (fields));
    foreach (KeyValuePair<string, string> field in (IEnumerable<KeyValuePair<string, string>>) fields)
      formDataSet.Append(field.Key, field.Value, InputTypeNames.Text);
    return DocumentRequest.Post(target, formDataSet.AsUrlEncoded(), MimeTypeNames.UrlencodedForm);
  }

  public INode Source { get; set; }

  public Url Target { get; }

  public string Referer
  {
    get => this.GetHeader(HeaderNames.Referer);
    set => this.SetHeader(HeaderNames.Referer, value);
  }

  public HttpMethod Method { get; set; }

  public Stream Body { get; set; }

  public string MimeType
  {
    get => this.GetHeader(HeaderNames.ContentType);
    set => this.SetHeader(HeaderNames.ContentType, value);
  }

  public Dictionary<string, string> Headers { get; }

  private void SetHeader(string name, string value) => this.Headers[name] = value;

  private string GetHeader(string name)
  {
    string header;
    this.Headers.TryGetValue(name, out header);
    return header;
  }
}
