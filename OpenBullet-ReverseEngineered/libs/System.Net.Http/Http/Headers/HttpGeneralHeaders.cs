// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpGeneralHeaders
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

internal sealed class HttpGeneralHeaders
{
  private HttpHeaderValueCollection<string> _connection;
  private HttpHeaderValueCollection<string> _trailer;
  private HttpHeaderValueCollection<TransferCodingHeaderValue> _transferEncoding;
  private HttpHeaderValueCollection<ProductHeaderValue> _upgrade;
  private HttpHeaderValueCollection<ViaHeaderValue> _via;
  private HttpHeaderValueCollection<WarningHeaderValue> _warning;
  private HttpHeaderValueCollection<NameValueHeaderValue> _pragma;
  private HttpHeaders _parent;
  private bool _transferEncodingChunkedSet;
  private bool _connectionCloseSet;

  public CacheControlHeaderValue CacheControl
  {
    get => (CacheControlHeaderValue) this._parent.GetParsedValues("Cache-Control");
    set => this._parent.SetOrRemoveParsedValue("Cache-Control", (object) value);
  }

  public HttpHeaderValueCollection<string> Connection => this.ConnectionCore;

  public bool? ConnectionClose
  {
    get
    {
      if (this.ConnectionCore.IsSpecialValueSet)
        return new bool?(true);
      return this._connectionCloseSet ? new bool?(false) : new bool?();
    }
    set
    {
      bool? nullable = value;
      bool flag = true;
      if ((nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) != 0)
      {
        this._connectionCloseSet = true;
        this.ConnectionCore.SetSpecialValue();
      }
      else
      {
        this._connectionCloseSet = value.HasValue;
        this.ConnectionCore.RemoveSpecialValue();
      }
    }
  }

  public DateTimeOffset? Date
  {
    get => HeaderUtilities.GetDateTimeOffsetValue(nameof (Date), this._parent);
    set => this._parent.SetOrRemoveParsedValue(nameof (Date), (object) value);
  }

  public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
  {
    get
    {
      if (this._pragma == null)
        this._pragma = new HttpHeaderValueCollection<NameValueHeaderValue>(nameof (Pragma), this._parent);
      return this._pragma;
    }
  }

  public HttpHeaderValueCollection<string> Trailer
  {
    get
    {
      if (this._trailer == null)
        this._trailer = new HttpHeaderValueCollection<string>(nameof (Trailer), this._parent, HeaderUtilities.TokenValidator);
      return this._trailer;
    }
  }

  public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
  {
    get => this.TransferEncodingCore;
  }

  public bool? TransferEncodingChunked
  {
    get
    {
      if (this.TransferEncodingCore.IsSpecialValueSet)
        return new bool?(true);
      return this._transferEncodingChunkedSet ? new bool?(false) : new bool?();
    }
    set
    {
      bool? nullable = value;
      bool flag = true;
      if ((nullable.GetValueOrDefault() == flag ? (nullable.HasValue ? 1 : 0) : 0) != 0)
      {
        this._transferEncodingChunkedSet = true;
        this.TransferEncodingCore.SetSpecialValue();
      }
      else
      {
        this._transferEncodingChunkedSet = value.HasValue;
        this.TransferEncodingCore.RemoveSpecialValue();
      }
    }
  }

  public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
  {
    get
    {
      if (this._upgrade == null)
        this._upgrade = new HttpHeaderValueCollection<ProductHeaderValue>(nameof (Upgrade), this._parent);
      return this._upgrade;
    }
  }

  public HttpHeaderValueCollection<ViaHeaderValue> Via
  {
    get
    {
      if (this._via == null)
        this._via = new HttpHeaderValueCollection<ViaHeaderValue>(nameof (Via), this._parent);
      return this._via;
    }
  }

  public HttpHeaderValueCollection<WarningHeaderValue> Warning
  {
    get
    {
      if (this._warning == null)
        this._warning = new HttpHeaderValueCollection<WarningHeaderValue>(nameof (Warning), this._parent);
      return this._warning;
    }
  }

  private HttpHeaderValueCollection<string> ConnectionCore
  {
    get
    {
      if (this._connection == null)
        this._connection = new HttpHeaderValueCollection<string>("Connection", this._parent, "close", HeaderUtilities.TokenValidator);
      return this._connection;
    }
  }

  private HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncodingCore
  {
    get
    {
      if (this._transferEncoding == null)
        this._transferEncoding = new HttpHeaderValueCollection<TransferCodingHeaderValue>("Transfer-Encoding", this._parent, HeaderUtilities.TransferEncodingChunked);
      return this._transferEncoding;
    }
  }

  internal HttpGeneralHeaders(HttpHeaders parent) => this._parent = parent;

  internal static void AddParsers(Dictionary<string, HttpHeaderParser> parserStore)
  {
    parserStore.Add("Cache-Control", (HttpHeaderParser) CacheControlHeaderParser.Parser);
    parserStore.Add("Connection", GenericHeaderParser.TokenListParser);
    parserStore.Add("Date", (HttpHeaderParser) DateHeaderParser.Parser);
    parserStore.Add("Pragma", GenericHeaderParser.MultipleValueNameValueParser);
    parserStore.Add("Trailer", GenericHeaderParser.TokenListParser);
    parserStore.Add("Transfer-Encoding", (HttpHeaderParser) TransferCodingHeaderParser.MultipleValueParser);
    parserStore.Add("Upgrade", GenericHeaderParser.MultipleValueProductParser);
    parserStore.Add("Via", GenericHeaderParser.MultipleValueViaParser);
    parserStore.Add("Warning", GenericHeaderParser.MultipleValueWarningParser);
  }

  internal static void AddKnownHeaders(HashSet<string> headerSet)
  {
    headerSet.Add("Cache-Control");
    headerSet.Add("Connection");
    headerSet.Add("Date");
    headerSet.Add("Pragma");
    headerSet.Add("Trailer");
    headerSet.Add("Transfer-Encoding");
    headerSet.Add("Upgrade");
    headerSet.Add("Via");
    headerSet.Add("Warning");
  }

  internal void AddSpecialsFrom(HttpGeneralHeaders sourceHeaders)
  {
    if (!this.TransferEncodingChunked.HasValue)
      this.TransferEncodingChunked = sourceHeaders.TransferEncodingChunked;
    if (this.ConnectionClose.HasValue)
      return;
    this.ConnectionClose = sourceHeaders.ConnectionClose;
  }
}
