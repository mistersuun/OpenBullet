// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ProductHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

public class ProductHeaderValue : ICloneable
{
  private string _name;
  private string _version;

  public string Name => this._name;

  public string Version => this._version;

  public ProductHeaderValue(string name)
    : this(name, (string) null)
  {
  }

  public ProductHeaderValue(string name, string version)
  {
    HeaderUtilities.CheckValidToken(name, nameof (name));
    if (!string.IsNullOrEmpty(version))
    {
      HeaderUtilities.CheckValidToken(version, nameof (version));
      this._version = version;
    }
    this._name = name;
  }

  private ProductHeaderValue(ProductHeaderValue source)
  {
    this._name = source._name;
    this._version = source._version;
  }

  private ProductHeaderValue()
  {
  }

  public override string ToString()
  {
    return string.IsNullOrEmpty(this._version) ? this._name : $"{this._name}/{this._version}";
  }

  public override bool Equals(object obj)
  {
    return obj is ProductHeaderValue productHeaderValue && string.Equals(this._name, productHeaderValue._name, StringComparison.OrdinalIgnoreCase) && string.Equals(this._version, productHeaderValue._version, StringComparison.OrdinalIgnoreCase);
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._name);
    if (!string.IsNullOrEmpty(this._version))
      hashCode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(this._version);
    return hashCode;
  }

  public static ProductHeaderValue Parse(string input)
  {
    int index = 0;
    return (ProductHeaderValue) GenericHeaderParser.SingleValueProductParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out ProductHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (ProductHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueProductParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (ProductHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetProductLength(
    string input,
    int startIndex,
    out ProductHeaderValue parsedValue)
  {
    parsedValue = (ProductHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength1 = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength1 == 0)
      return 0;
    ProductHeaderValue productHeaderValue = new ProductHeaderValue();
    productHeaderValue._name = input.Substring(startIndex, tokenLength1);
    int startIndex1 = startIndex + tokenLength1;
    int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index == input.Length || input[index] != '/')
    {
      parsedValue = productHeaderValue;
      return index - startIndex;
    }
    int startIndex2 = index + 1;
    int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    int tokenLength2 = HttpRuleParser.GetTokenLength(input, startIndex3);
    if (tokenLength2 == 0)
      return 0;
    productHeaderValue._version = input.Substring(startIndex3, tokenLength2);
    int startIndex4 = startIndex3 + tokenLength2;
    int num = startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4);
    parsedValue = productHeaderValue;
    return num - startIndex;
  }

  object ICloneable.Clone() => (object) new ProductHeaderValue(this);
}
