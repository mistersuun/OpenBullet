// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.CustomAttributeTracker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using System;
using System.Dynamic;

#nullable disable
namespace IronPython.Runtime.Types;

internal class CustomAttributeTracker : PythonCustomTracker
{
  private readonly PythonTypeSlot _slot;
  private readonly Type _declType;
  private readonly string _name;

  public CustomAttributeTracker(Type declaringType, string name, PythonTypeSlot slot)
  {
    this._declType = declaringType;
    this._name = name;
    this._slot = slot;
  }

  public override DynamicMetaObject GetValue(
    OverloadResolverFactory factory,
    ActionBinder binder,
    Type instanceType)
  {
    return this.GetBoundValue(factory, binder, instanceType, new DynamicMetaObject(Utils.Constant((object) null), BindingRestrictions.Empty));
  }

  public override string Name => this._name;

  public override Type DeclaringType => this._declType;

  public override PythonTypeSlot GetSlot() => this._slot;
}
