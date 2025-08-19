// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.BooleanQueryExpression
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace Newtonsoft.Json.Linq.JsonPath;

internal class BooleanQueryExpression : QueryExpression
{
  public object Left { get; set; }

  public object Right { get; set; }

  private IEnumerable<JToken> GetResult(JToken root, JToken t, object o)
  {
    switch (o)
    {
      case JToken jtoken:
        return (IEnumerable<JToken>) new JToken[1]{ jtoken };
      case List<PathFilter> filters:
        return JPath.Evaluate(filters, root, t, false);
      default:
        return (IEnumerable<JToken>) CollectionUtils.ArrayEmpty<JToken>();
    }
  }

  public override bool IsMatch(JToken root, JToken t)
  {
    if (this.Operator == QueryOperator.Exists)
      return this.GetResult(root, t, this.Left).Any<JToken>();
    using (IEnumerator<JToken> enumerator = this.GetResult(root, t, this.Left).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        IEnumerable<JToken> result = this.GetResult(root, t, this.Right);
        if (!(result is ICollection<JToken> jtokens1))
          jtokens1 = (ICollection<JToken>) result.ToList<JToken>();
        ICollection<JToken> jtokens2 = jtokens1;
        do
        {
          JToken current = enumerator.Current;
          foreach (JToken rightResult in (IEnumerable<JToken>) jtokens2)
          {
            if (this.MatchTokens(current, rightResult))
              return true;
          }
        }
        while (enumerator.MoveNext());
      }
    }
    return false;
  }

  private bool MatchTokens(JToken leftResult, JToken rightResult)
  {
    if (leftResult is JValue input && rightResult is JValue jvalue)
    {
      switch (this.Operator)
      {
        case QueryOperator.Equals:
          if (BooleanQueryExpression.EqualsWithStringCoercion(input, jvalue))
            return true;
          break;
        case QueryOperator.NotEquals:
          if (!BooleanQueryExpression.EqualsWithStringCoercion(input, jvalue))
            return true;
          break;
        case QueryOperator.Exists:
          return true;
        case QueryOperator.LessThan:
          if (input.CompareTo(jvalue) < 0)
            return true;
          break;
        case QueryOperator.LessThanOrEquals:
          if (input.CompareTo(jvalue) <= 0)
            return true;
          break;
        case QueryOperator.GreaterThan:
          if (input.CompareTo(jvalue) > 0)
            return true;
          break;
        case QueryOperator.GreaterThanOrEquals:
          if (input.CompareTo(jvalue) >= 0)
            return true;
          break;
        case QueryOperator.RegexEquals:
          if (BooleanQueryExpression.RegexEquals(input, jvalue))
            return true;
          break;
        case QueryOperator.StrictEquals:
          if (BooleanQueryExpression.EqualsWithStrictMatch(input, jvalue))
            return true;
          break;
        case QueryOperator.StrictNotEquals:
          if (!BooleanQueryExpression.EqualsWithStrictMatch(input, jvalue))
            return true;
          break;
      }
    }
    else
    {
      switch (this.Operator)
      {
        case QueryOperator.NotEquals:
        case QueryOperator.Exists:
          return true;
      }
    }
    return false;
  }

  private static bool RegexEquals(JValue input, JValue pattern)
  {
    if (input.Type != JTokenType.String || pattern.Type != JTokenType.String)
      return false;
    string str = (string) pattern.Value;
    int num = str.LastIndexOf('/');
    string pattern1 = str.Substring(1, num - 1);
    string optionsText = str.Substring(num + 1);
    return Regex.IsMatch((string) input.Value, pattern1, MiscellaneousUtils.GetRegexOptions(optionsText));
  }

  internal static bool EqualsWithStringCoercion(JValue value, JValue queryValue)
  {
    if (value.Equals(queryValue))
      return true;
    if (value.Type == JTokenType.Integer && queryValue.Type == JTokenType.Float || value.Type == JTokenType.Float && queryValue.Type == JTokenType.Integer)
      return JValue.Compare(value.Type, value.Value, queryValue.Value) == 0;
    if (queryValue.Type != JTokenType.String)
      return false;
    string b = (string) queryValue.Value;
    string a;
    switch (value.Type)
    {
      case JTokenType.Date:
        using (StringWriter stringWriter = StringUtils.CreateStringWriter(64 /*0x40*/))
        {
          if (value.Value is DateTimeOffset dateTimeOffset)
            DateTimeUtils.WriteDateTimeOffsetString((TextWriter) stringWriter, dateTimeOffset, DateFormatHandling.IsoDateFormat, (string) null, CultureInfo.InvariantCulture);
          else
            DateTimeUtils.WriteDateTimeString((TextWriter) stringWriter, (DateTime) value.Value, DateFormatHandling.IsoDateFormat, (string) null, CultureInfo.InvariantCulture);
          a = stringWriter.ToString();
          break;
        }
      case JTokenType.Bytes:
        a = Convert.ToBase64String((byte[]) value.Value);
        break;
      case JTokenType.Guid:
      case JTokenType.TimeSpan:
        a = value.Value.ToString();
        break;
      case JTokenType.Uri:
        a = ((Uri) value.Value).OriginalString;
        break;
      default:
        return false;
    }
    return string.Equals(a, b, StringComparison.Ordinal);
  }

  internal static bool EqualsWithStrictMatch(JValue value, JValue queryValue)
  {
    if (value.Type == JTokenType.Integer && queryValue.Type == JTokenType.Float || value.Type == JTokenType.Float && queryValue.Type == JTokenType.Integer)
      return JValue.Compare(value.Type, value.Value, queryValue.Value) == 0;
    return value.Type == queryValue.Type && value.Equals(queryValue);
  }
}
