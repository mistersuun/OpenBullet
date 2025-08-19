// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.MediaTypeHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

internal class MediaTypeHeaderParser : BaseHeaderParser
{
  private bool _supportsMultipleValues;
  private Func<MediaTypeHeaderValue> _mediaTypeCreator;
  internal static readonly MediaTypeHeaderParser SingleValueParser = new MediaTypeHeaderParser(false, new Func<MediaTypeHeaderValue>(MediaTypeHeaderParser.CreateMediaType));
  internal static readonly MediaTypeHeaderParser SingleValueWithQualityParser = new MediaTypeHeaderParser(false, new Func<MediaTypeHeaderValue>(MediaTypeHeaderParser.CreateMediaTypeWithQuality));
  internal static readonly MediaTypeHeaderParser MultipleValuesParser = new MediaTypeHeaderParser(true, new Func<MediaTypeHeaderValue>(MediaTypeHeaderParser.CreateMediaTypeWithQuality));

  private MediaTypeHeaderParser(
    bool supportsMultipleValues,
    Func<MediaTypeHeaderValue> mediaTypeCreator)
    : base(supportsMultipleValues)
  {
    this._supportsMultipleValues = supportsMultipleValues;
    this._mediaTypeCreator = mediaTypeCreator;
  }

  protected override int GetParsedValueLength(
    string value,
    int startIndex,
    object storeValue,
    out object parsedValue)
  {
    MediaTypeHeaderValue parsedValue1 = (MediaTypeHeaderValue) null;
    int mediaTypeLength = MediaTypeHeaderValue.GetMediaTypeLength(value, startIndex, this._mediaTypeCreator, out parsedValue1);
    parsedValue = (object) parsedValue1;
    return mediaTypeLength;
  }

  private static MediaTypeHeaderValue CreateMediaType() => new MediaTypeHeaderValue();

  private static MediaTypeHeaderValue CreateMediaTypeWithQuality()
  {
    return (MediaTypeHeaderValue) new MediaTypeWithQualityHeaderValue();
  }
}
