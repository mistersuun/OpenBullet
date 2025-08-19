// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.IsoDateTimeConverter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Converters;

internal class IsoDateTimeConverter : DateTimeConverterBase
{
  private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
  private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
  private string _dateTimeFormat;
  private CultureInfo _culture;

  public DateTimeStyles DateTimeStyles
  {
    get => this._dateTimeStyles;
    set => this._dateTimeStyles = value;
  }

  public string DateTimeFormat
  {
    get => this._dateTimeFormat ?? string.Empty;
    set => this._dateTimeFormat = string.IsNullOrEmpty(value) ? (string) null : value;
  }

  public CultureInfo Culture
  {
    get => this._culture ?? CultureInfo.CurrentCulture;
    set => this._culture = value;
  }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    string str;
    switch (value)
    {
      case DateTime universalTime1:
        if ((this._dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (this._dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
          universalTime1 = universalTime1.ToUniversalTime();
        str = universalTime1.ToString(this._dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", (IFormatProvider) this.Culture);
        break;
      case DateTimeOffset universalTime2:
        if ((this._dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (this._dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
          universalTime2 = universalTime2.ToUniversalTime();
        str = universalTime2.ToString(this._dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", (IFormatProvider) this.Culture);
        break;
      default:
        throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) ReflectionUtils.GetObjectType(value)));
    }
    writer.WriteValue(str);
  }

  public override object ReadJson(
    JsonReader reader,
    Type objectType,
    object existingValue,
    JsonSerializer serializer)
  {
    bool flag = ReflectionUtils.IsNullableType(objectType);
    if (reader.TokenType == JsonToken.Null)
    {
      if (!ReflectionUtils.IsNullableType(objectType))
        throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType));
      return (object) null;
    }
    Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
    if (reader.TokenType == JsonToken.Date)
      return type == typeof (DateTimeOffset) ? (!(reader.Value is DateTimeOffset) ? (object) new DateTimeOffset((DateTime) reader.Value) : reader.Value) : (reader.Value is DateTimeOffset ? (object) ((DateTimeOffset) reader.Value).DateTime : reader.Value);
    string str = reader.TokenType == JsonToken.String ? reader.Value.ToString() : throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reader.TokenType));
    if (string.IsNullOrEmpty(str) & flag)
      return (object) null;
    return type == typeof (DateTimeOffset) ? (!string.IsNullOrEmpty(this._dateTimeFormat) ? (object) DateTimeOffset.ParseExact(str, this._dateTimeFormat, (IFormatProvider) this.Culture, this._dateTimeStyles) : (object) DateTimeOffset.Parse(str, (IFormatProvider) this.Culture, this._dateTimeStyles)) : (!string.IsNullOrEmpty(this._dateTimeFormat) ? (object) DateTime.ParseExact(str, this._dateTimeFormat, (IFormatProvider) this.Culture, this._dateTimeStyles) : (object) DateTime.Parse(str, (IFormatProvider) this.Culture, this._dateTimeStyles));
  }
}
