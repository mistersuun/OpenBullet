// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TransferCodingWithQualityHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

public sealed class TransferCodingWithQualityHeaderValue : TransferCodingHeaderValue, ICloneable
{
  public double? Quality
  {
    get => HeaderUtilities.GetQuality((ObjectCollection<NameValueHeaderValue>) this.Parameters);
    set
    {
      HeaderUtilities.SetQuality((ObjectCollection<NameValueHeaderValue>) this.Parameters, value);
    }
  }

  internal TransferCodingWithQualityHeaderValue()
  {
  }

  public TransferCodingWithQualityHeaderValue(string value)
    : base(value)
  {
  }

  public TransferCodingWithQualityHeaderValue(string value, double quality)
    : base(value)
  {
    this.Quality = new double?(quality);
  }

  private TransferCodingWithQualityHeaderValue(TransferCodingWithQualityHeaderValue source)
    : base((TransferCodingHeaderValue) source)
  {
  }

  object ICloneable.Clone() => (object) new TransferCodingWithQualityHeaderValue(this);

  public static TransferCodingWithQualityHeaderValue Parse(string input)
  {
    int index = 0;
    return (TransferCodingWithQualityHeaderValue) TransferCodingHeaderParser.SingleValueWithQualityParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(
    string input,
    out TransferCodingWithQualityHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (TransferCodingWithQualityHeaderValue) null;
    object parsedValue1;
    if (!TransferCodingHeaderParser.SingleValueWithQualityParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (TransferCodingWithQualityHeaderValue) parsedValue1;
    return true;
  }
}
