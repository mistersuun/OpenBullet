// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Converters.IsoDateTimeConverter
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Converters;

public class IsoDateTimeConverter : DateTimeConverterBase
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
      if (!flag)
        throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) objectType));
      return (object) null;
    }
    Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
    if (reader.TokenType == JsonToken.Date)
      return type == typeof (DateTimeOffset) ? (!(reader.Value is DateTimeOffset) ? (object) new DateTimeOffset((DateTime) reader.Value) : reader.Value) : (reader.Value is DateTimeOffset dateTimeOffset ? (object) dateTimeOffset.DateTime : reader.Value);
    string str = reader.TokenType == JsonToken.String ? reader.Value.ToString() : throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) reader.TokenType));
    if (string.IsNullOrEmpty(str) & flag)
      return (object) null;
    return type == typeof (DateTimeOffset) ? (!string.IsNullOrEmpty(this._dateTimeFormat) ? (object) DateTimeOffset.ParseExact(str, this._dateTimeFormat, (IFormatProvider) this.Culture, this._dateTimeStyles) : (object) DateTimeOffset.Parse(str, (IFormatProvider) this.Culture, this._dateTimeStyles)) : (!string.IsNullOrEmpty(this._dateTimeFormat) ? (object) DateTime.ParseExact(str, this._dateTimeFormat, (IFormatProvider) this.Culture, this._dateTimeStyles) : (object) DateTime.Parse(str, (IFormatProvider) this.Culture, this._dateTimeStyles));
  }
}
