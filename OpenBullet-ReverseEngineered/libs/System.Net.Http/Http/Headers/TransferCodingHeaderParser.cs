// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TransferCodingHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

internal class TransferCodingHeaderParser : BaseHeaderParser
{
  private Func<TransferCodingHeaderValue> _transferCodingCreator;
  internal static readonly TransferCodingHeaderParser SingleValueParser = new TransferCodingHeaderParser(false, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCoding));
  internal static readonly TransferCodingHeaderParser MultipleValueParser = new TransferCodingHeaderParser(true, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCoding));
  internal static readonly TransferCodingHeaderParser SingleValueWithQualityParser = new TransferCodingHeaderParser(false, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCodingWithQuality));
  internal static readonly TransferCodingHeaderParser MultipleValueWithQualityParser = new TransferCodingHeaderParser(true, new Func<TransferCodingHeaderValue>(TransferCodingHeaderParser.CreateTransferCodingWithQuality));

  private TransferCodingHeaderParser(
    bool supportsMultipleValues,
    Func<TransferCodingHeaderValue> transferCodingCreator)
    : base(supportsMultipleValues)
  {
    this._transferCodingCreator = transferCodingCreator;
  }

  protected override int GetParsedValueLength(
    string value,
    int startIndex,
    object storeValue,
    out object parsedValue)
  {
    TransferCodingHeaderValue parsedValue1 = (TransferCodingHeaderValue) null;
    int transferCodingLength = TransferCodingHeaderValue.GetTransferCodingLength(value, startIndex, this._transferCodingCreator, out parsedValue1);
    parsedValue = (object) parsedValue1;
    return transferCodingLength;
  }

  private static TransferCodingHeaderValue CreateTransferCoding()
  {
    return new TransferCodingHeaderValue();
  }

  private static TransferCodingHeaderValue CreateTransferCodingWithQuality()
  {
    return (TransferCodingHeaderValue) new TransferCodingWithQualityHeaderValue();
  }
}
