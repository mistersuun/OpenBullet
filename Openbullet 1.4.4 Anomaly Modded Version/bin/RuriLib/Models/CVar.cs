// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.CVar
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace RuriLib.Models;

public class CVar
{
  public string Name { get; set; }

  public object Value { get; set; }

  public bool IsCapture { get; set; }

  public CVar.VarType Type { get; set; }

  public bool Hidden { get; set; }

  public CVar()
  {
  }

  public CVar(string name, string value, bool isCapture = false, bool hidden = false)
  {
    this.Name = name;
    this.Value = (object) value;
    this.IsCapture = isCapture;
    this.Type = CVar.VarType.Single;
    this.Hidden = hidden;
  }

  public CVar(string name, List<string> value, bool isCapture = false, bool hidden = false)
  {
    this.Name = name;
    this.Value = (object) value;
    this.IsCapture = isCapture;
    this.Type = CVar.VarType.List;
    this.Hidden = hidden;
  }

  public CVar(string name, Dictionary<string, string> value, bool isCapture = false, bool hidden = false)
  {
    this.Name = name;
    this.Value = (object) value;
    this.IsCapture = isCapture;
    this.Type = CVar.VarType.Dictionary;
    this.Hidden = hidden;
  }

  public CVar(string name, CVar.VarType type, object value, bool isCapture = false, bool hidden = false)
  {
    this.Name = name;
    this.Value = value;
    this.IsCapture = isCapture;
    this.Type = type;
    this.Hidden = hidden;
  }

  public override string ToString()
  {
    switch (this.Type)
    {
      case CVar.VarType.Single:
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (CVar)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return CVar.\u003C\u003Eo__26.\u003C\u003Ep__0.Target((CallSite) CVar.\u003C\u003Eo__26.\u003C\u003Ep__0, this.Value);
      case CVar.VarType.List:
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target1 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p3 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__3;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, System.Type, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, System.Type, object> target2 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__2.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, System.Type, object>> p2 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__2;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetType", (IEnumerable<System.Type>) null, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__1.Target((CallSite) CVar.\u003C\u003Eo__26.\u003C\u003Ep__1, this.Value);
        System.Type type1 = typeof (List<string>);
        object obj2 = target2((CallSite) p2, obj1, type1);
        if (target1((CallSite) p3, obj2))
        {
          // ISSUE: reference to a compiler-generated field
          if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            CVar.\u003C\u003Eo__26.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, List<string>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (List<string>), typeof (CVar)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return $"[{string.Join(", ", (IEnumerable<string>) CVar.\u003C\u003Eo__26.\u003C\u003Ep__4.Target((CallSite) CVar.\u003C\u003Eo__26.\u003C\u003Ep__4, this.Value))}]";
        }
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target3 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__7.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p7 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__7;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, System.Type, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, System.Type, object> target4 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, System.Type, object>> p6 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__6;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "GetType", (IEnumerable<System.Type>) null, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__5.Target((CallSite) CVar.\u003C\u003Eo__26.\u003C\u003Ep__5, this.Value);
        System.Type type2 = typeof (object[]);
        object obj4 = target4((CallSite) p6, obj3, type2);
        if (!target3((CallSite) p7, obj4))
          return "";
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (CVar)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target5 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__11.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p11 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__11;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Add, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string, object> target6 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__10.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string, object>> p10 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__10;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__9 = CallSite<Func<CallSite, string, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Add, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, string, object, object> target7 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__9.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, string, object, object>> p9 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__9;
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__8 = CallSite<Func<CallSite, System.Type, string, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Join", (IEnumerable<System.Type>) null, typeof (CVar), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = CVar.\u003C\u003Eo__26.\u003C\u003Ep__8.Target((CallSite) CVar.\u003C\u003Eo__26.\u003C\u003Ep__8, typeof (string), ", ", this.Value);
        object obj6 = target7((CallSite) p9, "[", obj5);
        object obj7 = target6((CallSite) p10, obj6, "]");
        return target5((CallSite) p11, obj7);
      case CVar.VarType.Dictionary:
        // ISSUE: reference to a compiler-generated field
        if (CVar.\u003C\u003Eo__26.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          CVar.\u003C\u003Eo__26.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, Dictionary<string, string>>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Dictionary<string, string>), typeof (CVar)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return $"{{{string.Join(", ", CVar.\u003C\u003Eo__26.\u003C\u003Ep__12.Target((CallSite) CVar.\u003C\u003Eo__26.\u003C\u003Ep__12, this.Value).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (d => $"({d.Key}, {d.Value})")))}}}";
      default:
        return base.ToString();
    }
  }

  public string GetListItem(int index)
  {
    if (this.Type != CVar.VarType.List)
      return (string) null;
    List<string> stringList = this.Value as List<string>;
    return index > stringList.Count - 1 || index < 0 ? (string) null : stringList[index];
  }

  public string GetDictValue(string key)
  {
    Dictionary<string, string> source = this.Value as Dictionary<string, string>;
    if (source.ContainsKey(key))
      return source.First<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (d => d.Key == key)).Value;
    throw new Exception("Key not in dictionary");
  }

  public string GetDictKey(string value)
  {
    Dictionary<string, string> source = this.Value as Dictionary<string, string>;
    if (source.ContainsValue(value))
      return source.First<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (d => d.Value == value)).Key;
    throw new Exception("Value not in dictionary");
  }

  public enum VarType
  {
    Single,
    List,
    Dictionary,
  }
}
