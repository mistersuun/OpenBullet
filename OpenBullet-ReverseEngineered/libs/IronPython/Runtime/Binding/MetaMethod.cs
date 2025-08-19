// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaMethod
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaMethod(Expression expression, BindingRestrictions restrictions, Method value) : 
  MetaPythonObject(expression, BindingRestrictions.Empty, (object) value),
  IPythonInvokable,
  IPythonConvertible
{
  public DynamicMetaObject Invoke(
    PythonInvokeBinder pythonInvoke,
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) pythonInvoke, args);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder action,
    DynamicMetaObject[] args)
  {
    return BindingHelpers.GenericInvokeMember(action, (ValidationInfo) null, (DynamicMetaObject) this, args);
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder callAction, DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) callAction, args);
  }

  public override DynamicMetaObject BindConvert(ConvertBinder conversion)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) conversion, conversion.Type, conversion.Explicit ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast);
  }

  public DynamicMetaObject BindConvert(PythonConversionBinder binder)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) binder, binder.Type, binder.ResultKind);
  }

  public DynamicMetaObject ConvertWorker(
    DynamicMetaObjectBinder binder,
    Type toType,
    ConversionResultKind kind)
  {
    return toType.IsSubclassOf(typeof (Delegate)) ? MetaPythonObject.MakeDelegateTarget(binder, toType, this.Restrict(typeof (Method))) : this.FallbackConvert(binder);
  }

  private DynamicMetaObject InvokeWorker(
    DynamicMetaObjectBinder callAction,
    DynamicMetaObject[] args)
  {
    CallSignature callSignature = BindingHelpers.GetCallSignature(callAction);
    DynamicMetaObject self = this.Restrict(typeof (Method));
    BindingRestrictions restrictions1 = self.Restrictions;
    DynamicMetaObject metaFunction = this.GetMetaFunction(self);
    BindingRestrictions restrictions2;
    DynamicMetaObject dynamicMetaObject;
    if (this.Value.im_self == null)
    {
      restrictions2 = restrictions1.Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal((Expression) MetaMethod.GetSelfExpression(self), Microsoft.Scripting.Ast.Utils.Constant((object) null))));
      dynamicMetaObject = args.Length != 0 ? new DynamicMetaObject((Expression) Expression.Block(this.MakeCheckSelf(callAction, callSignature, args), (Expression) Expression.Dynamic(PythonContext.GetPythonContext(callAction).Invoke(BindingHelpers.GetCallSignature(callAction)).GetLightExceptionBinder(callAction.SupportsLightThrow()), typeof (object), ArrayUtils.Insert<Expression>(PythonContext.GetCodeContext(callAction), DynamicUtils.GetExpressions(ArrayUtils.Insert<DynamicMetaObject>(metaFunction, args))))), BindingRestrictions.Empty) : new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MethodCheckSelf"), PythonContext.GetCodeContext(callAction), self.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) null)), restrictions2);
    }
    else
    {
      restrictions2 = restrictions1.Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.NotEqual((Expression) MetaMethod.GetSelfExpression(self), Microsoft.Scripting.Ast.Utils.Constant((object) null))));
      DynamicMetaObject metaSelf = this.GetMetaSelf(self);
      DynamicMetaObject[] objects = ArrayUtils.Insert<DynamicMetaObject>(metaFunction, metaSelf, args);
      CallSignature signature = new CallSignature(ArrayUtils.Insert<Argument>(new Argument(ArgumentType.Simple), callSignature.GetArgumentInfos()));
      dynamicMetaObject = new DynamicMetaObject((Expression) Expression.Dynamic(PythonContext.GetPythonContext(callAction).Invoke(signature).GetLightExceptionBinder(callAction.SupportsLightThrow()), typeof (object), ArrayUtils.Insert<Expression>(PythonContext.GetCodeContext(callAction), DynamicUtils.GetExpressions(objects))), BindingRestrictions.Empty);
    }
    return dynamicMetaObject.HasValue ? new DynamicMetaObject(dynamicMetaObject.Expression, restrictions2.Merge(dynamicMetaObject.Restrictions), dynamicMetaObject.Value) : new DynamicMetaObject(dynamicMetaObject.Expression, restrictions2.Merge(dynamicMetaObject.Restrictions));
  }

  private DynamicMetaObject GetMetaSelf(DynamicMetaObject self)
  {
    return !(this.Value.im_self is IDynamicMetaObjectProvider imSelf) ? (this.Value.im_self != null ? new DynamicMetaObject((Expression) MetaMethod.GetSelfExpression(self), BindingRestrictions.Empty, this.Value.im_self) : new DynamicMetaObject((Expression) MetaMethod.GetSelfExpression(self), BindingRestrictions.Empty)) : imSelf.GetMetaObject((Expression) MetaMethod.GetSelfExpression(self));
  }

  private DynamicMetaObject GetMetaFunction(DynamicMetaObject self)
  {
    return !(this.Value.im_func is IDynamicMetaObjectProvider imFunc) ? new DynamicMetaObject((Expression) MetaMethod.GetFunctionExpression(self), BindingRestrictions.Empty) : imFunc.GetMetaObject((Expression) MetaMethod.GetFunctionExpression(self));
  }

  private static MemberExpression GetFunctionExpression(DynamicMetaObject self)
  {
    return Expression.Property(self.Expression, typeof (Method).GetProperty("im_func"));
  }

  private static MemberExpression GetSelfExpression(DynamicMetaObject self)
  {
    return Expression.Property(self.Expression, typeof (Method).GetProperty("im_self"));
  }

  public Method Value => (Method) base.Value;

  private Expression MakeCheckSelf(
    DynamicMetaObjectBinder binder,
    CallSignature signature,
    DynamicMetaObject[] args)
  {
    Expression expression;
    switch (signature.GetArgumentKind(0))
    {
      case ArgumentType.Simple:
      case ArgumentType.Instance:
        expression = MetaMethod.CheckSelf(binder, Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (Method)), args[0].Expression);
        break;
      case ArgumentType.List:
        expression = MetaMethod.CheckSelf(binder, Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (Method)), (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.TypeIs(args[0].Expression, typeof (IList<object>)), (Expression) Expression.NotEqual((Expression) Expression.Property((Expression) Expression.Convert(args[0].Expression, typeof (ICollection)), typeof (ICollection).GetProperty("Count")), Microsoft.Scripting.Ast.Utils.Constant((object) 0))), (Expression) Expression.Call((Expression) Expression.Convert(args[0].Expression, typeof (IList<object>)), typeof (IList<object>).GetMethod("get_Item"), Microsoft.Scripting.Ast.Utils.Constant((object) 0)), Microsoft.Scripting.Ast.Utils.Constant((object) null)));
        break;
      default:
        expression = MetaMethod.CheckSelf(binder, Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (Method)), Microsoft.Scripting.Ast.Utils.Constant((object) null));
        break;
    }
    return expression;
  }

  private static Expression CheckSelf(
    DynamicMetaObjectBinder binder,
    Expression method,
    Expression inst)
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MethodCheckSelf"), PythonContext.GetCodeContext(binder), method, Microsoft.Scripting.Ast.Utils.Convert(inst, typeof (object)));
  }
}
