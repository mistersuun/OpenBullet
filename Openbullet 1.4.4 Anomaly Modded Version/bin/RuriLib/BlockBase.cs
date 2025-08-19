// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockBase
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public abstract class BlockBase : ViewModelBase
{
  private string label = "BASE";
  private bool disabled;

  public string Label
  {
    get => this.label;
    set
    {
      this.label = value;
      this.OnPropertyChanged(nameof (Label));
    }
  }

  public bool Disabled
  {
    get => this.disabled;
    set
    {
      this.disabled = value;
      this.OnPropertyChanged(nameof (Disabled));
    }
  }

  [JsonIgnore]
  public bool IsSelenium => this.GetType().ToString().StartsWith("S");

  [JsonIgnore]
  public bool IsCaptcha
  {
    get
    {
      return this.GetType() == typeof (BlockImageCaptcha) || this.GetType() == typeof (BlockRecaptcha);
    }
  }

  public virtual BlockBase FromLS(string line)
  {
    throw new Exception("Cannot Convert to the abstract class BlockBase");
  }

  public virtual BlockBase FromLS(List<string> lines)
  {
    throw new Exception("Cannot Convert from the abstract class BlockBase");
  }

  public virtual string ToLS(bool indent = true)
  {
    throw new Exception("Cannot Convert from the abstract class BlockBase");
  }

  public virtual void Process(BotData data)
  {
    data.LogBuffer.Clear();
    data.Log(new LogEntry($"<--- Executing Block {this.Label} --->", Colors.Orange));
  }

  public static List<string> ReplaceValuesRecursive(string input, BotData data)
  {
    List<string> source1 = new List<string>();
    MatchCollection matchCollection = Regex.Matches(input, "<([^\\[]*)\\[\\*\\]>");
    List<CVar> source2 = new List<CVar>();
    foreach (Match match in matchCollection)
    {
      string name = match.Groups[1].Value;
      CVar cvar = data.Variables.Get(name);
      if (cvar == null)
      {
        cvar = data.GlobalVariables.Get(name);
        if (cvar == null)
          continue;
      }
      if (cvar.Type == CVar.VarType.List)
        source2.Add(cvar);
    }
    if (source2.Count > 0)
    {
      // ISSUE: reference to a compiler-generated field
      if (BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (BlockBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__1.Target((CallSite) BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__1, source2.OrderBy<CVar, object>((Func<CVar, object>) (v =>
      {
        // ISSUE: reference to a compiler-generated field
        if (BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (BlockBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__0.Target((CallSite) BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__0, v.Value);
      })).Last<CVar>().Value);
      int index = 0;
      while (true)
      {
        // ISSUE: reference to a compiler-generated field
        if (BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (BlockBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target = BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p3 = BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__3;
        // ISSUE: reference to a compiler-generated field
        if (BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__2 = CallSite<Func<CallSite, int, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.LessThan, typeof (BlockBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__2.Target((CallSite) BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__2, index, obj1);
        if (target((CallSite) p3, obj2))
        {
          string str = input;
          foreach (CVar cvar in source2)
          {
            // ISSUE: reference to a compiler-generated field
            if (BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__4 == null)
            {
              // ISSUE: reference to a compiler-generated field
              BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, List<string>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (List<string>), typeof (BlockBase)));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            List<string> stringList = BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__4.Target((CallSite) BlockBase.\u003C\u003Eo__16.\u003C\u003Ep__4, cvar.Value);
            str = stringList.Count <= index ? str.Replace($"<{cvar.Name}[*]>", "NULL") : str.Replace($"<{cvar.Name}[*]>", stringList[index]);
          }
          source1.Add(str);
          ++index;
        }
        else
          break;
      }
    }
    else
    {
      Match match1 = Regex.Match(input, "<([^\\(]*)\\(\\*\\)>");
      if (match1.Success)
      {
        string oldValue = match1.Groups[0].Value;
        string name = match1.Groups[1].Value;
        Dictionary<string, string> dictionary = data.Variables.GetDictionary(name) ?? data.GlobalVariables.GetDictionary(name);
        if (dictionary == null)
        {
          source1.Add(input);
        }
        else
        {
          foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            source1.Add(input.Replace(oldValue, keyValuePair.Value));
        }
      }
      else
      {
        Match match2 = Regex.Match(input, "<([^\\{]*)\\{\\*\\}>");
        if (match2.Success)
        {
          string oldValue = match2.Groups[0].Value;
          string name = match2.Groups[1].Value;
          Dictionary<string, string> dictionary = data.Variables.GetDictionary(name) ?? data.GlobalVariables.GetDictionary(name);
          if (dictionary == null)
          {
            source1.Add(input);
          }
          else
          {
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
              source1.Add(input.Replace(oldValue, keyValuePair.Key));
          }
        }
        else
          source1.Add(input);
      }
    }
    return source1.Select<string, string>((Func<string, string>) (i => BlockBase.ReplaceValues(i, data))).ToList<string>();
  }

  public static string ReplaceValues(string input, BotData data)
  {
    if (!input.Contains("<") && !input.Contains(">"))
      return input;
    string input1 = input;
    string str1;
    do
    {
      str1 = input1;
      input1 = input1.Replace("<INPUT>", data.Data.Data).Replace("<STATUS>", data.Status.ToString()).Replace("<BOTNUM>", data.BotNumber.ToString()).Replace("<RETRIES>", data.Data.Retries.ToString());
      if (data.Proxy != null)
        input1 = input1.Replace("<PROXY>", data.Proxy.Proxy);
      foreach (Match match in Regex.Matches(input1, "<([^<>]*)>"))
      {
        string oldValue = match.Groups[0].Value;
        string input2 = match.Groups[1].Value;
        string str2 = Regex.Match(input2, "^[^\\[\\{\\(]*").Value;
        CVar cvar = data.Variables.Get(str2) ?? data.GlobalVariables.Get(str2);
        if (cvar != null)
        {
          string input3 = input2.Replace(str2, "");
          switch (cvar.Type)
          {
            case CVar.VarType.Single:
              // ISSUE: reference to a compiler-generated field
              if (BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__1 == null)
              {
                // ISSUE: reference to a compiler-generated field
                BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (BlockBase)));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, string> target = BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__1.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, string>> p1 = BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__1;
              // ISSUE: reference to a compiler-generated field
              if (BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__0 == null)
              {
                // ISSUE: reference to a compiler-generated field
                BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__0 = CallSite<Func<CallSite, string, string, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Replace", (IEnumerable<Type>) null, typeof (BlockBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj = BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__0.Target((CallSite) BlockBase.\u003C\u003Eo__17.\u003C\u003Ep__0, input1, oldValue, cvar.Value);
              input1 = target((CallSite) p1, obj);
              continue;
            case CVar.VarType.List:
              if (string.IsNullOrEmpty(input3))
              {
                input1 = input1.Replace(oldValue, cvar.ToString());
                continue;
              }
              int result = 0;
              int.TryParse(BlockBase.ParseArguments(input3, '[', ']')[0], out result);
              string listItem = cvar.GetListItem(result);
              if (listItem != null)
              {
                input1 = input1.Replace(oldValue, listItem);
                continue;
              }
              continue;
            case CVar.VarType.Dictionary:
              if (input3.Contains("(") && input3.Contains(")"))
              {
                string key = BlockBase.ParseArguments(input3, '(', ')')[0];
                try
                {
                  input1 = input1.Replace(oldValue, cvar.GetDictValue(key));
                  continue;
                }
                catch
                {
                  continue;
                }
              }
              else if (input3.Contains("{") && input3.Contains("}"))
              {
                string str3 = BlockBase.ParseArguments(input3, '{', '}')[0];
                try
                {
                  input1 = input1.Replace(oldValue, cvar.GetDictKey(str3));
                  continue;
                }
                catch
                {
                  continue;
                }
              }
              else
              {
                input1 = input1.Replace(oldValue, cvar.ToString());
                continue;
              }
            default:
              continue;
          }
        }
      }
    }
    while (input.Contains("<") && input.Contains(">") && input1 != str1);
    return input1;
  }

  public static List<string> ParseArguments(string input, char delimL, char delimR)
  {
    List<string> arguments = new List<string>();
    foreach (Match match in Regex.Matches(input, $"\\{delimL.ToString()}([^\\{delimR.ToString()}]*)\\{delimR.ToString()}"))
      arguments.Add(match.Groups[1].Value);
    return arguments;
  }

  public static void UpdateSeleniumData(BotData data)
  {
    data.Address = data.Driver.Url;
    data.ResponseSource = data.Driver.PageSource;
  }

  public static void InsertVariables(
    BotData data,
    bool isCapture,
    bool recursive,
    List<string> values,
    string variableName,
    string prefix,
    string suffix,
    bool urlEncode,
    bool createEmpty)
  {
    List<string> list = values.Select<string, string>((Func<string, string>) (v => BlockBase.ReplaceValues(prefix, data) + v.Trim() + BlockBase.ReplaceValues(suffix, data))).ToList<string>();
    if (urlEncode)
      list = list.Select<string, string>((Func<string, string>) (v => Uri.EscapeDataString(v))).ToList<string>();
    CVar variable = (CVar) null;
    if (recursive)
    {
      if (list.Count == 0)
      {
        if (createEmpty)
          variable = new CVar(variableName, list, isCapture);
      }
      else
        variable = new CVar(variableName, list, isCapture);
    }
    else if (list.Count == 0)
    {
      if (createEmpty)
        variable = new CVar(variableName, "", isCapture);
    }
    else
      variable = new CVar(variableName, list.First<string>(), isCapture);
    if (variable != null)
    {
      data.Variables.Set(variable);
      data.Log(new LogEntry($"Parsed variable | Name: {variable.Name} | Value: {variable.ToString()}{Environment.NewLine}", isCapture ? Colors.OrangeRed : Colors.Gold));
    }
    else
      data.Log(new LogEntry("Could not parse any data. The variable was not created.", Colors.White));
  }

  public static string TruncatePretty(string input, int max)
  {
    input = input.Replace("\r\n", "").Replace("\n", "");
    return input.Length < max ? input : input.Substring(0, max) + " [...]";
  }
}
