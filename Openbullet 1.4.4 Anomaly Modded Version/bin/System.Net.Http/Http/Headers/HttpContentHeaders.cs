// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpContentHeaders
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

public sealed class HttpContentHeaders : HttpHeaders
{
  private static readonly Dictionary<string, HttpHeaderParser> s_parserStore = HttpContentHeaders.CreateParserStore();
  private static readonly HashSet<string> s_invalidHeaders = HttpContentHeaders.CreateInvalidHeaders();
  private Func<long?> _calculateLengthFunc;
  private bool _contentLengthSet;
  private HttpHeaderValueCollection<string> _allow;
  private HttpHeaderValueCollection<string> _contentEncoding;
  private HttpHeaderValueCollection<string> _contentLanguage;

  public ICollection<string> Allow
  {
    get
    {
      if (this._allow == null)
        this._allow = new HttpHeaderValueCollection<string>(nameof (Allow), (HttpHeaders) this, HeaderUtilities.TokenValidator);
      return (ICollection<string>) this._allow;
    }
  }

  public ContentDispositionHeaderValue ContentDisposition
  {
    get => (ContentDispositionHeaderValue) this.GetParsedValues("Content-Disposition");
    set => this.SetOrRemoveParsedValue("Content-Disposition", (object) value);
  }

  public ICollection<string> ContentEncoding
  {
    get
    {
      if (this._contentEncoding == null)
        this._contentEncoding = new HttpHeaderValueCollection<string>("Content-Encoding", (HttpHeaders) this, HeaderUtilities.TokenValidator);
      return (ICollection<string>) this._contentEncoding;
    }
  }

  public ICollection<string> ContentLanguage
  {
    get
    {
      if (this._contentLanguage == null)
        this._contentLanguage = new HttpHeaderValueCollection<string>("Content-Language", (HttpHeaders) this, HeaderUtilities.TokenValidator);
      return (ICollection<string>) this._contentLanguage;
    }
  }

  public long? ContentLength
  {
    get
    {
      object parsedValues = this.GetParsedValues("Content-Length");
      if (!this._contentLengthSet && parsedValues == null)
      {
        long? contentLength = this._calculateLengthFunc();
        if (contentLength.HasValue)
          this.SetParsedValue("Content-Length", (object) contentLength.Value);
        return contentLength;
      }
      return parsedValues == null ? new long?() : new long?((long) parsedValues);
    }
    set
    {
      this.SetOrRemoveParsedValue("Content-Length", (object) value);
      this._contentLengthSet = true;
    }
  }

  public Uri ContentLocation
  {
    get => (Uri) this.GetParsedValues("Content-Location");
    set => this.SetOrRemoveParsedValue("Content-Location", (object) value);
  }

  public byte[] ContentMD5
  {
    get => (byte[]) this.GetParsedValues("Content-MD5");
    set => this.SetOrRemoveParsedValue("Content-MD5", (object) value);
  }

  public ContentRangeHeaderValue ContentRange
  {
    get => (ContentRangeHeaderValue) this.GetParsedValues("Content-Range");
    set => this.SetOrRemoveParsedValue("Content-Range", (object) value);
  }

  public MediaTypeHeaderValue ContentType
  {
    get => (MediaTypeHeaderValue) this.GetParsedValues("Content-Type");
    set => this.SetOrRemoveParsedValue("Content-Type", (object) value);
  }

  public DateTimeOffset? Expires
  {
    get => HeaderUtilities.GetDateTimeOffsetValue(nameof (Expires), (HttpHeaders) this);
    set => this.SetOrRemoveParsedValue(nameof (Expires), (object) value);
  }

  public DateTimeOffset? LastModified
  {
    get => HeaderUtilities.GetDateTimeOffsetValue("Last-Modified", (HttpHeaders) this);
    set => this.SetOrRemoveParsedValue("Last-Modified", (object) value);
  }

  internal HttpContentHeaders(Func<long?> calculateLengthFunc)
  {
    this._calculateLengthFunc = calculateLengthFunc;
    this.SetConfiguration(HttpContentHeaders.s_parserStore, HttpContentHeaders.s_invalidHeaders);
  }

  private static Dictionary<string, HttpHeaderParser> CreateParserStore()
  {
    return new Dictionary<string, HttpHeaderParser>(11, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Allow",
        GenericHeaderParser.TokenListParser
      },
      {
        "Content-Disposition",
        GenericHeaderParser.ContentDispositionParser
      },
      {
        "Content-Encoding",
        GenericHeaderParser.TokenListParser
      },
      {
        "Content-Language",
        GenericHeaderParser.TokenListParser
      },
      {
        "Content-Length",
        (HttpHeaderParser) Int64NumberHeaderParser.Parser
      },
      {
        "Content-Location",
        (HttpHeaderParser) UriHeaderParser.RelativeOrAbsoluteUriParser
      },
      {
        "Content-MD5",
        (HttpHeaderParser) ByteArrayHeaderParser.Parser
      },
      {
        "Content-Range",
        GenericHeaderParser.ContentRangeParser
      },
      {
        "Content-Type",
        (HttpHeaderParser) MediaTypeHeaderParser.SingleValueParser
      },
      {
        "Expires",
        (HttpHeaderParser) DateHeaderParser.Parser
      },
      {
        "Last-Modified",
        (HttpHeaderParser) DateHeaderParser.Parser
      }
    };
  }

  private static HashSet<string> CreateInvalidHeaders()
  {
    HashSet<string> headerSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    HttpRequestHeaders.AddKnownHeaders(headerSet);
    HttpResponseHeaders.AddKnownHeaders(headerSet);
    HttpGeneralHeaders.AddKnownHeaders(headerSet);
    return headerSet;
  }

  internal static void AddKnownHeaders(HashSet<string> headerSet)
  {
    headerSet.Add("Allow");
    headerSet.Add("Content-Disposition");
    headerSet.Add("Content-Encoding");
    headerSet.Add("Content-Language");
    headerSet.Add("Content-Length");
    headerSet.Add("Content-Location");
    headerSet.Add("Content-MD5");
    headerSet.Add("Content-Range");
    headerSet.Add("Content-Type");
    headerSet.Add("Expires");
    headerSet.Add("Last-Modified");
  }
}
