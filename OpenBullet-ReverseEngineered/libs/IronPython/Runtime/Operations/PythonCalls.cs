// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.PythonCalls
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class PythonCalls
{
  public static object Call(object func, params object[] args)
  {
    return DefaultContext.DefaultPythonContext.CallSplat(func, args);
  }

  public static object Call(CodeContext context, object func)
  {
    return context.LanguageContext.Call(context, func);
  }

  public static object Call(CodeContext context, object func, object arg0)
  {
    return context.LanguageContext.Call(context, func, arg0);
  }

  public static object Call(CodeContext context, object func, object arg0, object arg1)
  {
    return context.LanguageContext.Call(context, func, arg0, arg1);
  }

  public static object Call(CodeContext context, object func, params object[] args)
  {
    return context.LanguageContext.CallSplat(func, args);
  }

  public static object CallWithKeywordArgs(
    CodeContext context,
    object func,
    object[] args,
    string[] names)
  {
    PythonDictionary dict = new PythonDictionary();
    for (int index = 0; index < names.Length; ++index)
      dict[(object) names[index]] = args[args.Length - names.Length + index];
    object[] args1 = new object[args.Length - names.Length];
    for (int index = 0; index < args1.Length; ++index)
      args1[index] = args[index];
    return PythonCalls.CallWithKeywordArgs(context, func, args1, (IDictionary<object, object>) dict);
  }

  public static object CallWithKeywordArgs(
    CodeContext context,
    object func,
    object[] args,
    IDictionary<object, object> dict)
  {
    return context.LanguageContext.CallWithKeywords(func, args, dict);
  }
}
