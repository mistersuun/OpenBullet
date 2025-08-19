// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpResponseHeaders
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

public sealed class HttpResponseHeaders : HttpHeaders
{
  private static readonly Dictionary<string, HttpHeaderParser> s_parserStore = HttpResponseHeaders.CreateParserStore();
  private static readonly HashSet<string> s_invalidHeaders = HttpResponseHeaders.CreateInvalidHeaders();
  private HttpGeneralHeaders _generalHeaders;
  private HttpHeaderValueCollection<string> _acceptRanges;
  private HttpHeaderValueCollection<AuthenticationHeaderValue> _wwwAuthenticate;
  private HttpHeaderValueCollection<AuthenticationHeaderValue> _proxyAuthenticate;
  private HttpHeaderValueCollection<ProductInfoHeaderValue> _server;
  private HttpHeaderValueCollection<string> _vary;

  public HttpHeaderValueCollection<string> AcceptRanges
  {
    get
    {
      if (this._acceptRanges == null)
        this._acceptRanges = new HttpHeaderValueCollection<string>("Accept-Ranges", (HttpHeaders) this, HeaderUtilities.TokenValidator);
      return this._acceptRanges;
    }
  }

  public TimeSpan? Age
  {
    get => HeaderUtilities.GetTimeSpanValue(nameof (Age), (HttpHeaders) this);
    set => this.SetOrRemoveParsedValue(nameof (Age), (object) value);
  }

  public EntityTagHeaderValue ETag
  {
    get => (EntityTagHeaderValue) this.GetParsedValues(nameof (ETag));
    set => this.SetOrRemoveParsedValue(nameof (ETag), (object) value);
  }

  public Uri Location
  {
    get => (Uri) this.GetParsedValues(nameof (Location));
    set => this.SetOrRemoveParsedValue(nameof (Location), (object) value);
  }

  public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate
  {
    get
    {
      if (this._proxyAuthenticate == null)
        this._proxyAuthenticate = new HttpHeaderValueCollection<AuthenticationHeaderValue>("Proxy-Authenticate", (HttpHeaders) this);
      return this._proxyAuthenticate;
    }
  }

  public RetryConditionHeaderValue RetryAfter
  {
    get => (RetryConditionHeaderValue) this.GetParsedValues("Retry-After");
    set => this.SetOrRemoveParsedValue("Retry-After", (object) value);
  }

  public HttpHeaderValueCollection<ProductInfoHeaderValue> Server
  {
    get
    {
      if (this._server == null)
        this._server = new HttpHeaderValueCollection<ProductInfoHeaderValue>(nameof (Server), (HttpHeaders) this);
      return this._server;
    }
  }

  public HttpHeaderValueCollection<string> Vary
  {
    get
    {
      if (this._vary == null)
        this._vary = new HttpHeaderValueCollection<string>(nameof (Vary), (HttpHeaders) this, HeaderUtilities.TokenValidator);
      return this._vary;
    }
  }

  public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate
  {
    get
    {
      if (this._wwwAuthenticate == null)
        this._wwwAuthenticate = new HttpHeaderValueCollection<AuthenticationHeaderValue>("WWW-Authenticate", (HttpHeaders) this);
      return this._wwwAuthenticate;
    }
  }

  public CacheControlHeaderValue CacheControl
  {
    get => this._generalHeaders.CacheControl;
    set => this._generalHeaders.CacheControl = value;
  }

  public HttpHeaderValueCollection<string> Connection => this._generalHeaders.Connection;

  public bool? ConnectionClose
  {
    get => this._generalHeaders.ConnectionClose;
    set => this._generalHeaders.ConnectionClose = value;
  }

  public DateTimeOffset? Date
  {
    get => this._generalHeaders.Date;
    set => this._generalHeaders.Date = value;
  }

  public HttpHeaderValueCollection<NameValueHeaderValue> Pragma => this._generalHeaders.Pragma;

  public HttpHeaderValueCollection<string> Trailer => this._generalHeaders.Trailer;

  public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
  {
    get => this._generalHeaders.TransferEncoding;
  }

  public bool? TransferEncodingChunked
  {
    get => this._generalHeaders.TransferEncodingChunked;
    set => this._generalHeaders.TransferEncodingChunked = value;
  }

  public HttpHeaderValueCollection<ProductHeaderValue> Upgrade => this._generalHeaders.Upgrade;

  public HttpHeaderValueCollection<ViaHeaderValue> Via => this._generalHeaders.Via;

  public HttpHeaderValueCollection<WarningHeaderValue> Warning => this._generalHeaders.Warning;

  internal HttpResponseHeaders()
  {
    this._generalHeaders = new HttpGeneralHeaders((HttpHeaders) this);
    this.SetConfiguration(HttpResponseHeaders.s_parserStore, HttpResponseHeaders.s_invalidHeaders);
  }

  private static Dictionary<string, HttpHeaderParser> CreateParserStore()
  {
    Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    parserStore.Add("Accept-Ranges", GenericHeaderParser.TokenListParser);
    parserStore.Add("Age", (HttpHeaderParser) TimeSpanHeaderParser.Parser);
    parserStore.Add("ETag", GenericHeaderParser.SingleValueEntityTagParser);
    parserStore.Add("Location", (HttpHeaderParser) UriHeaderParser.RelativeOrAbsoluteUriParser);
    parserStore.Add("Proxy-Authenticate", GenericHeaderParser.MultipleValueAuthenticationParser);
    parserStore.Add("Retry-After", GenericHeaderParser.RetryConditionParser);
    parserStore.Add("Server", (HttpHeaderParser) ProductInfoHeaderParser.MultipleValueParser);
    parserStore.Add("Vary", GenericHeaderParser.TokenListParser);
    parserStore.Add("WWW-Authenticate", GenericHeaderParser.MultipleValueAuthenticationParser);
    HttpGeneralHeaders.AddParsers(parserStore);
    return parserStore;
  }

  private static HashSet<string> CreateInvalidHeaders()
  {
    HashSet<string> headerSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    HttpContentHeaders.AddKnownHeaders(headerSet);
    return headerSet;
  }

  internal static void AddKnownHeaders(HashSet<string> headerSet)
  {
    headerSet.Add("Accept-Ranges");
    headerSet.Add("Age");
    headerSet.Add("ETag");
    headerSet.Add("Location");
    headerSet.Add("Proxy-Authenticate");
    headerSet.Add("Retry-After");
    headerSet.Add("Server");
    headerSet.Add("Vary");
    headerSet.Add("WWW-Authenticate");
  }

  internal override void AddHeaders(HttpHeaders sourceHeaders)
  {
    base.AddHeaders(sourceHeaders);
    this._generalHeaders.AddSpecialsFrom((sourceHeaders as HttpResponseHeaders)._generalHeaders);
  }
}
