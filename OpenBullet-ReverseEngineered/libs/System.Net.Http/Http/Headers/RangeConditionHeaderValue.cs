// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RangeConditionHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

public class RangeConditionHeaderValue : ICloneable
{
  private DateTimeOffset? _date;
  private EntityTagHeaderValue _entityTag;

  public DateTimeOffset? Date => this._date;

  public EntityTagHeaderValue EntityTag => this._entityTag;

  public RangeConditionHeaderValue(DateTimeOffset date) => this._date = new DateTimeOffset?(date);

  public RangeConditionHeaderValue(EntityTagHeaderValue entityTag)
  {
    this._entityTag = entityTag != null ? entityTag : throw new ArgumentNullException(nameof (entityTag));
  }

  public RangeConditionHeaderValue(string entityTag)
    : this(new EntityTagHeaderValue(entityTag))
  {
  }

  private RangeConditionHeaderValue(RangeConditionHeaderValue source)
  {
    this._entityTag = source._entityTag;
    this._date = source._date;
  }

  private RangeConditionHeaderValue()
  {
  }

  public override string ToString()
  {
    return this._entityTag == null ? HttpRuleParser.DateToString(this._date.Value) : this._entityTag.ToString();
  }

  public override bool Equals(object obj)
  {
    if (!(obj is RangeConditionHeaderValue conditionHeaderValue))
      return false;
    if (this._entityTag != null)
      return this._entityTag.Equals((object) conditionHeaderValue._entityTag);
    return conditionHeaderValue._date.HasValue && this._date.Value == conditionHeaderValue._date.Value;
  }

  public override int GetHashCode()
  {
    return this._entityTag == null ? this._date.Value.GetHashCode() : this._entityTag.GetHashCode();
  }

  public static RangeConditionHeaderValue Parse(string input)
  {
    int index = 0;
    return (RangeConditionHeaderValue) GenericHeaderParser.RangeConditionParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out RangeConditionHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (RangeConditionHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.RangeConditionParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (RangeConditionHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetRangeConditionLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex + 1 >= input.Length)
      return 0;
    int num1 = startIndex;
    DateTimeOffset result = DateTimeOffset.MinValue;
    EntityTagHeaderValue parsedValue1 = (EntityTagHeaderValue) null;
    char ch1 = input[num1];
    char ch2 = input[num1 + 1];
    int num2;
    switch (ch1)
    {
      case '"':
        int entityTagLength = EntityTagHeaderValue.GetEntityTagLength(input, num1, out parsedValue1);
        if (entityTagLength == 0)
          return 0;
        num2 = num1 + entityTagLength;
        if (num2 != input.Length)
          return 0;
        break;
      case 'W':
      case 'w':
        if (ch2 != '/')
          goto default;
        goto case '"';
      default:
        if (!HttpRuleParser.TryStringToDate(input.Substring(num1), out result))
          return 0;
        num2 = input.Length;
        break;
    }
    RangeConditionHeaderValue conditionHeaderValue = new RangeConditionHeaderValue();
    if (parsedValue1 == null)
      conditionHeaderValue._date = new DateTimeOffset?(result);
    else
      conditionHeaderValue._entityTag = parsedValue1;
    parsedValue = (object) conditionHeaderValue;
    return num2 - startIndex;
  }

  object ICloneable.Clone() => (object) new RangeConditionHeaderValue(this);
}
