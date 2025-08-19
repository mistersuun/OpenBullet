// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Conditions.Condition
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.Functions.Conditions;

public static class Condition
{
  public static bool ReplaceAndVerify(string left, Comparer comparer, string right, BotData data)
  {
    return Condition.ReplaceAndVerify(new KeycheckCondition()
    {
      Left = left,
      Comparer = comparer,
      Right = right
    }, data);
  }

  public static bool ReplaceAndVerify(KeycheckCondition kcCond, BotData data)
  {
    NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
    CultureInfo provider = new CultureInfo("en-US");
    List<string> source = BlockBase.ReplaceValuesRecursive(kcCond.Left, data);
    string r = BlockBase.ReplaceValues(kcCond.Right, data);
    switch (kcCond.Comparer)
    {
      case Comparer.LessThan:
        return source.Any<string>((Func<string, bool>) (l => Decimal.Parse(l.Replace(',', '.'), style, (IFormatProvider) provider) < Decimal.Parse(r.Replace(',', '.'), style, (IFormatProvider) provider)));
      case Comparer.GreaterThan:
        return source.Any<string>((Func<string, bool>) (l => Decimal.Parse(l.Replace(',', '.'), style, (IFormatProvider) provider) > Decimal.Parse(r.Replace(',', '.'), style, (IFormatProvider) provider)));
      case Comparer.EqualTo:
        return source.Any<string>((Func<string, bool>) (l => l == r));
      case Comparer.NotEqualTo:
        return source.Any<string>((Func<string, bool>) (l => l != r));
      case Comparer.Contains:
        return source.Any<string>((Func<string, bool>) (l => l.Contains(r)));
      case Comparer.DoesNotContain:
        return source.Any<string>((Func<string, bool>) (l => !l.Contains(r)));
      case Comparer.Exists:
        return source.Any<string>((Func<string, bool>) (l => l != kcCond.Left));
      case Comparer.DoesNotExist:
        return source.All<string>((Func<string, bool>) (l => l == kcCond.Left));
      case Comparer.MatchesRegex:
        return source.Any<string>((Func<string, bool>) (l => Regex.Match(l, r).Success));
      case Comparer.DoesNotMatchRegex:
        return source.Any<string>((Func<string, bool>) (l => !Regex.Match(l, r).Success));
      default:
        return false;
    }
  }

  public static bool Verify(KeycheckCondition kcCond)
  {
    NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
    CultureInfo provider = new CultureInfo("en-US");
    switch (kcCond.Comparer)
    {
      case Comparer.LessThan:
        return Decimal.Parse(kcCond.Left.Replace(',', '.'), style, (IFormatProvider) provider) < Decimal.Parse(kcCond.Right.Replace(',', '.'), style, (IFormatProvider) provider);
      case Comparer.GreaterThan:
        return Decimal.Parse(kcCond.Left.Replace(',', '.'), style, (IFormatProvider) provider) > Decimal.Parse(kcCond.Right.Replace(',', '.'), style, (IFormatProvider) provider);
      case Comparer.EqualTo:
        return kcCond.Left == kcCond.Right;
      case Comparer.NotEqualTo:
        return kcCond.Left != kcCond.Right;
      case Comparer.Contains:
        return kcCond.Left.Contains(kcCond.Right);
      case Comparer.DoesNotContain:
        return !kcCond.Left.Contains(kcCond.Right);
      case Comparer.Exists:
      case Comparer.DoesNotExist:
        throw new NotSupportedException("Exists and DoesNotExist operators are only supported in the ReplaceAndVerify method.");
      case Comparer.MatchesRegex:
        return Regex.Match(kcCond.Left, kcCond.Right).Success;
      case Comparer.DoesNotMatchRegex:
        return !Regex.Match(kcCond.Left, kcCond.Right).Success;
      default:
        return false;
    }
  }

  public static bool ReplaceAndVerifyAll(KeycheckCondition[] conditions, BotData data)
  {
    return ((IEnumerable<KeycheckCondition>) conditions).All<KeycheckCondition>((Func<KeycheckCondition, bool>) (c => Condition.ReplaceAndVerify(c, data)));
  }

  public static bool VerifyAll(KeycheckCondition[] conditions)
  {
    return ((IEnumerable<KeycheckCondition>) conditions).All<KeycheckCondition>((Func<KeycheckCondition, bool>) (c => Condition.Verify(c)));
  }

  public static bool ReplaceAndVerifyAny(KeycheckCondition[] conditions, BotData data)
  {
    return ((IEnumerable<KeycheckCondition>) conditions).Any<KeycheckCondition>((Func<KeycheckCondition, bool>) (c => Condition.ReplaceAndVerify(c, data)));
  }

  public static bool VerifyAny(KeycheckCondition[] conditions)
  {
    return ((IEnumerable<KeycheckCondition>) conditions).Any<KeycheckCondition>((Func<KeycheckCondition, bool>) (c => Condition.Verify(c)));
  }
}
