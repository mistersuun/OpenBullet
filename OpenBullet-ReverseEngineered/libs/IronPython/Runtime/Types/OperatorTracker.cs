// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.OperatorTracker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Types;

internal class OperatorTracker : PythonCustomTracker
{
  private MethodTracker[] _trackers;
  private bool _reversed;
  private string _name;
  private Type _declType;

  public OperatorTracker(
    Type declaringType,
    string name,
    bool reversed,
    params MethodTracker[] members)
  {
    this._declType = declaringType;
    this._reversed = reversed;
    this._trackers = members;
    this._name = name;
  }

  public override PythonTypeSlot GetSlot()
  {
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MethodTracker tracker in this._trackers)
      methodBaseList.Add((MethodBase) tracker.Method);
    MethodBase[] array = methodBaseList.ToArray();
    FunctionType functionType1 = (PythonTypeOps.GetMethodFunctionType(this.DeclaringType, (MemberInfo[]) array) | FunctionType.Method) & ~FunctionType.Function;
    FunctionType functionType2 = !this._reversed ? functionType1 & ~FunctionType.ReversedOperator : functionType1 | FunctionType.ReversedOperator;
    foreach (MemberInfo m in array)
    {
      if (!PythonHiddenAttribute.IsHidden(m))
      {
        functionType2 |= FunctionType.AlwaysVisible;
        break;
      }
    }
    return PythonTypeOps.GetFinalSlotForFunction(PythonTypeOps.GetBuiltinFunction(this.DeclaringType, this.Name, new FunctionType?(functionType2), (MemberInfo[]) methodBaseList.ToArray()));
  }

  public override Type DeclaringType => this._declType;

  public override string Name => this._name;
}
