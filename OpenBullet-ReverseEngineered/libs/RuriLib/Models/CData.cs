// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.CData
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.Models;

public class CData
{
  public string Data { get; set; }

  public WordlistType Type { get; set; }

  public int Retries { get; set; }

  public bool IsValid => !this.Type.Verify || Regex.Match(this.Data, this.Type.Regex).Success;

  public CData(string data, WordlistType type)
  {
    this.Data = data;
    this.Type = type;
  }

  public List<CVar> GetVariables(bool encode)
  {
    return ((IEnumerable<string>) this.Data.Split(new string[1]
    {
      this.Type.Separator
    }, StringSplitOptions.None)).Zip((IEnumerable<string>) this.Type.Slices, (k, v) => new
    {
      k = k,
      v = v
    }).Select(x => new CVar(x.v, encode ? Uri.EscapeDataString(x.k) : x.k, hidden: true)).ToList<CVar>();
  }

  public bool RespectsRules(List<DataRule> rules)
  {
    bool flag = true;
    List<CVar> variables = this.GetVariables(false);
    foreach (DataRule rule1 in rules)
    {
      DataRule rule = rule1;
      try
      {
        // ISSUE: reference to a compiler-generated field
        if (CData.\u003C\u003Eo__16.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CData.\u003C\u003Eo__16.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (CData)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string input = CData.\u003C\u003Eo__16.\u003C\u003Ep__0.Target((CallSite) CData.\u003C\u003Eo__16.\u003C\u003Ep__0, variables.FirstOrDefault<CVar>((Func<CVar, bool>) (v => v.Name == rule.SliceName)).Value);
        switch (rule.RuleType)
        {
          case RuleType.MustContain:
            flag = CData.CheckContains(input, rule.RuleString);
            break;
          case RuleType.MustNotContain:
            flag = !CData.CheckContains(input, rule.RuleString);
            break;
          case RuleType.MinLength:
            flag = input.Length >= int.Parse(rule.RuleString);
            break;
          case RuleType.MaxLength:
            flag = input.Length <= int.Parse(rule.RuleString);
            break;
          case RuleType.MustMatchRegex:
            flag = Regex.Match(input, rule.RuleString).Success;
            break;
        }
        if (!flag)
          return false;
      }
      catch
      {
      }
    }
    return true;
  }

  private static bool CheckContains(string input, string what)
  {
    switch (what)
    {
      case "Lowercase":
        return input.Any<char>((Func<char, bool>) (c => char.IsLower(c)));
      case "Uppercase":
        return input.Any<char>((Func<char, bool>) (c => char.IsUpper(c)));
      case "Digit":
        return input.Any<char>((Func<char, bool>) (c => char.IsDigit(c)));
      case "Symbol":
        return input.Any<char>((Func<char, bool>) (c => char.IsSymbol(c) || char.IsPunctuation(c)));
      default:
        foreach (char ch in what.ToCharArray())
        {
          if (input.Contains<char>(ch))
            return true;
        }
        return false;
    }
  }
}
