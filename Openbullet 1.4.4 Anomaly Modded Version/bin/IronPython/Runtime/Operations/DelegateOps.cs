// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.DelegateOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class DelegateOps
{
  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType type, object function)
  {
    if (type == null)
      throw PythonOps.TypeError("expected type for 1st param, got null");
    if (function is IDelegateConvertible delegateConvertible)
    {
      Delegate @delegate = delegateConvertible.ConvertToDelegate(type.UnderlyingSystemType);
      if ((object) @delegate != null)
        return (object) @delegate;
    }
    return (object) context.LanguageContext.DelegateCreator.GetDelegate(function, type.UnderlyingSystemType);
  }

  public static Delegate InPlaceAdd(Delegate self, Delegate other)
  {
    ContractUtils.RequiresNotNull((object) self, nameof (self));
    ContractUtils.RequiresNotNull((object) other, nameof (other));
    return Delegate.Combine(self, other);
  }

  public static Delegate InPlaceSubtract(Delegate self, Delegate other)
  {
    ContractUtils.RequiresNotNull((object) self, nameof (self));
    ContractUtils.RequiresNotNull((object) other, nameof (other));
    return Delegate.Remove(self, other);
  }

  public static object Call(CodeContext context, Delegate @delegate, params object[] args)
  {
    return context.LanguageContext.CallSplat((object) @delegate, args);
  }

  public static object Call(
    CodeContext context,
    Delegate @delegate,
    [ParamDictionary] IDictionary<object, object> dict,
    params object[] args)
  {
    return context.LanguageContext.CallWithKeywords((object) @delegate, args, dict);
  }
}
