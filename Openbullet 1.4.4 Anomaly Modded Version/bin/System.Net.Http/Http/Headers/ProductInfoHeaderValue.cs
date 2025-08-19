// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ProductInfoHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

public class ProductInfoHeaderValue : ICloneable
{
  private ProductHeaderValue _product;
  private string _comment;

  public ProductHeaderValue Product => this._product;

  public string Comment => this._comment;

  public ProductInfoHeaderValue(string productName, string productVersion)
    : this(new ProductHeaderValue(productName, productVersion))
  {
  }

  public ProductInfoHeaderValue(ProductHeaderValue product)
  {
    this._product = product != null ? product : throw new ArgumentNullException(nameof (product));
  }

  public ProductInfoHeaderValue(string comment)
  {
    HeaderUtilities.CheckValidComment(comment, nameof (comment));
    this._comment = comment;
  }

  private ProductInfoHeaderValue(ProductInfoHeaderValue source)
  {
    this._product = source._product;
    this._comment = source._comment;
  }

  private ProductInfoHeaderValue()
  {
  }

  public override string ToString()
  {
    return this._product == null ? this._comment : this._product.ToString();
  }

  public override bool Equals(object obj)
  {
    if (!(obj is ProductInfoHeaderValue productInfoHeaderValue))
      return false;
    return this._product == null ? string.Equals(this._comment, productInfoHeaderValue._comment, StringComparison.Ordinal) : this._product.Equals((object) productInfoHeaderValue._product);
  }

  public override int GetHashCode()
  {
    return this._product == null ? this._comment.GetHashCode() : this._product.GetHashCode();
  }

  public static ProductInfoHeaderValue Parse(string input)
  {
    int index = 0;
    object obj = ProductInfoHeaderParser.SingleValueParser.ParseValue(input, (object) null, ref index);
    if (index < input.Length)
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) input.Substring(index)));
    return (ProductInfoHeaderValue) obj;
  }

  public static bool TryParse(string input, out ProductInfoHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (ProductInfoHeaderValue) null;
    object parsedValue1;
    if (!ProductInfoHeaderParser.SingleValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1) || index < input.Length)
      return false;
    parsedValue = (ProductInfoHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetProductInfoLength(
    string input,
    int startIndex,
    out ProductInfoHeaderValue parsedValue)
  {
    parsedValue = (ProductInfoHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int num1 = startIndex;
    string str = (string) null;
    ProductHeaderValue parsedValue1 = (ProductHeaderValue) null;
    int num2;
    if (input[num1] == '(')
    {
      int length = 0;
      if (HttpRuleParser.GetCommentLength(input, num1, out length) != HttpParseResult.Parsed)
        return 0;
      str = input.Substring(num1, length);
      int startIndex1 = num1 + length;
      num2 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    }
    else
    {
      int productLength = ProductHeaderValue.GetProductLength(input, num1, out parsedValue1);
      if (productLength == 0)
        return 0;
      num2 = num1 + productLength;
    }
    parsedValue = new ProductInfoHeaderValue();
    parsedValue._product = parsedValue1;
    parsedValue._comment = str;
    return num2 - startIndex;
  }

  object ICloneable.Clone() => (object) new ProductInfoHeaderValue(this);
}
