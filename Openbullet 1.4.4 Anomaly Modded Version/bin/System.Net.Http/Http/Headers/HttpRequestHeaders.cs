// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpRequestHeaders
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

public sealed class HttpRequestHeaders : HttpHeaders
{
  private static readonly Dictionary<string, HttpHeaderParser> s_parserStore = HttpRequestHeaders.CreateParserStore();
  private static readonly HashSet<string> s_invalidHeaders = HttpRequestHeaders.CreateInvalidHeaders();
  private HttpGeneralHeaders _generalHeaders;
  private HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> _accept;
  private HttpHeaderValueCollection<NameValueWithParametersHeaderValue> _expect;
  private bool _expectContinueSet;
  private HttpHeaderValueCollection<EntityTagHeaderValue> _ifMatch;
  private HttpHeaderValueCollection<EntityTagHeaderValue> _ifNoneMatch;
  private HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> _te;
  private HttpHeaderValueCollection<ProductInfoHeaderValue> _userAgent;
  private HttpHeaderValueCollection<StringWithQualityHeaderValue> _acceptCharset;
  private HttpHeaderValueCollection<StringWithQualityHeaderValue> _acceptEncoding;
  private HttpHeaderValueCollection<StringWithQualityHeaderValue> _acceptLanguage;

  public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept
  {
    get
    {
      if (this._accept == null)
        this._accept = new HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>(nameof (Accept), (HttpHeaders) this);
      return this._accept;
    }
  }

  public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset
  {
    get
    {
      if (this._acceptCharset == null)
        this._acceptCharset = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Charset", (HttpHeaders) this);
      return this._acceptCharset;
    }
  }

  public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding
  {
    get
    {
      if (this._acceptEncoding == null)
        this._acceptEncoding = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Encoding", (HttpHeaders) this);
      return this._acceptEncoding;
    }
  }

  public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage
  {
    get
    {
      if (this._acceptLanguage == null)
        this._acceptLanguage = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Language", (HttpHeaders) this);
      return this._acceptLanguage;
    }
  }

  public AuthenticationHeaderValue Authorization
  {
    get => (AuthenticationHeaderValue) this.GetParsedValues(nameof (Authorization));
    set => this.SetOrRemoveParsedValue(nameof (Authorization), (object) value);
  }

  public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect => this.ExpectCore;

  public bool? ExpectContinue
  {
    get
    {
      if (this.ExpectCore.IsSpecialValueSet)
        return new bool?(true);
      return this._expectContinueSet ? new bool?(false) : new bool?();
    }
    set
    {
      bool? nullable = value;
      bool flag = true;
      if ((nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) != 0)
      {
        this._expectContinueSet = true;
        this.ExpectCore.SetSpecialValue();
      }
      else
      {
        this._expectContinueSet = value.HasValue;
        this.ExpectCore.RemoveSpecialValue();
      }
    }
  }

  public string From
  {
    get => (string) this.GetParsedValues(nameof (From));
    set
    {
      if (value == string.Empty)
        value = (string) null;
      if (value != null && !HeaderUtilities.IsValidEmailAddress(value))
        throw new FormatException(SR.net_http_headers_invalid_from_header);
      this.SetOrRemoveParsedValue(nameof (From), (object) value);
    }
  }

  public string Host
  {
    get => (string) this.GetParsedValues(nameof (Host));
    set
    {
      if (value == string.Empty)
        value = (string) null;
      string host = (string) null;
      if (value != null && HttpRuleParser.GetHostLength(value, 0, false, out host) != value.Length)
        throw new FormatException(SR.net_http_headers_invalid_host_header);
      this.SetOrRemoveParsedValue(nameof (Host), (object) value);
    }
  }

  public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch
  {
    get
    {
      if (this._ifMatch == null)
        this._ifMatch = new HttpHeaderValueCollection<EntityTagHeaderValue>("If-Match", (HttpHeaders) this);
      return this._ifMatch;
    }
  }

  public DateTimeOffset? IfModifiedSince
  {
    get => HeaderUtilities.GetDateTimeOffsetValue("If-Modified-Since", (HttpHeaders) this);
    set => this.SetOrRemoveParsedValue("If-Modified-Since", (object) value);
  }

  public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch
  {
    get
    {
      if (this._ifNoneMatch == null)
        this._ifNoneMatch = new HttpHeaderValueCollection<EntityTagHeaderValue>("If-None-Match", (HttpHeaders) this);
      return this._ifNoneMatch;
    }
  }

  public RangeConditionHeaderValue IfRange
  {
    get => (RangeConditionHeaderValue) this.GetParsedValues("If-Range");
    set => this.SetOrRemoveParsedValue("If-Range", (object) value);
  }

  public DateTimeOffset? IfUnmodifiedSince
  {
    get => HeaderUtilities.GetDateTimeOffsetValue("If-Unmodified-Since", (HttpHeaders) this);
    set => this.SetOrRemoveParsedValue("If-Unmodified-Since", (object) value);
  }

  public int? MaxForwards
  {
    get
    {
      object parsedValues = this.GetParsedValues("Max-Forwards");
      return parsedValues != null ? new int?((int) parsedValues) : new int?();
    }
    set => this.SetOrRemoveParsedValue("Max-Forwards", (object) value);
  }

  public AuthenticationHeaderValue ProxyAuthorization
  {
    get => (AuthenticationHeaderValue) this.GetParsedValues("Proxy-Authorization");
    set => this.SetOrRemoveParsedValue("Proxy-Authorization", (object) value);
  }

  public RangeHeaderValue Range
  {
    get => (RangeHeaderValue) this.GetParsedValues(nameof (Range));
    set => this.SetOrRemoveParsedValue(nameof (Range), (object) value);
  }

  public Uri Referrer
  {
    get => (Uri) this.GetParsedValues("Referer");
    set => this.SetOrRemoveParsedValue("Referer", (object) value);
  }

  public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE
  {
    get
    {
      if (this._te == null)
        this._te = new HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue>(nameof (TE), (HttpHeaders) this);
      return this._te;
    }
  }

  public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent
  {
    get
    {
      if (this._userAgent == null)
        this._userAgent = new HttpHeaderValueCollection<ProductInfoHeaderValue>("User-Agent", (HttpHeaders) this);
      return this._userAgent;
    }
  }

  private HttpHeaderValueCollection<NameValueWithParametersHeaderValue> ExpectCore
  {
    get
    {
      if (this._expect == null)
        this._expect = new HttpHeaderValueCollection<NameValueWithParametersHeaderValue>("Expect", (HttpHeaders) this, HeaderUtilities.ExpectContinue);
      return this._expect;
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

  internal HttpRequestHeaders()
  {
    this._generalHeaders = new HttpGeneralHeaders((HttpHeaders) this);
    this.SetConfiguration(HttpRequestHeaders.s_parserStore, HttpRequestHeaders.s_invalidHeaders);
  }

  private static Dictionary<string, HttpHeaderParser> CreateParserStore()
  {
    Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    parserStore.Add("Accept", (HttpHeaderParser) MediaTypeHeaderParser.MultipleValuesParser);
    parserStore.Add("Accept-Charset", GenericHeaderParser.MultipleValueStringWithQualityParser);
    parserStore.Add("Accept-Encoding", GenericHeaderParser.MultipleValueStringWithQualityParser);
    parserStore.Add("Accept-Language", GenericHeaderParser.MultipleValueStringWithQualityParser);
    parserStore.Add("Authorization", GenericHeaderParser.SingleValueAuthenticationParser);
    parserStore.Add("Expect", GenericHeaderParser.MultipleValueNameValueWithParametersParser);
    parserStore.Add("From", GenericHeaderParser.MailAddressParser);
    parserStore.Add("Host", GenericHeaderParser.HostParser);
    parserStore.Add("If-Match", GenericHeaderParser.MultipleValueEntityTagParser);
    parserStore.Add("If-Modified-Since", (HttpHeaderParser) DateHeaderParser.Parser);
    parserStore.Add("If-None-Match", GenericHeaderParser.MultipleValueEntityTagParser);
    parserStore.Add("If-Range", GenericHeaderParser.RangeConditionParser);
    parserStore.Add("If-Unmodified-Since", (HttpHeaderParser) DateHeaderParser.Parser);
    parserStore.Add("Max-Forwards", (HttpHeaderParser) Int32NumberHeaderParser.Parser);
    parserStore.Add("Proxy-Authorization", GenericHeaderParser.SingleValueAuthenticationParser);
    parserStore.Add("Range", GenericHeaderParser.RangeParser);
    parserStore.Add("Referer", (HttpHeaderParser) UriHeaderParser.RelativeOrAbsoluteUriParser);
    parserStore.Add("TE", (HttpHeaderParser) TransferCodingHeaderParser.MultipleValueWithQualityParser);
    parserStore.Add("User-Agent", (HttpHeaderParser) ProductInfoHeaderParser.MultipleValueParser);
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
    headerSet.Add("Accept");
    headerSet.Add("Accept-Charset");
    headerSet.Add("Accept-Encoding");
    headerSet.Add("Accept-Language");
    headerSet.Add("Authorization");
    headerSet.Add("Expect");
    headerSet.Add("From");
    headerSet.Add("Host");
    headerSet.Add("If-Match");
    headerSet.Add("If-Modified-Since");
    headerSet.Add("If-None-Match");
    headerSet.Add("If-Range");
    headerSet.Add("If-Unmodified-Since");
    headerSet.Add("Max-Forwards");
    headerSet.Add("Proxy-Authorization");
    headerSet.Add("Range");
    headerSet.Add("Referer");
    headerSet.Add("TE");
    headerSet.Add("User-Agent");
  }

  internal override void AddHeaders(HttpHeaders sourceHeaders)
  {
    base.AddHeaders(sourceHeaders);
    HttpRequestHeaders httpRequestHeaders = sourceHeaders as HttpRequestHeaders;
    this._generalHeaders.AddSpecialsFrom(httpRequestHeaders._generalHeaders);
    if (this.ExpectContinue.HasValue)
      return;
    this.ExpectContinue = httpRequestHeaders.ExpectContinue;
  }
}
