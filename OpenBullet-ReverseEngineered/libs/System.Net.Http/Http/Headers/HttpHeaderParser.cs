// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

internal abstract class HttpHeaderParser
{
  internal const string DefaultSeparator = ", ";
  private bool _supportsMultipleValues;
  private string _separator;

  public bool SupportsMultipleValues => this._supportsMultipleValues;

  public string Separator => this._separator;

  public virtual IEqualityComparer Comparer => (IEqualityComparer) null;

  protected HttpHeaderParser(bool supportsMultipleValues)
  {
    this._supportsMultipleValues = supportsMultipleValues;
    if (!supportsMultipleValues)
      return;
    this._separator = ", ";
  }

  protected HttpHeaderParser(bool supportsMultipleValues, string separator)
  {
    this._supportsMultipleValues = supportsMultipleValues;
    this._separator = separator;
  }

  public abstract bool TryParseValue(
    string value,
    object storeValue,
    ref int index,
    out object parsedValue);

  public object ParseValue(string value, object storeValue, ref int index)
  {
    object parsedValue = (object) null;
    if (!this.TryParseValue(value, storeValue, ref index, out parsedValue))
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, value == null ? (object) "<null>" : (object) value.Substring(index)));
    return parsedValue;
  }

  public virtual string ToString(object value) => value.ToString();
}
