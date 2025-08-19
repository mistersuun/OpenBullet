// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.CacheControlHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public class CacheControlHeaderValue : ICloneable
{
  private const string maxAgeString = "max-age";
  private const string maxStaleString = "max-stale";
  private const string minFreshString = "min-fresh";
  private const string mustRevalidateString = "must-revalidate";
  private const string noCacheString = "no-cache";
  private const string noStoreString = "no-store";
  private const string noTransformString = "no-transform";
  private const string onlyIfCachedString = "only-if-cached";
  private const string privateString = "private";
  private const string proxyRevalidateString = "proxy-revalidate";
  private const string publicString = "public";
  private const string sharedMaxAgeString = "s-maxage";
  private static readonly HttpHeaderParser s_nameValueListParser = GenericHeaderParser.MultipleValueNameValueParser;
  private static readonly Action<string> s_checkIsValidToken = new Action<string>(CacheControlHeaderValue.CheckIsValidToken);
  private bool _noCache;
  private ObjectCollection<string> _noCacheHeaders;
  private bool _noStore;
  private TimeSpan? _maxAge;
  private TimeSpan? _sharedMaxAge;
  private bool _maxStale;
  private TimeSpan? _maxStaleLimit;
  private TimeSpan? _minFresh;
  private bool _noTransform;
  private bool _onlyIfCached;
  private bool _publicField;
  private bool _privateField;
  private ObjectCollection<string> _privateHeaders;
  private bool _mustRevalidate;
  private bool _proxyRevalidate;
  private ObjectCollection<NameValueHeaderValue> _extensions;

  public bool NoCache
  {
    get => this._noCache;
    set => this._noCache = value;
  }

  public ICollection<string> NoCacheHeaders
  {
    get
    {
      if (this._noCacheHeaders == null)
        this._noCacheHeaders = new ObjectCollection<string>(CacheControlHeaderValue.s_checkIsValidToken);
      return (ICollection<string>) this._noCacheHeaders;
    }
  }

  public bool NoStore
  {
    get => this._noStore;
    set => this._noStore = value;
  }

  public TimeSpan? MaxAge
  {
    get => this._maxAge;
    set => this._maxAge = value;
  }

  public TimeSpan? SharedMaxAge
  {
    get => this._sharedMaxAge;
    set => this._sharedMaxAge = value;
  }

  public bool MaxStale
  {
    get => this._maxStale;
    set => this._maxStale = value;
  }

  public TimeSpan? MaxStaleLimit
  {
    get => this._maxStaleLimit;
    set => this._maxStaleLimit = value;
  }

  public TimeSpan? MinFresh
  {
    get => this._minFresh;
    set => this._minFresh = value;
  }

  public bool NoTransform
  {
    get => this._noTransform;
    set => this._noTransform = value;
  }

  public bool OnlyIfCached
  {
    get => this._onlyIfCached;
    set => this._onlyIfCached = value;
  }

  public bool Public
  {
    get => this._publicField;
    set => this._publicField = value;
  }

  public bool Private
  {
    get => this._privateField;
    set => this._privateField = value;
  }

  public ICollection<string> PrivateHeaders
  {
    get
    {
      if (this._privateHeaders == null)
        this._privateHeaders = new ObjectCollection<string>(CacheControlHeaderValue.s_checkIsValidToken);
      return (ICollection<string>) this._privateHeaders;
    }
  }

  public bool MustRevalidate
  {
    get => this._mustRevalidate;
    set => this._mustRevalidate = value;
  }

  public bool ProxyRevalidate
  {
    get => this._proxyRevalidate;
    set => this._proxyRevalidate = value;
  }

  public ICollection<NameValueHeaderValue> Extensions
  {
    get
    {
      if (this._extensions == null)
        this._extensions = new ObjectCollection<NameValueHeaderValue>();
      return (ICollection<NameValueHeaderValue>) this._extensions;
    }
  }

  public CacheControlHeaderValue()
  {
  }

  private CacheControlHeaderValue(CacheControlHeaderValue source)
  {
    this._noCache = source._noCache;
    this._noStore = source._noStore;
    this._maxAge = source._maxAge;
    this._sharedMaxAge = source._sharedMaxAge;
    this._maxStale = source._maxStale;
    this._maxStaleLimit = source._maxStaleLimit;
    this._minFresh = source._minFresh;
    this._noTransform = source._noTransform;
    this._onlyIfCached = source._onlyIfCached;
    this._publicField = source._publicField;
    this._privateField = source._privateField;
    this._mustRevalidate = source._mustRevalidate;
    this._proxyRevalidate = source._proxyRevalidate;
    if (source._noCacheHeaders != null)
    {
      foreach (string noCacheHeader in source._noCacheHeaders)
        this.NoCacheHeaders.Add(noCacheHeader);
    }
    if (source._privateHeaders != null)
    {
      foreach (string privateHeader in source._privateHeaders)
        this.PrivateHeaders.Add(privateHeader);
    }
    if (source._extensions == null)
      return;
    foreach (ICloneable extension in source._extensions)
      this.Extensions.Add((NameValueHeaderValue) extension.Clone());
  }

  public override string ToString()
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    CacheControlHeaderValue.AppendValueIfRequired(stringBuilder1, this._noStore, "no-store");
    CacheControlHeaderValue.AppendValueIfRequired(stringBuilder1, this._noTransform, "no-transform");
    CacheControlHeaderValue.AppendValueIfRequired(stringBuilder1, this._onlyIfCached, "only-if-cached");
    CacheControlHeaderValue.AppendValueIfRequired(stringBuilder1, this._publicField, "public");
    CacheControlHeaderValue.AppendValueIfRequired(stringBuilder1, this._mustRevalidate, "must-revalidate");
    CacheControlHeaderValue.AppendValueIfRequired(stringBuilder1, this._proxyRevalidate, "proxy-revalidate");
    if (this._noCache)
    {
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder1, "no-cache");
      if (this._noCacheHeaders != null && this._noCacheHeaders.Count > 0)
      {
        stringBuilder1.Append("=\"");
        CacheControlHeaderValue.AppendValues(stringBuilder1, this._noCacheHeaders);
        stringBuilder1.Append('"');
      }
    }
    int totalSeconds;
    if (this._maxAge.HasValue)
    {
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder1, "max-age");
      stringBuilder1.Append('=');
      StringBuilder stringBuilder2 = stringBuilder1;
      totalSeconds = (int) this._maxAge.Value.TotalSeconds;
      string str = totalSeconds.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      stringBuilder2.Append(str);
    }
    if (this._sharedMaxAge.HasValue)
    {
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder1, "s-maxage");
      stringBuilder1.Append('=');
      StringBuilder stringBuilder3 = stringBuilder1;
      totalSeconds = (int) this._sharedMaxAge.Value.TotalSeconds;
      string str = totalSeconds.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      stringBuilder3.Append(str);
    }
    if (this._maxStale)
    {
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder1, "max-stale");
      if (this._maxStaleLimit.HasValue)
      {
        stringBuilder1.Append('=');
        StringBuilder stringBuilder4 = stringBuilder1;
        totalSeconds = (int) this._maxStaleLimit.Value.TotalSeconds;
        string str = totalSeconds.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        stringBuilder4.Append(str);
      }
    }
    if (this._minFresh.HasValue)
    {
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder1, "min-fresh");
      stringBuilder1.Append('=');
      StringBuilder stringBuilder5 = stringBuilder1;
      totalSeconds = (int) this._minFresh.Value.TotalSeconds;
      string str = totalSeconds.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      stringBuilder5.Append(str);
    }
    if (this._privateField)
    {
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder1, "private");
      if (this._privateHeaders != null && this._privateHeaders.Count > 0)
      {
        stringBuilder1.Append("=\"");
        CacheControlHeaderValue.AppendValues(stringBuilder1, this._privateHeaders);
        stringBuilder1.Append('"');
      }
    }
    NameValueHeaderValue.ToString(this._extensions, ',', false, stringBuilder1);
    return stringBuilder1.ToString();
  }

  public override bool Equals(object obj)
  {
    if (!(obj is CacheControlHeaderValue controlHeaderValue) || this._noCache != controlHeaderValue._noCache || this._noStore != controlHeaderValue._noStore)
      return false;
    TimeSpan? nullable1 = this._maxAge;
    TimeSpan? nullable2 = controlHeaderValue._maxAge;
    if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
    {
      nullable2 = this._sharedMaxAge;
      nullable1 = controlHeaderValue._sharedMaxAge;
      if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && this._maxStale == controlHeaderValue._maxStale)
      {
        nullable1 = this._maxStaleLimit;
        nullable2 = controlHeaderValue._maxStaleLimit;
        if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
        {
          nullable2 = this._minFresh;
          nullable1 = controlHeaderValue._minFresh;
          if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && this._noTransform == controlHeaderValue._noTransform && this._onlyIfCached == controlHeaderValue._onlyIfCached && this._publicField == controlHeaderValue._publicField && this._privateField == controlHeaderValue._privateField && this._mustRevalidate == controlHeaderValue._mustRevalidate && this._proxyRevalidate == controlHeaderValue._proxyRevalidate && HeaderUtilities.AreEqualCollections<string>(this._noCacheHeaders, controlHeaderValue._noCacheHeaders, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<string>(this._privateHeaders, controlHeaderValue._privateHeaders, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this._extensions, controlHeaderValue._extensions))
            return true;
        }
      }
    }
    return false;
  }

  public override int GetHashCode()
  {
    int hashCode = this._noCache.GetHashCode() ^ this._noStore.GetHashCode() << 1 ^ this._maxStale.GetHashCode() << 2 ^ this._noTransform.GetHashCode() << 3 ^ this._onlyIfCached.GetHashCode() << 4 ^ this._publicField.GetHashCode() << 5 ^ this._privateField.GetHashCode() << 6 ^ this._mustRevalidate.GetHashCode() << 7 ^ this._proxyRevalidate.GetHashCode() << 8 ^ (this._maxAge.HasValue ? this._maxAge.Value.GetHashCode() ^ 1 : 0) ^ (this._sharedMaxAge.HasValue ? this._sharedMaxAge.Value.GetHashCode() ^ 2 : 0) ^ (this._maxStaleLimit.HasValue ? this._maxStaleLimit.Value.GetHashCode() ^ 4 : 0) ^ (this._minFresh.HasValue ? this._minFresh.Value.GetHashCode() ^ 8 : 0);
    if (this._noCacheHeaders != null && this._noCacheHeaders.Count > 0)
    {
      foreach (string noCacheHeader in this._noCacheHeaders)
        hashCode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(noCacheHeader);
    }
    if (this._privateHeaders != null && this._privateHeaders.Count > 0)
    {
      foreach (string privateHeader in this._privateHeaders)
        hashCode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(privateHeader);
    }
    if (this._extensions != null && this._extensions.Count > 0)
    {
      foreach (NameValueHeaderValue extension in this._extensions)
        hashCode ^= extension.GetHashCode();
    }
    return hashCode;
  }

  public static CacheControlHeaderValue Parse(string input)
  {
    int index = 0;
    return (CacheControlHeaderValue) CacheControlHeaderParser.Parser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out CacheControlHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (CacheControlHeaderValue) null;
    object parsedValue1;
    if (!CacheControlHeaderParser.Parser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (CacheControlHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetCacheControlLength(
    string input,
    int startIndex,
    CacheControlHeaderValue storeValue,
    out CacheControlHeaderValue parsedValue)
  {
    parsedValue = (CacheControlHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int index = startIndex;
    object parsedValue1 = (object) null;
    List<NameValueHeaderValue> nameValueList = new List<NameValueHeaderValue>();
    while (index < input.Length)
    {
      if (!CacheControlHeaderValue.s_nameValueListParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return 0;
      nameValueList.Add(parsedValue1 as NameValueHeaderValue);
    }
    CacheControlHeaderValue cc = storeValue ?? new CacheControlHeaderValue();
    if (!CacheControlHeaderValue.TrySetCacheControlValues(cc, nameValueList))
      return 0;
    if (storeValue == null)
      parsedValue = cc;
    return input.Length - startIndex;
  }

  private static bool TrySetCacheControlValues(
    CacheControlHeaderValue cc,
    List<NameValueHeaderValue> nameValueList)
  {
    foreach (NameValueHeaderValue nameValue in nameValueList)
    {
      bool flag = true;
      switch (nameValue.Name.ToLowerInvariant())
      {
        case "max-age":
          flag = CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc._maxAge);
          break;
        case "max-stale":
          flag = nameValue.Value == null || CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc._maxStaleLimit);
          if (flag)
          {
            cc._maxStale = true;
            break;
          }
          break;
        case "min-fresh":
          flag = CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc._minFresh);
          break;
        case "must-revalidate":
          flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc._mustRevalidate);
          break;
        case "no-cache":
          flag = CacheControlHeaderValue.TrySetOptionalTokenList(nameValue, ref cc._noCache, ref cc._noCacheHeaders);
          break;
        case "no-store":
          flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc._noStore);
          break;
        case "no-transform":
          flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc._noTransform);
          break;
        case "only-if-cached":
          flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc._onlyIfCached);
          break;
        case "private":
          flag = CacheControlHeaderValue.TrySetOptionalTokenList(nameValue, ref cc._privateField, ref cc._privateHeaders);
          break;
        case "proxy-revalidate":
          flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc._proxyRevalidate);
          break;
        case "public":
          flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc._publicField);
          break;
        case "s-maxage":
          flag = CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc._sharedMaxAge);
          break;
        default:
          cc.Extensions.Add(nameValue);
          break;
      }
      if (!flag)
        return false;
    }
    return true;
  }

  private static bool TrySetTokenOnlyValue(NameValueHeaderValue nameValue, ref bool boolField)
  {
    if (nameValue.Value != null)
      return false;
    boolField = true;
    return true;
  }

  private static bool TrySetOptionalTokenList(
    NameValueHeaderValue nameValue,
    ref bool boolField,
    ref ObjectCollection<string> destination)
  {
    if (nameValue.Value == null)
    {
      boolField = true;
      return true;
    }
    string input = nameValue.Value;
    if (input.Length < 3 || input[0] != '"' || input[input.Length - 1] != '"')
      return false;
    int startIndex = 1;
    int num = input.Length - 1;
    bool separatorFound = false;
    int count = destination == null ? 0 : destination.Count;
    int orWhitespaceIndex;
    int tokenLength;
    for (; startIndex < num; startIndex = orWhitespaceIndex + tokenLength)
    {
      orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
      if (orWhitespaceIndex != num)
      {
        tokenLength = HttpRuleParser.GetTokenLength(input, orWhitespaceIndex);
        if (tokenLength == 0)
          return false;
        if (destination == null)
          destination = new ObjectCollection<string>(CacheControlHeaderValue.s_checkIsValidToken);
        destination.Add(input.Substring(orWhitespaceIndex, tokenLength));
      }
      else
        break;
    }
    if (destination == null || destination.Count <= count)
      return false;
    boolField = true;
    return true;
  }

  private static bool TrySetTimeSpan(NameValueHeaderValue nameValue, ref TimeSpan? timeSpan)
  {
    int result;
    if (nameValue.Value == null || !HeaderUtilities.TryParseInt32(nameValue.Value, out result))
      return false;
    timeSpan = new TimeSpan?(new TimeSpan(0, 0, result));
    return true;
  }

  private static void AppendValueIfRequired(StringBuilder sb, bool appendValue, string value)
  {
    if (!appendValue)
      return;
    CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(sb, value);
  }

  private static void AppendValueWithSeparatorIfRequired(StringBuilder sb, string value)
  {
    if (sb.Length > 0)
      sb.Append(", ");
    sb.Append(value);
  }

  private static void AppendValues(StringBuilder sb, ObjectCollection<string> values)
  {
    bool flag = true;
    foreach (string str in values)
    {
      if (flag)
        flag = false;
      else
        sb.Append(", ");
      sb.Append(str);
    }
  }

  private static void CheckIsValidToken(string item)
  {
    HeaderUtilities.CheckValidToken(item, nameof (item));
  }

  object ICloneable.Clone() => (object) new CacheControlHeaderValue(this);
}
