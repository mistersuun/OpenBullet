// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TransferCodingHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

public class TransferCodingHeaderValue : ICloneable
{
  private ObjectCollection<NameValueHeaderValue> _parameters;
  private string _value;

  public string Value => this._value;

  public ICollection<NameValueHeaderValue> Parameters
  {
    get
    {
      if (this._parameters == null)
        this._parameters = new ObjectCollection<NameValueHeaderValue>();
      return (ICollection<NameValueHeaderValue>) this._parameters;
    }
  }

  internal TransferCodingHeaderValue()
  {
  }

  protected TransferCodingHeaderValue(TransferCodingHeaderValue source)
  {
    this._value = source._value;
    if (source._parameters == null)
      return;
    foreach (ICloneable parameter in source._parameters)
      this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
  }

  public TransferCodingHeaderValue(string value)
  {
    HeaderUtilities.CheckValidToken(value, nameof (value));
    this._value = value;
  }

  public static TransferCodingHeaderValue Parse(string input)
  {
    int index = 0;
    return (TransferCodingHeaderValue) TransferCodingHeaderParser.SingleValueParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out TransferCodingHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (TransferCodingHeaderValue) null;
    object parsedValue1;
    if (!TransferCodingHeaderParser.SingleValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (TransferCodingHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetTransferCodingLength(
    string input,
    int startIndex,
    Func<TransferCodingHeaderValue> transferCodingCreator,
    out TransferCodingHeaderValue parsedValue)
  {
    parsedValue = (TransferCodingHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength == 0)
      return 0;
    string str = input.Substring(startIndex, tokenLength);
    int startIndex1 = startIndex + tokenLength;
    int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index < input.Length && input[index] == ';')
    {
      TransferCodingHeaderValue codingHeaderValue = transferCodingCreator();
      codingHeaderValue._value = str;
      int startIndex2 = index + 1;
      int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', (ObjectCollection<NameValueHeaderValue>) codingHeaderValue.Parameters);
      if (nameValueListLength == 0)
        return 0;
      parsedValue = codingHeaderValue;
      return startIndex2 + nameValueListLength - startIndex;
    }
    TransferCodingHeaderValue codingHeaderValue1 = transferCodingCreator();
    codingHeaderValue1._value = str;
    parsedValue = codingHeaderValue1;
    return index - startIndex;
  }

  public override string ToString()
  {
    return this._value + NameValueHeaderValue.ToString(this._parameters, ';', true);
  }

  public override bool Equals(object obj)
  {
    return obj is TransferCodingHeaderValue codingHeaderValue && string.Equals(this._value, codingHeaderValue._value, StringComparison.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this._parameters, codingHeaderValue._parameters);
  }

  public override int GetHashCode()
  {
    return StringComparer.OrdinalIgnoreCase.GetHashCode(this._value) ^ NameValueHeaderValue.GetHashCode(this._parameters);
  }

  object ICloneable.Clone() => (object) new TransferCodingHeaderValue(this);
}
