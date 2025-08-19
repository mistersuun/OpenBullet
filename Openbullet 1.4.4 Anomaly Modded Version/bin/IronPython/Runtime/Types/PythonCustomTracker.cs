// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.PythonCustomTracker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Types;

public abstract class PythonCustomTracker : CustomTracker
{
  public abstract PythonTypeSlot GetSlot();

  public override DynamicMetaObject GetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type)
  {
    return new DynamicMetaObject((Expression) Utils.Constant((object) this.GetSlot(), typeof (PythonTypeSlot)), BindingRestrictions.Empty);
  }

  public override MemberTracker BindToInstance(DynamicMetaObject instance)
  {
    return (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance);
  }

  public override DynamicMetaObject SetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject value)
  {
    return this.SetBoundValue(resolverFactory, binder, type, value, new DynamicMetaObject(Utils.Constant((object) null), BindingRestrictions.Empty));
  }

  public override DynamicMetaObject SetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    return base.SetValue(resolverFactory, binder, type, value, errorSuggestion);
  }

  protected override DynamicMetaObject GetBoundValue(
    OverloadResolverFactory factory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject instance)
  {
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotGetValue"), ((PythonOverloadResolverFactory) factory)._codeContext, (Expression) Utils.Constant((object) this.GetSlot(), typeof (PythonTypeSlot)), Utils.Convert(instance.Expression, typeof (object)), Utils.Constant((object) DynamicHelpers.GetPythonTypeFromType(instanceType))), BindingRestrictions.Empty);
  }

  protected override DynamicMetaObject SetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject value,
    DynamicMetaObject instance)
  {
    return this.SetBoundValue(resolverFactory, binder, type, value, instance, (DynamicMetaObject) null);
  }

  protected override DynamicMetaObject SetBoundValue(
    OverloadResolverFactory factory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject value,
    DynamicMetaObject instance,
    DynamicMetaObject errorSuggestion)
  {
    MethodCallExpression test = Expression.Call(typeof (PythonOps).GetMethod("SlotTrySetValue"), ((PythonOverloadResolverFactory) factory)._codeContext, (Expression) Utils.Constant((object) this.GetSlot(), typeof (PythonTypeSlot)), Utils.Convert(instance.Expression, typeof (object)), Utils.Constant((object) DynamicHelpers.GetPythonTypeFromType(type)), value.Expression);
    Expression ifTrue = Utils.Convert(value.Expression, typeof (object));
    Expression ifFalse;
    if (errorSuggestion == null)
      ifFalse = (Expression) Expression.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("AttributeErrorForMissingAttribute", new Type[2]
      {
        typeof (object),
        typeof (string)
      }), instance.Expression, (Expression) Expression.Constant((object) this.Name)), typeof (object));
    else
      ifFalse = errorSuggestion.Expression;
    return new DynamicMetaObject((Expression) Expression.Condition((Expression) test, ifTrue, ifFalse), BindingRestrictions.Empty);
  }
}
