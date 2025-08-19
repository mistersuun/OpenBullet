// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ProductInfoHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

internal class ProductInfoHeaderParser : HttpHeaderParser
{
  private const string separator = " ";
  internal static readonly ProductInfoHeaderParser SingleValueParser = new ProductInfoHeaderParser(false);
  internal static readonly ProductInfoHeaderParser MultipleValueParser = new ProductInfoHeaderParser(true);

  private ProductInfoHeaderParser(bool supportsMultipleValues)
    : base(supportsMultipleValues, " ")
  {
  }

  public override bool TryParseValue(
    string value,
    object storeValue,
    ref int index,
    out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(value) || index == value.Length)
      return false;
    int startIndex = index + HttpRuleParser.GetWhitespaceLength(value, index);
    if (startIndex == value.Length)
      return false;
    ProductInfoHeaderValue parsedValue1 = (ProductInfoHeaderValue) null;
    int productInfoLength = ProductInfoHeaderValue.GetProductInfoLength(value, startIndex, out parsedValue1);
    if (productInfoLength == 0)
      return false;
    int num = startIndex + productInfoLength;
    if (num < value.Length)
    {
      switch (value[num - 1])
      {
        case '\t':
        case ' ':
          break;
        default:
          return false;
      }
    }
    index = num;
    parsedValue = (object) parsedValue1;
    return true;
  }
}
