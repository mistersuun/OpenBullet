// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.VariableList
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using RuriLib.Functions.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace RuriLib.Models;

public class VariableList
{
  public List<CVar> All { get; set; }

  [JsonIgnore]
  public List<CVar> Captures
  {
    get => this.All.Where<CVar>((Func<CVar, bool>) (v => v.IsCapture && !v.Hidden)).ToList<CVar>();
  }

  [JsonIgnore]
  public List<CVar> Singles
  {
    get
    {
      return this.All.Where<CVar>((Func<CVar, bool>) (v => v.Type == CVar.VarType.Single)).ToList<CVar>();
    }
  }

  [JsonIgnore]
  public List<CVar> Lists
  {
    get
    {
      return this.All.Where<CVar>((Func<CVar, bool>) (v => v.Type == CVar.VarType.List)).ToList<CVar>();
    }
  }

  [JsonIgnore]
  public List<CVar> Dictionaries
  {
    get
    {
      return this.All.Where<CVar>((Func<CVar, bool>) (v => v.Type == CVar.VarType.Dictionary)).ToList<CVar>();
    }
  }

  public VariableList() => this.All = new List<CVar>();

  public VariableList(List<CVar> list) => this.All = list;

  public CVar Get(string name)
  {
    return this.All.FirstOrDefault<CVar>((Func<CVar, bool>) (v => v.Name == name));
  }

  public CVar Get(string name, CVar.VarType type)
  {
    return this.All.FirstOrDefault<CVar>((Func<CVar, bool>) (v => v.Name == name && v.Type == type));
  }

  public string GetSingle(string name)
  {
    try
    {
      // ISSUE: reference to a compiler-generated field
      if (VariableList.\u003C\u003Eo__16.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VariableList.\u003C\u003Eo__16.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (string), typeof (VariableList)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return VariableList.\u003C\u003Eo__16.\u003C\u003Ep__0.Target((CallSite) VariableList.\u003C\u003Eo__16.\u003C\u003Ep__0, this.Get(name, CVar.VarType.Single).Value);
    }
    catch
    {
      return (string) null;
    }
  }

  public List<string> GetList(string name)
  {
    try
    {
      // ISSUE: reference to a compiler-generated field
      if (VariableList.\u003C\u003Eo__17.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VariableList.\u003C\u003Eo__17.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, List<string>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (List<string>), typeof (VariableList)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return VariableList.\u003C\u003Eo__17.\u003C\u003Ep__0.Target((CallSite) VariableList.\u003C\u003Eo__17.\u003C\u003Ep__0, this.Get(name, CVar.VarType.List).Value);
    }
    catch
    {
      return (List<string>) null;
    }
  }

  public Dictionary<string, string> GetDictionary(string name)
  {
    try
    {
      // ISSUE: reference to a compiler-generated field
      if (VariableList.\u003C\u003Eo__18.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VariableList.\u003C\u003Eo__18.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, Dictionary<string, string>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Dictionary<string, string>), typeof (VariableList)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return VariableList.\u003C\u003Eo__18.\u003C\u003Ep__0.Target((CallSite) VariableList.\u003C\u003Eo__18.\u003C\u003Ep__0, this.Get(name, CVar.VarType.Dictionary).Value);
    }
    catch
    {
      return (Dictionary<string, string>) null;
    }
  }

  public bool VariableExists(string name)
  {
    return this.All.Any<CVar>((Func<CVar, bool>) (v => v.Name == name));
  }

  public bool VariableExists(string name, CVar.VarType type)
  {
    return this.All.Any<CVar>((Func<CVar, bool>) (v => v.Name == name && v.Type == type));
  }

  public void Set(CVar variable)
  {
    this.Remove(variable.Name);
    this.All.Add(variable);
  }

  public void SetHidden(string name, object value)
  {
    if (this.All.Any<CVar>((Func<CVar, bool>) (v => v.Name == name && v.Hidden)))
    {
      this.All.FirstOrDefault<CVar>((Func<CVar, bool>) (v => v.Name == name && v.Hidden)).Value = value;
    }
    else
    {
      List<CVar> all = this.All;
      // ISSUE: reference to a compiler-generated field
      if (VariableList.\u003C\u003Eo__22.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VariableList.\u003C\u003Eo__22.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, string, object, bool, bool, CVar>>.Create(Binder.InvokeConstructor(CSharpBinderFlags.None, typeof (VariableList), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      CVar cvar = VariableList.\u003C\u003Eo__22.\u003C\u003Ep__0.Target((CallSite) VariableList.\u003C\u003Eo__22.\u003C\u003Ep__0, typeof (CVar), name, value, false, true);
      all.Add(cvar);
    }
  }

  public void SetNew(CVar variable)
  {
    if (this.VariableExists(variable.Name))
      return;
    this.Set(variable);
  }

  public void Remove(string name)
  {
    this.All.RemoveAll((Predicate<CVar>) (v => v.Name == name && !v.Hidden));
  }

  public void Remove(Comparer comparer, string name, BotData data)
  {
    this.All.RemoveAll((Predicate<CVar>) (v => Condition.ReplaceAndVerify(v.Name, comparer, name, data) && !v.Hidden));
  }

  public string ToCaptureString()
  {
    return string.Join(" | ", this.Captures.Select<CVar, string>((Func<CVar, string>) (c => $"{c.Name} = {c.ToString()}")));
  }
}
