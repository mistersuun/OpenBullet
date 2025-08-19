// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ClassMethodTracker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime.Types;

internal class ClassMethodTracker : PythonCustomTracker
{
  private MethodTracker[] _trackers;

  public ClassMethodTracker(MemberGroup group)
  {
    List<MethodTracker> methodTrackerList = new List<MethodTracker>(group.Count);
    foreach (MethodTracker methodTracker in group)
      methodTrackerList.Add(methodTracker);
    this._trackers = methodTrackerList.ToArray();
  }

  public override PythonTypeSlot GetSlot()
  {
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MethodTracker tracker in this._trackers)
      methodBaseList.Add((MethodBase) tracker.Method);
    return PythonTypeOps.GetFinalSlotForFunction(PythonTypeOps.GetBuiltinFunction(this.DeclaringType, this.Name, (MemberInfo[]) methodBaseList.ToArray()));
  }

  public override DynamicMetaObject GetValue(
    OverloadResolverFactory factory,
    ActionBinder binder,
    Type instanceType)
  {
    return this.GetBoundValue(factory, binder, instanceType, new DynamicMetaObject(Utils.Constant((object) null), BindingRestrictions.Empty));
  }

  public override Type DeclaringType => this._trackers[0].DeclaringType;

  public override string Name => this._trackers[0].Name;
}
