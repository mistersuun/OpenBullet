// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpHeaders
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public abstract class HttpHeaders : 
  IEnumerable<KeyValuePair<string, IEnumerable<string>>>,
  IEnumerable
{
  private Dictionary<string, HttpHeaders.HeaderStoreItemInfo> _headerStore;
  private Dictionary<string, HttpHeaderParser> _parserStore;
  private HashSet<string> _invalidHeaders;

  public void Add(string name, string value)
  {
    this.CheckHeaderName(name);
    HttpHeaders.HeaderStoreItemInfo info;
    bool addToStore;
    this.PrepareHeaderInfoForAdd(name, out info, out addToStore);
    this.ParseAndAddValue(name, info, value);
    if (!addToStore || info.ParsedValue == null)
      return;
    this.AddHeaderToStore(name, info);
  }

  public void Add(string name, IEnumerable<string> values)
  {
    if (values == null)
      throw new ArgumentNullException(nameof (values));
    this.CheckHeaderName(name);
    HttpHeaders.HeaderStoreItemInfo info;
    bool addToStore;
    this.PrepareHeaderInfoForAdd(name, out info, out addToStore);
    try
    {
      foreach (string str in values)
        this.ParseAndAddValue(name, info, str);
    }
    finally
    {
      if (addToStore && info.ParsedValue != null)
        this.AddHeaderToStore(name, info);
    }
  }

  public bool TryAddWithoutValidation(string name, string value)
  {
    if (!this.TryCheckHeaderName(name))
      return false;
    if (value == null)
      value = string.Empty;
    HttpHeaders.AddValue(this.GetOrCreateHeaderInfo(name, false), (object) value, HttpHeaders.StoreLocation.Raw);
    return true;
  }

  public bool TryAddWithoutValidation(string name, IEnumerable<string> values)
  {
    if (values == null)
      throw new ArgumentNullException(nameof (values));
    if (!this.TryCheckHeaderName(name))
      return false;
    HttpHeaders.HeaderStoreItemInfo headerInfo = this.GetOrCreateHeaderInfo(name, false);
    foreach (string str in values)
      HttpHeaders.AddValue(headerInfo, (object) (str ?? string.Empty), HttpHeaders.StoreLocation.Raw);
    return true;
  }

  public void Clear()
  {
    if (this._headerStore == null)
      return;
    this._headerStore.Clear();
  }

  public bool Remove(string name)
  {
    this.CheckHeaderName(name);
    return this._headerStore != null && this._headerStore.Remove(name);
  }

  public IEnumerable<string> GetValues(string name)
  {
    this.CheckHeaderName(name);
    IEnumerable<string> values;
    if (!this.TryGetValues(name, out values))
      throw new InvalidOperationException(SR.net_http_headers_not_found);
    return values;
  }

  public bool TryGetValues(string name, out IEnumerable<string> values)
  {
    if (!this.TryCheckHeaderName(name))
    {
      values = (IEnumerable<string>) null;
      return false;
    }
    if (this._headerStore == null)
    {
      values = (IEnumerable<string>) null;
      return false;
    }
    HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
    if (this.TryGetAndParseHeaderInfo(name, out info))
    {
      values = (IEnumerable<string>) HttpHeaders.GetValuesAsStrings(info);
      return true;
    }
    values = (IEnumerable<string>) null;
    return false;
  }

  public bool Contains(string name)
  {
    this.CheckHeaderName(name);
    if (this._headerStore == null)
      return false;
    HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
    return this.TryGetAndParseHeaderInfo(name, out info);
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in this)
    {
      stringBuilder.Append(keyValuePair.Key);
      stringBuilder.Append(": ");
      stringBuilder.Append(this.GetHeaderString(keyValuePair.Key));
      stringBuilder.Append("\r\n");
    }
    return stringBuilder.ToString();
  }

  internal IEnumerable<KeyValuePair<string, string>> GetHeaderStrings()
  {
    if (this._headerStore != null)
    {
      foreach (KeyValuePair<string, HttpHeaders.HeaderStoreItemInfo> keyValuePair in this._headerStore)
      {
        string headerString = this.GetHeaderString(keyValuePair.Value);
        yield return new KeyValuePair<string, string>(keyValuePair.Key, headerString);
      }
    }
  }

  internal string GetHeaderString(string headerName)
  {
    return this.GetHeaderString(headerName, (object) null);
  }

  internal string GetHeaderString(string headerName, object exclude)
  {
    HttpHeaders.HeaderStoreItemInfo info;
    return !this.TryGetHeaderInfo(headerName, out info) ? string.Empty : this.GetHeaderString(info, exclude);
  }

  private string GetHeaderString(HttpHeaders.HeaderStoreItemInfo info)
  {
    return this.GetHeaderString(info, (object) null);
  }

  private string GetHeaderString(HttpHeaders.HeaderStoreItemInfo info, object exclude)
  {
    string empty = string.Empty;
    string[] valuesAsStrings = HttpHeaders.GetValuesAsStrings(info, exclude);
    string headerString;
    if (valuesAsStrings.Length == 1)
    {
      headerString = valuesAsStrings[0];
    }
    else
    {
      string separator = ", ";
      if (info.Parser != null && info.Parser.SupportsMultipleValues)
        separator = info.Parser.Separator;
      headerString = string.Join(separator, valuesAsStrings);
    }
    return headerString;
  }

  public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
  {
    if (this._headerStore != null)
    {
      List<string> invalidHeaders = (List<string>) null;
      foreach (KeyValuePair<string, HttpHeaders.HeaderStoreItemInfo> keyValuePair in this._headerStore)
      {
        HttpHeaders.HeaderStoreItemInfo info = keyValuePair.Value;
        if (!this.ParseRawHeaderValues(keyValuePair.Key, info, false))
        {
          if (invalidHeaders == null)
            invalidHeaders = new List<string>();
          invalidHeaders.Add(keyValuePair.Key);
        }
        else
        {
          string[] valuesAsStrings = HttpHeaders.GetValuesAsStrings(info);
          yield return new KeyValuePair<string, IEnumerable<string>>(keyValuePair.Key, (IEnumerable<string>) valuesAsStrings);
        }
      }
      if (invalidHeaders != null)
      {
        foreach (string key in invalidHeaders)
          this._headerStore.Remove(key);
      }
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  internal void SetConfiguration(
    Dictionary<string, HttpHeaderParser> parserStore,
    HashSet<string> invalidHeaders)
  {
    this._parserStore = parserStore;
    this._invalidHeaders = invalidHeaders;
  }

  internal void AddParsedValue(string name, object value)
  {
    HttpHeaders.AddValue(this.GetOrCreateHeaderInfo(name, true), value, HttpHeaders.StoreLocation.Parsed);
  }

  internal void SetParsedValue(string name, object value)
  {
    HttpHeaders.HeaderStoreItemInfo headerInfo = this.GetOrCreateHeaderInfo(name, true);
    headerInfo.InvalidValue = (object) null;
    headerInfo.ParsedValue = (object) null;
    headerInfo.RawValue = (object) null;
    HttpHeaders.AddValue(headerInfo, value, HttpHeaders.StoreLocation.Parsed);
  }

  internal void SetOrRemoveParsedValue(string name, object value)
  {
    if (value == null)
      this.Remove(name);
    else
      this.SetParsedValue(name, value);
  }

  internal bool RemoveParsedValue(string name, object value)
  {
    if (this._headerStore == null)
      return false;
    HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
    if (!this.TryGetAndParseHeaderInfo(name, out info))
      return false;
    bool flag = false;
    if (info.ParsedValue == null)
      return false;
    IEqualityComparer comparer = info.Parser.Comparer;
    if (!(info.ParsedValue is List<object> parsedValue))
    {
      if (this.AreEqual(value, info.ParsedValue, comparer))
      {
        info.ParsedValue = (object) null;
        flag = true;
      }
    }
    else
    {
      foreach (object storeValue in parsedValue)
      {
        if (this.AreEqual(value, storeValue, comparer))
        {
          flag = parsedValue.Remove(storeValue);
          break;
        }
      }
      if (parsedValue.Count == 0)
        info.ParsedValue = (object) null;
    }
    if (info.IsEmpty)
      this.Remove(name);
    return flag;
  }

  internal bool ContainsParsedValue(string name, object value)
  {
    if (this._headerStore == null)
      return false;
    HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
    if (!this.TryGetAndParseHeaderInfo(name, out info) || info.ParsedValue == null)
      return false;
    List<object> parsedValue = info.ParsedValue as List<object>;
    IEqualityComparer comparer = info.Parser.Comparer;
    if (parsedValue == null)
      return this.AreEqual(value, info.ParsedValue, comparer);
    foreach (object storeValue in parsedValue)
    {
      if (this.AreEqual(value, storeValue, comparer))
        return true;
    }
    return false;
  }

  internal virtual void AddHeaders(HttpHeaders sourceHeaders)
  {
    if (sourceHeaders._headerStore == null)
      return;
    List<string> stringList = (List<string>) null;
    foreach (KeyValuePair<string, HttpHeaders.HeaderStoreItemInfo> keyValuePair in sourceHeaders._headerStore)
    {
      if (this._headerStore == null || !this._headerStore.ContainsKey(keyValuePair.Key))
      {
        HttpHeaders.HeaderStoreItemInfo headerStoreItemInfo = keyValuePair.Value;
        if (!sourceHeaders.ParseRawHeaderValues(keyValuePair.Key, headerStoreItemInfo, false))
        {
          if (stringList == null)
            stringList = new List<string>();
          stringList.Add(keyValuePair.Key);
        }
        else
          this.AddHeaderInfo(keyValuePair.Key, headerStoreItemInfo);
      }
    }
    if (stringList == null)
      return;
    foreach (string key in stringList)
      sourceHeaders._headerStore.Remove(key);
  }

  private void AddHeaderInfo(string headerName, HttpHeaders.HeaderStoreItemInfo sourceInfo)
  {
    HttpHeaders.HeaderStoreItemInfo addHeaderToStore = this.CreateAndAddHeaderToStore(headerName);
    if (addHeaderToStore.Parser == null)
    {
      addHeaderToStore.ParsedValue = HttpHeaders.CloneStringHeaderInfoValues(sourceInfo.ParsedValue);
    }
    else
    {
      addHeaderToStore.InvalidValue = HttpHeaders.CloneStringHeaderInfoValues(sourceInfo.InvalidValue);
      if (sourceInfo.ParsedValue == null)
        return;
      if (!(sourceInfo.ParsedValue is List<object> parsedValue))
      {
        HttpHeaders.CloneAndAddValue(addHeaderToStore, sourceInfo.ParsedValue);
      }
      else
      {
        foreach (object source in parsedValue)
          HttpHeaders.CloneAndAddValue(addHeaderToStore, source);
      }
    }
  }

  private static void CloneAndAddValue(
    HttpHeaders.HeaderStoreItemInfo destinationInfo,
    object source)
  {
    if (source is ICloneable cloneable)
      HttpHeaders.AddValue(destinationInfo, cloneable.Clone(), HttpHeaders.StoreLocation.Parsed);
    else
      HttpHeaders.AddValue(destinationInfo, source, HttpHeaders.StoreLocation.Parsed);
  }

  private static object CloneStringHeaderInfoValues(object source)
  {
    if (source == null)
      return (object) null;
    return !(source is List<object> collection) ? source : (object) new List<object>((IEnumerable<object>) collection);
  }

  private HttpHeaders.HeaderStoreItemInfo GetOrCreateHeaderInfo(string name, bool parseRawValues)
  {
    HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
    if (!(!parseRawValues ? this.TryGetHeaderInfo(name, out info) : this.TryGetAndParseHeaderInfo(name, out info)))
      info = this.CreateAndAddHeaderToStore(name);
    return info;
  }

  private HttpHeaders.HeaderStoreItemInfo CreateAndAddHeaderToStore(string name)
  {
    HttpHeaders.HeaderStoreItemInfo info = new HttpHeaders.HeaderStoreItemInfo(this.GetParser(name));
    this.AddHeaderToStore(name, info);
    return info;
  }

  private void AddHeaderToStore(string name, HttpHeaders.HeaderStoreItemInfo info)
  {
    if (this._headerStore == null)
      this._headerStore = new Dictionary<string, HttpHeaders.HeaderStoreItemInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    this._headerStore.Add(name, info);
  }

  private bool TryGetHeaderInfo(string name, out HttpHeaders.HeaderStoreItemInfo info)
  {
    if (this._headerStore != null)
      return this._headerStore.TryGetValue(name, out info);
    info = (HttpHeaders.HeaderStoreItemInfo) null;
    return false;
  }

  private bool TryGetAndParseHeaderInfo(string name, out HttpHeaders.HeaderStoreItemInfo info)
  {
    return this.TryGetHeaderInfo(name, out info) && this.ParseRawHeaderValues(name, info, true);
  }

  private bool ParseRawHeaderValues(
    string name,
    HttpHeaders.HeaderStoreItemInfo info,
    bool removeEmptyHeader)
  {
    lock (info)
    {
      if (info.RawValue != null)
      {
        if (!(info.RawValue is List<string> rawValue))
          HttpHeaders.ParseSingleRawHeaderValue(name, info);
        else
          HttpHeaders.ParseMultipleRawHeaderValues(name, info, rawValue);
        info.RawValue = (object) null;
        if (info.InvalidValue == null)
        {
          if (info.ParsedValue == null)
          {
            if (removeEmptyHeader)
              this._headerStore.Remove(name);
            return false;
          }
        }
      }
    }
    return true;
  }

  private static void ParseMultipleRawHeaderValues(
    string name,
    HttpHeaders.HeaderStoreItemInfo info,
    List<string> rawValues)
  {
    if (info.Parser == null)
    {
      foreach (string rawValue in rawValues)
      {
        if (!HttpHeaders.ContainsInvalidNewLine(rawValue, name))
          HttpHeaders.AddValue(info, (object) rawValue, HttpHeaders.StoreLocation.Parsed);
      }
    }
    else
    {
      foreach (string rawValue in rawValues)
      {
        if (!HttpHeaders.TryParseAndAddRawHeaderValue(name, info, rawValue, true) && HttpEventSource.Log.IsEnabled())
          HttpEventSource.Log.HeadersInvalidValue(name, rawValue);
      }
    }
  }

  private static void ParseSingleRawHeaderValue(string name, HttpHeaders.HeaderStoreItemInfo info)
  {
    string rawValue = info.RawValue as string;
    if (info.Parser == null)
    {
      if (HttpHeaders.ContainsInvalidNewLine(rawValue, name))
        return;
      HttpHeaders.AddValue(info, (object) rawValue, HttpHeaders.StoreLocation.Parsed);
    }
    else
    {
      if (HttpHeaders.TryParseAndAddRawHeaderValue(name, info, rawValue, true) || !HttpEventSource.Log.IsEnabled())
        return;
      HttpEventSource.Log.HeadersInvalidValue(name, rawValue);
    }
  }

  internal bool TryParseAndAddValue(string name, string value)
  {
    HttpHeaders.HeaderStoreItemInfo info;
    bool addToStore;
    this.PrepareHeaderInfoForAdd(name, out info, out addToStore);
    bool addRawHeaderValue = HttpHeaders.TryParseAndAddRawHeaderValue(name, info, value, false);
    if (addRawHeaderValue & addToStore && info.ParsedValue != null)
      this.AddHeaderToStore(name, info);
    return addRawHeaderValue;
  }

  private static bool TryParseAndAddRawHeaderValue(
    string name,
    HttpHeaders.HeaderStoreItemInfo info,
    string value,
    bool addWhenInvalid)
  {
    if (!info.CanAddValue)
    {
      if (addWhenInvalid)
        HttpHeaders.AddValue(info, (object) (value ?? string.Empty), HttpHeaders.StoreLocation.Invalid);
      return false;
    }
    int index = 0;
    object parsedValue = (object) null;
    if (info.Parser.TryParseValue(value, info.ParsedValue, ref index, out parsedValue))
    {
      if (value == null || index == value.Length)
      {
        if (parsedValue != null)
          HttpHeaders.AddValue(info, parsedValue, HttpHeaders.StoreLocation.Parsed);
        return true;
      }
      List<object> objectList = new List<object>();
      if (parsedValue != null)
        objectList.Add(parsedValue);
      while (index < value.Length)
      {
        if (info.Parser.TryParseValue(value, info.ParsedValue, ref index, out parsedValue))
        {
          if (parsedValue != null)
            objectList.Add(parsedValue);
        }
        else
        {
          if (!HttpHeaders.ContainsInvalidNewLine(value, name) & addWhenInvalid)
            HttpHeaders.AddValue(info, (object) value, HttpHeaders.StoreLocation.Invalid);
          return false;
        }
      }
      foreach (object obj in objectList)
        HttpHeaders.AddValue(info, obj, HttpHeaders.StoreLocation.Parsed);
      return true;
    }
    if (!HttpHeaders.ContainsInvalidNewLine(value, name) & addWhenInvalid)
      HttpHeaders.AddValue(info, (object) (value ?? string.Empty), HttpHeaders.StoreLocation.Invalid);
    return false;
  }

  private static void AddValue(
    HttpHeaders.HeaderStoreItemInfo info,
    object value,
    HttpHeaders.StoreLocation location)
  {
    switch (location)
    {
      case HttpHeaders.StoreLocation.Raw:
        object rawValue = info.RawValue;
        HttpHeaders.AddValueToStoreValue<string>(info, value, ref rawValue);
        info.RawValue = rawValue;
        break;
      case HttpHeaders.StoreLocation.Invalid:
        object invalidValue = info.InvalidValue;
        HttpHeaders.AddValueToStoreValue<string>(info, value, ref invalidValue);
        info.InvalidValue = invalidValue;
        break;
      case HttpHeaders.StoreLocation.Parsed:
        object parsedValue = info.ParsedValue;
        HttpHeaders.AddValueToStoreValue<object>(info, value, ref parsedValue);
        info.ParsedValue = parsedValue;
        break;
    }
  }

  private static void AddValueToStoreValue<T>(
    HttpHeaders.HeaderStoreItemInfo info,
    object value,
    ref object currentStoreValue)
    where T : class
  {
    if (currentStoreValue == null)
    {
      currentStoreValue = value;
    }
    else
    {
      if (!(currentStoreValue is List<T> objList))
      {
        objList = new List<T>(2);
        objList.Add(currentStoreValue as T);
        currentStoreValue = (object) objList;
      }
      objList.Add(value as T);
    }
  }

  internal object GetParsedValues(string name)
  {
    HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
    return !this.TryGetAndParseHeaderInfo(name, out info) ? (object) null : info.ParsedValue;
  }

  private void PrepareHeaderInfoForAdd(
    string name,
    out HttpHeaders.HeaderStoreItemInfo info,
    out bool addToStore)
  {
    info = (HttpHeaders.HeaderStoreItemInfo) null;
    addToStore = false;
    if (this.TryGetAndParseHeaderInfo(name, out info))
      return;
    info = new HttpHeaders.HeaderStoreItemInfo(this.GetParser(name));
    addToStore = true;
  }

  private void ParseAndAddValue(string name, HttpHeaders.HeaderStoreItemInfo info, string value)
  {
    if (info.Parser == null)
    {
      HttpHeaders.CheckInvalidNewLine(value);
      HttpHeaders.AddValue(info, (object) (value ?? string.Empty), HttpHeaders.StoreLocation.Parsed);
    }
    else
    {
      if (!info.CanAddValue)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_single_value_header, (object) name));
      int index = 0;
      object obj1 = info.Parser.ParseValue(value, info.ParsedValue, ref index);
      if (value == null || index == value.Length)
      {
        if (obj1 == null)
          return;
        HttpHeaders.AddValue(info, obj1, HttpHeaders.StoreLocation.Parsed);
      }
      else
      {
        List<object> objectList = new List<object>();
        if (obj1 != null)
          objectList.Add(obj1);
        while (index < value.Length)
        {
          object obj2 = info.Parser.ParseValue(value, info.ParsedValue, ref index);
          if (obj2 != null)
            objectList.Add(obj2);
        }
        foreach (object obj3 in objectList)
          HttpHeaders.AddValue(info, obj3, HttpHeaders.StoreLocation.Parsed);
      }
    }
  }

  private HttpHeaderParser GetParser(string name)
  {
    if (this._parserStore == null)
      return (HttpHeaderParser) null;
    HttpHeaderParser httpHeaderParser = (HttpHeaderParser) null;
    return this._parserStore.TryGetValue(name, out httpHeaderParser) ? httpHeaderParser : (HttpHeaderParser) null;
  }

  private void CheckHeaderName(string name)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (name));
    if (HttpRuleParser.GetTokenLength(name, 0) != name.Length)
      throw new FormatException(SR.net_http_headers_invalid_header_name);
    if (this._invalidHeaders != null && this._invalidHeaders.Contains(name))
      throw new InvalidOperationException(SR.net_http_headers_not_allowed_header_name);
  }

  private bool TryCheckHeaderName(string name)
  {
    return !string.IsNullOrEmpty(name) && HttpRuleParser.GetTokenLength(name, 0) == name.Length && (this._invalidHeaders == null || !this._invalidHeaders.Contains(name));
  }

  private static void CheckInvalidNewLine(string value)
  {
    if (value != null && HttpRuleParser.ContainsInvalidNewLine(value))
      throw new FormatException(SR.net_http_headers_no_newlines);
  }

  private static bool ContainsInvalidNewLine(string value, string name)
  {
    if (!HttpRuleParser.ContainsInvalidNewLine(value))
      return false;
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.PrintError(NetEventSource.ComponentType.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_log_headers_no_newlines, (object) name, (object) value));
    return true;
  }

  private static string[] GetValuesAsStrings(HttpHeaders.HeaderStoreItemInfo info)
  {
    return HttpHeaders.GetValuesAsStrings(info, (object) null);
  }

  private static string[] GetValuesAsStrings(HttpHeaders.HeaderStoreItemInfo info, object exclude)
  {
    int valueCount = HttpHeaders.GetValueCount(info);
    string[] valuesAsStrings = new string[valueCount];
    if (valueCount > 0)
    {
      int currentIndex = 0;
      HttpHeaders.ReadStoreValues<string>(valuesAsStrings, info.RawValue, (HttpHeaderParser) null, (string) null, ref currentIndex);
      HttpHeaders.ReadStoreValues<object>(valuesAsStrings, info.ParsedValue, info.Parser, exclude, ref currentIndex);
      HttpHeaders.ReadStoreValues<string>(valuesAsStrings, info.InvalidValue, (HttpHeaderParser) null, (string) null, ref currentIndex);
      if (currentIndex < valueCount)
      {
        string[] destinationArray = new string[currentIndex];
        Array.Copy((Array) valuesAsStrings, 0, (Array) destinationArray, 0, currentIndex);
        valuesAsStrings = destinationArray;
      }
    }
    return valuesAsStrings;
  }

  private static int GetValueCount(HttpHeaders.HeaderStoreItemInfo info)
  {
    int valueCount = 0;
    HttpHeaders.UpdateValueCount<string>(info.RawValue, ref valueCount);
    HttpHeaders.UpdateValueCount<string>(info.InvalidValue, ref valueCount);
    HttpHeaders.UpdateValueCount<object>(info.ParsedValue, ref valueCount);
    return valueCount;
  }

  private static void UpdateValueCount<T>(object valueStore, ref int valueCount)
  {
    if (valueStore == null)
      return;
    if (valueStore is List<T> objList)
      valueCount += objList.Count;
    else
      ++valueCount;
  }

  private static void ReadStoreValues<T>(
    string[] values,
    object storeValue,
    HttpHeaderParser parser,
    T exclude,
    ref int currentIndex)
  {
    if (storeValue == null)
      return;
    if (!(storeValue is List<T> objList))
    {
      if (!HttpHeaders.ShouldAdd<T>(storeValue, parser, exclude))
        return;
      values[currentIndex] = parser == null ? storeValue.ToString() : parser.ToString(storeValue);
      ++currentIndex;
    }
    else
    {
      foreach (T obj in objList)
      {
        object storeValue1 = (object) obj;
        if (HttpHeaders.ShouldAdd<T>(storeValue1, parser, exclude))
        {
          values[currentIndex] = parser == null ? storeValue1.ToString() : parser.ToString(storeValue1);
          ++currentIndex;
        }
      }
    }
  }

  private static bool ShouldAdd<T>(object storeValue, HttpHeaderParser parser, T exclude)
  {
    bool flag = true;
    if (parser != null && (object) exclude != null)
      flag = parser.Comparer == null ? !exclude.Equals(storeValue) : !parser.Comparer.Equals((object) exclude, storeValue);
    return flag;
  }

  private bool AreEqual(object value, object storeValue, IEqualityComparer comparer)
  {
    return comparer != null ? comparer.Equals(value, storeValue) : value.Equals(storeValue);
  }

  private enum StoreLocation
  {
    Raw,
    Invalid,
    Parsed,
  }

  private class HeaderStoreItemInfo
  {
    private object _rawValue;
    private object _invalidValue;
    private object _parsedValue;
    private HttpHeaderParser _parser;

    internal object RawValue
    {
      get => this._rawValue;
      set => this._rawValue = value;
    }

    internal object InvalidValue
    {
      get => this._invalidValue;
      set => this._invalidValue = value;
    }

    internal object ParsedValue
    {
      get => this._parsedValue;
      set => this._parsedValue = value;
    }

    internal HttpHeaderParser Parser => this._parser;

    internal bool CanAddValue
    {
      get
      {
        if (this._parser.SupportsMultipleValues)
          return true;
        return this._invalidValue == null && this._parsedValue == null;
      }
    }

    internal bool IsEmpty
    {
      get => this._rawValue == null && this._invalidValue == null && this._parsedValue == null;
    }

    internal HeaderStoreItemInfo(HttpHeaderParser parser) => this._parser = parser;
  }
}
