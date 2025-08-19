// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.MediaTypeWithQualityHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

public sealed class MediaTypeWithQualityHeaderValue : MediaTypeHeaderValue, ICloneable
{
  public double? Quality
  {
    get => HeaderUtilities.GetQuality((ObjectCollection<NameValueHeaderValue>) this.Parameters);
    set
    {
      HeaderUtilities.SetQuality((ObjectCollection<NameValueHeaderValue>) this.Parameters, value);
    }
  }

  internal MediaTypeWithQualityHeaderValue()
  {
  }

  public MediaTypeWithQualityHeaderValue(string mediaType)
    : base(mediaType)
  {
  }

  public MediaTypeWithQualityHeaderValue(string mediaType, double quality)
    : base(mediaType)
  {
    this.Quality = new double?(quality);
  }

  private MediaTypeWithQualityHeaderValue(MediaTypeWithQualityHeaderValue source)
    : base((MediaTypeHeaderValue) source)
  {
  }

  object ICloneable.Clone() => (object) new MediaTypeWithQualityHeaderValue(this);

  public static MediaTypeWithQualityHeaderValue Parse(string input)
  {
    int index = 0;
    return (MediaTypeWithQualityHeaderValue) MediaTypeHeaderParser.SingleValueWithQualityParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out MediaTypeWithQualityHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (MediaTypeWithQualityHeaderValue) null;
    object parsedValue1;
    if (!MediaTypeHeaderParser.SingleValueWithQualityParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (MediaTypeWithQualityHeaderValue) parsedValue1;
    return true;
  }
}
