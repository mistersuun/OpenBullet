// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaBuiltinMethodDescriptor
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaBuiltinMethodDescriptor(
  Expression expression,
  BindingRestrictions restrictions,
  BuiltinMethodDescriptor value) : MetaPythonObject(expression, BindingRestrictions.Empty, (object) value), IPythonInvokable, IPythonOperable
{
  public DynamicMetaObject Invoke(
    PythonInvokeBinder pythonInvoke,
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) pythonInvoke, codeContext, args);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder action,
    DynamicMetaObject[] args)
  {
    return BindingHelpers.GenericInvokeMember(action, (ValidationInfo) null, (DynamicMetaObject) this, args);
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder call, DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) call, PythonContext.GetCodeContext((DynamicMetaObjectBinder) call), args);
  }

  private DynamicMetaObject InvokeWorker(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    CallSignature signature = BindingHelpers.GetCallSignature(call);
    BindingRestrictions selfRestrict = BindingRestrictions.GetInstanceRestriction(this.Expression, (object) this.Value).Merge(this.Restrictions);
    selfRestrict = selfRestrict.Merge(BindingRestrictions.GetExpressionRestriction(this.MakeFunctionTest((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetBuiltinMethodDescriptorTemplate"), (Expression) Expression.Convert(this.Expression, typeof (BuiltinMethodDescriptor))))));
    return this.Value.Template.MakeBuiltinFunctionCall(call, codeContext, (DynamicMetaObject) this, args, false, selfRestrict, (Func<DynamicMetaObject[], BuiltinFunction.BindingResult>) (newArgs =>
    {
      PythonContext pythonContext = PythonContext.GetPythonContext(call);
      BindingTarget target;
      return BindingHelpers.CheckLightThrow(call, pythonContext.Binder.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(pythonContext.Binder, (IList<DynamicMetaObject>) newArgs, signature, codeContext), this.Value.Template.Targets, selfRestrict, this.Value.Template.Name, NarrowingLevel.None, this.Value.Template.IsBinaryOperator ? NarrowingLevel.Two : NarrowingLevel.All, out target), target);
    }));
  }

  internal Expression MakeFunctionTest(Expression functionTarget)
  {
    return (Expression) Expression.Equal(functionTarget, Utils.Constant((object) this.Value.Template));
  }

  public BuiltinMethodDescriptor Value => (BuiltinMethodDescriptor) base.Value;

  DynamicMetaObject IPythonOperable.BindOperation(
    PythonOperationBinder action,
    DynamicMetaObject[] args)
  {
    return action.Operation == PythonOperationKind.CallSignatures ? PythonProtocol.MakeCallSignatureOperation((DynamicMetaObject) this, this.Value.Template.Targets) : (DynamicMetaObject) null;
  }
}
