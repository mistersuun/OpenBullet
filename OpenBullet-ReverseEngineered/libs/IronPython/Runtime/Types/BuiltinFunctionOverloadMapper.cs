// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.BuiltinFunctionOverloadMapper
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

public class BuiltinFunctionOverloadMapper : ICodeFormattable
{
  private BuiltinFunction _function;
  private object _instance;
  private PythonTuple _allOverloads;

  public BuiltinFunctionOverloadMapper(BuiltinFunction builtinFunction, object instance)
  {
    this._function = builtinFunction;
    this._instance = instance;
  }

  public object this[params Type[] types] => this.GetOverload(types, this.Targets);

  protected object GetOverload(Type[] sig, IList<MethodBase> targets)
  {
    return this.GetOverload(sig, targets, true);
  }

  private object GetOverload(Type[] sig, IList<MethodBase> targets, bool wrapCtors)
  {
    BuiltinFunction.TypeList key = new BuiltinFunction.TypeList(sig);
    BuiltinFunction bf;
    lock (this._function.OverloadDictionary)
    {
      if (!this._function.OverloadDictionary.TryGetValue(key, out bf))
      {
        MethodBase[] matchingTargets = BuiltinFunctionOverloadMapper.FindMatchingTargets(sig, targets, true);
        if (matchingTargets.Length == 0)
          matchingTargets = BuiltinFunctionOverloadMapper.FindMatchingTargets(sig, targets, false);
        if (matchingTargets.Length == 0)
          this.ThrowOverloadException(sig, targets);
        else
          this._function.OverloadDictionary[key] = bf = new BuiltinFunction(this._function.Name, matchingTargets, this.Function.DeclaringType, this._function.FunctionType);
      }
    }
    if (this._instance != null)
      return (object) bf.BindToInstance(this._instance);
    return wrapCtors ? this.GetTargetFunction(bf) : (object) bf;
  }

  private static MethodBase[] FindMatchingTargets(
    Type[] sig,
    IList<MethodBase> targets,
    bool removeCodeContext)
  {
    int length = sig.Length;
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MethodBase target in (IEnumerable<MethodBase>) targets)
    {
      ParameterInfo[] parameterInfoArray1 = target.GetParameters();
      if (removeCodeContext && parameterInfoArray1.Length != 0 && parameterInfoArray1[0].ParameterType == typeof (CodeContext))
      {
        ParameterInfo[] parameterInfoArray2 = new ParameterInfo[parameterInfoArray1.Length - 1];
        for (int index = 1; index < parameterInfoArray1.Length; ++index)
          parameterInfoArray2[index - 1] = parameterInfoArray1[index];
        parameterInfoArray1 = parameterInfoArray2;
      }
      if (parameterInfoArray1.Length == length)
      {
        bool flag = true;
        for (int index = 0; index < length; ++index)
        {
          if (parameterInfoArray1[index].ParameterType != sig[index])
          {
            flag = false;
            break;
          }
        }
        if (flag)
          methodBaseList.Add(target);
      }
    }
    return methodBaseList.ToArray();
  }

  public void ThrowOverloadException(Type[] sig, IList<MethodBase> targets)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.Append((targets.Count > 0 ? targets[0].Name : "") + "[");
    foreach (Type type in sig)
    {
      if (!stringBuilder1.ToString().endswith((object) "["))
        stringBuilder1.Append(", ");
      stringBuilder1.Append(type.Name);
    }
    stringBuilder1.Append("]");
    StringBuilder stringBuilder2 = new StringBuilder();
    foreach (MethodBase target in (IEnumerable<MethodBase>) targets)
    {
      if (stringBuilder2.Length > 0)
        stringBuilder2.Append(", ");
      stringBuilder2.Append("[");
      foreach (ParameterInfo parameter in target.GetParameters())
      {
        if (!stringBuilder2.ToString().endswith((object) "["))
          stringBuilder2.Append(", ");
        stringBuilder2.Append(parameter.ParameterType.Name);
      }
      stringBuilder2.Append("]");
    }
    throw ScriptingRuntimeHelpers.SimpleTypeError($"No match found for the method signature {stringBuilder1.ToString()}. Expected {stringBuilder2.ToString()}");
  }

  public BuiltinFunction Function => this._function;

  public virtual IList<MethodBase> Targets => this._function.Targets;

  public PythonTuple Functions
  {
    get
    {
      if (this._allOverloads == null)
      {
        object[] objArray = new object[this.Targets.Count];
        int num = 0;
        foreach (MethodBase target in (IEnumerable<MethodBase>) this.Targets)
        {
          ParameterInfo[] parameters = target.GetParameters();
          Type[] sig = new Type[parameters.Length];
          for (int index = 0; index < parameters.Length; ++index)
            sig[index] = parameters[index].ParameterType;
          objArray[num++] = this.GetOverload(sig, this.Targets, false);
        }
        Interlocked.CompareExchange<PythonTuple>(ref this._allOverloads, PythonTuple.MakeTuple(objArray), (PythonTuple) null);
      }
      return this._allOverloads;
    }
  }

  protected virtual object GetTargetFunction(BuiltinFunction bf) => (object) bf;

  public virtual string __repr__(CodeContext context)
  {
    PythonDictionary pythonDictionary = new PythonDictionary();
    foreach (MethodBase target in (IEnumerable<MethodBase>) this.Targets)
    {
      string autoDoc = DocBuilder.CreateAutoDoc(target);
      pythonDictionary[(object) autoDoc] = (object) this.Function;
    }
    return pythonDictionary.__repr__(context);
  }
}
