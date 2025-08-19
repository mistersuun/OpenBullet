// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JsonPath.BooleanQueryExpression
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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
    JValue jvalue = leftResult as JValue;
    JValue queryValue = rightResult as JValue;
    if (jvalue != null && queryValue != null)
    {
      switch (this.Operator)
      {
        case QueryOperator.Equals:
          if (this.EqualsWithStringCoercion(jvalue, queryValue))
            return true;
          break;
        case QueryOperator.NotEquals:
          if (!this.EqualsWithStringCoercion(jvalue, queryValue))
            return true;
          break;
        case QueryOperator.Exists:
          return true;
        case QueryOperator.LessThan:
          if (jvalue.CompareTo(queryValue) < 0)
            return true;
          break;
        case QueryOperator.LessThanOrEquals:
          if (jvalue.CompareTo(queryValue) <= 0)
            return true;
          break;
        case QueryOperator.GreaterThan:
          if (jvalue.CompareTo(queryValue) > 0)
            return true;
          break;
        case QueryOperator.GreaterThanOrEquals:
          if (jvalue.CompareTo(queryValue) >= 0)
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

  private bool EqualsWithStringCoercion(JValue value, JValue queryValue)
  {
    if (value.Equals(queryValue))
      return true;
    if (queryValue.Type != JTokenType.String)
      return false;
    string b = (string) queryValue.Value;
    string a;
    switch (value.Type)
    {
      case JTokenType.Date:
        using (StringWriter stringWriter = StringUtils.CreateStringWriter(64 /*0x40*/))
        {
          if (value.Value is DateTimeOffset)
            DateTimeUtils.WriteDateTimeOffsetString((TextWriter) stringWriter, (DateTimeOffset) value.Value, DateFormatHandling.IsoDateFormat, (string) null, CultureInfo.InvariantCulture);
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
}
