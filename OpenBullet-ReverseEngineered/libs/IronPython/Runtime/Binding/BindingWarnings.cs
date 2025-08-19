// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.BindingWarnings
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using System;
using System.Linq.Expressions;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Binding;

internal static class BindingWarnings
{
  public static bool ShouldWarn(PythonContext context, OverloadInfo method, out WarningInfo info)
  {
    ObsoleteAttribute[] customAttributes1 = (ObsoleteAttribute[]) method.ReflectionInfo.GetCustomAttributes(typeof (ObsoleteAttribute), true);
    if (customAttributes1.Length != 0)
    {
      info = new WarningInfo(PythonExceptions.DeprecationWarning, $"{NameConverter.GetTypeName(method.DeclaringType)}.{method.Name} has been obsoleted.  {customAttributes1[0].Message}");
      return true;
    }
    if (context.PythonOptions.WarnPython30)
    {
      Python3WarningAttribute[] customAttributes2 = (Python3WarningAttribute[]) method.ReflectionInfo.GetCustomAttributes(typeof (Python3WarningAttribute), true);
      if (customAttributes2.Length != 0)
      {
        info = new WarningInfo(PythonExceptions.DeprecationWarning, customAttributes2[0].Message);
        return true;
      }
    }
    if (method.DeclaringType == typeof (Thread) && method.Name == "Sleep")
    {
      info = new WarningInfo(PythonExceptions.RuntimeWarning, "Calling Thread.Sleep on an STA thread doesn't pump messages.  Use Thread.CurrentThread.Join instead.", (Expression) Expression.Equal((Expression) Expression.Call((Expression) Expression.Property((Expression) null, typeof (Thread).GetProperty("CurrentThread")), typeof (Thread).GetMethod("GetApartmentState")), Utils.Constant((object) ApartmentState.STA)));
      return true;
    }
    info = (WarningInfo) null;
    return false;
  }
}
