// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.NameValueWithParametersHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

public class NameValueWithParametersHeaderValue : NameValueHeaderValue, ICloneable
{
  private static readonly Func<NameValueHeaderValue> s_nameValueCreator = new Func<NameValueHeaderValue>(NameValueWithParametersHeaderValue.CreateNameValue);
  private ObjectCollection<NameValueHeaderValue> _parameters;

  public ICollection<NameValueHeaderValue> Parameters
  {
    get
    {
      if (this._parameters == null)
        this._parameters = new ObjectCollection<NameValueHeaderValue>();
      return (ICollection<NameValueHeaderValue>) this._parameters;
    }
  }

  public NameValueWithParametersHeaderValue(string name)
    : base(name)
  {
  }

  public NameValueWithParametersHeaderValue(string name, string value)
    : base(name, value)
  {
  }

  internal NameValueWithParametersHeaderValue()
  {
  }

  protected NameValueWithParametersHeaderValue(NameValueWithParametersHeaderValue source)
    : base((NameValueHeaderValue) source)
  {
    if (source._parameters == null)
      return;
    foreach (ICloneable parameter in source._parameters)
      this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
  }

  public override bool Equals(object obj)
  {
    return base.Equals(obj) && obj is NameValueWithParametersHeaderValue parametersHeaderValue && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this._parameters, parametersHeaderValue._parameters);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode() ^ NameValueHeaderValue.GetHashCode(this._parameters);
  }

  public override string ToString()
  {
    return base.ToString() + NameValueHeaderValue.ToString(this._parameters, ';', true);
  }

  public static NameValueWithParametersHeaderValue Parse(string input)
  {
    int index = 0;
    return (NameValueWithParametersHeaderValue) GenericHeaderParser.SingleValueNameValueWithParametersParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out NameValueWithParametersHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (NameValueWithParametersHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueNameValueWithParametersParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (NameValueWithParametersHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetNameValueWithParametersLength(
    string input,
    int startIndex,
    out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    NameValueHeaderValue parsedValue1 = (NameValueHeaderValue) null;
    int nameValueLength = NameValueHeaderValue.GetNameValueLength(input, startIndex, NameValueWithParametersHeaderValue.s_nameValueCreator, out parsedValue1);
    if (nameValueLength == 0)
      return 0;
    int startIndex1 = startIndex + nameValueLength;
    int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    NameValueWithParametersHeaderValue parametersHeaderValue = parsedValue1 as NameValueWithParametersHeaderValue;
    if (index < input.Length && input[index] == ';')
    {
      int startIndex2 = index + 1;
      int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', (ObjectCollection<NameValueHeaderValue>) parametersHeaderValue.Parameters);
      if (nameValueListLength == 0)
        return 0;
      parsedValue = (object) parametersHeaderValue;
      return startIndex2 + nameValueListLength - startIndex;
    }
    parsedValue = (object) parametersHeaderValue;
    return index - startIndex;
  }

  private static NameValueHeaderValue CreateNameValue()
  {
    return (NameValueHeaderValue) new NameValueWithParametersHeaderValue();
  }

  object ICloneable.Clone() => (object) new NameValueWithParametersHeaderValue(this);
}
