// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaBuiltinFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaBuiltinFunction(
  Expression expression,
  BindingRestrictions restrictions,
  BuiltinFunction value) : MetaPythonObject(expression, BindingRestrictions.Empty, (object) value), IPythonInvokable, IPythonOperable, IPythonConvertible
{
  public override DynamicMetaObject BindInvoke(InvokeBinder call, DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) call, PythonContext.GetCodeContext((DynamicMetaObjectBinder) call), args);
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
    return toType.IsSubclassOf(typeof (Delegate)) ? MetaPythonObject.MakeDelegateTarget(binder, toType, this.Restrict(this.LimitType)) : this.FallbackConvert(binder);
  }

  DynamicMetaObject IPythonOperable.BindOperation(
    PythonOperationBinder action,
    DynamicMetaObject[] args)
  {
    return action.Operation == PythonOperationKind.CallSignatures ? PythonProtocol.MakeCallSignatureOperation((DynamicMetaObject) this, this.Value.Targets) : (DynamicMetaObject) null;
  }

  public DynamicMetaObject Invoke(
    PythonInvokeBinder pythonInvoke,
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) pythonInvoke, codeContext, args);
  }

  private DynamicMetaObject InvokeWorker(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    if (this.NeedsDeferral())
      return call.Defer(ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, args));
    for (int index = 0; index < args.Length; ++index)
    {
      if (args[index].NeedsDeferral())
        return call.Defer(ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, args));
    }
    return this.Value.IsUnbound ? this.MakeSelflessCall(call, codeContext, args) : this.MakeSelfCall(call, codeContext, args);
  }

  private DynamicMetaObject MakeSelflessCall(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    BindingRestrictions selfRestrict = BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal(this.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) this.Value))).Merge(this.Restrictions);
    return this.Value.MakeBuiltinFunctionCall(call, codeContext, (DynamicMetaObject) this, args, false, selfRestrict, (Func<DynamicMetaObject[], BuiltinFunction.BindingResult>) (newArgs =>
    {
      PythonBinder binder = PythonContext.GetPythonContext(call).Binder;
      BindingTarget target;
      return BindingHelpers.CheckLightThrow(call, binder.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(binder, (IList<DynamicMetaObject>) newArgs, BindingHelpers.GetCallSignature(call), codeContext), this.Value.Targets, selfRestrict, this.Value.Name, NarrowingLevel.None, this.Value.IsBinaryOperator ? NarrowingLevel.Two : NarrowingLevel.All, out target), target);
    }));
  }

  private DynamicMetaObject MakeSelfCall(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    BindingRestrictions functionRestriction = this.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(this.Expression, this.LimitType)).Merge(BindingRestrictions.GetExpressionRestriction(this.Value.MakeBoundFunctionTest(Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (BuiltinFunction)))));
    DynamicMetaObject self = this.GetInstance((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetBuiltinFunctionSelf"), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (BuiltinFunction))), CompilerHelpers.GetType(this.Value.BindingSelf));
    return this.Value.MakeBuiltinFunctionCall(call, codeContext, (DynamicMetaObject) this, ArrayUtils.Insert<DynamicMetaObject>(self, args), true, functionRestriction, (Func<DynamicMetaObject[], BuiltinFunction.BindingResult>) (newArgs =>
    {
      CallSignature callSignature = BindingHelpers.GetCallSignature(call);
      PythonContext pythonContext = PythonContext.GetPythonContext(call);
      PythonOverloadResolver resolver = !this.Value.IsReversedOperator ? new PythonOverloadResolver(pythonContext.Binder, self, (IList<DynamicMetaObject>) args, callSignature, codeContext) : new PythonOverloadResolver(pythonContext.Binder, (IList<DynamicMetaObject>) newArgs, MetaBuiltinFunction.GetReversedSignature(callSignature), codeContext);
      BindingTarget target;
      return BindingHelpers.CheckLightThrow(call, pythonContext.Binder.CallMethod((DefaultOverloadResolver) resolver, this.Value.Targets, self.Restrictions, this.Value.Name, NarrowingLevel.None, this.Value.IsBinaryOperator ? NarrowingLevel.Two : NarrowingLevel.All, out target), target);
    }));
  }

  private DynamicMetaObject GetInstance(Expression instance, Type testType)
  {
    object bindingSelf = this.Value.BindingSelf;
    BindingRestrictions runtimeTypeRestriction = BindingRestrictionsHelpers.GetRuntimeTypeRestriction(instance, testType);
    if (CompilerHelpers.IsStrongBox(bindingSelf))
    {
      instance = (Expression) this.ReadStrongBoxValue(instance);
      bindingSelf = ((IStrongBox) bindingSelf).Value;
    }
    else if (!testType.IsEnum())
    {
      Type type1 = CompilerHelpers.GetVisibleType(CompilerHelpers.GetType(this.Value.BindingSelf));
      if (type1 == typeof (object) && this.Value.DeclaringType.IsInterface())
      {
        type1 = this.Value.DeclaringType;
        Type type2 = (Type) null;
        if (this.Value.DeclaringType.IsGenericType() && (ClrModule.IsMono || this.Value.DeclaringType.FullName == null) && this.Value.DeclaringType.ContainsGenericParameters() && !this.Value.DeclaringType.IsGenericTypeDefinition())
        {
          Type[] genericArguments = this.Value.DeclaringType.GetGenericArguments();
          bool flag = genericArguments.Length != 0;
          foreach (Type type3 in genericArguments)
          {
            if (!type3.IsGenericParameter)
            {
              flag = false;
              break;
            }
          }
          if (flag)
            type2 = this.Value.DeclaringType.GetGenericTypeDefinition();
        }
        else if (this.Value.DeclaringType.IsGenericTypeDefinition())
          type2 = this.Value.DeclaringType;
        if (type2 != (Type) null)
        {
          foreach (Type type4 in CompilerHelpers.GetType(this.Value.BindingSelf).GetInterfaces())
          {
            if (type4.IsGenericType() && type4.GetGenericTypeDefinition() == type2)
            {
              type1 = type4;
              break;
            }
          }
        }
      }
      if (this.Value.DeclaringType.IsInterface() && type1.IsValueType())
        instance = Microsoft.Scripting.Ast.Utils.Convert(instance, this.Value.DeclaringType);
      else if (type1.IsValueType())
      {
        instance = (Expression) Expression.Unbox(instance, type1);
      }
      else
      {
        Type type5 = type1 == typeof (MarshalByRefObject) ? CompilerHelpers.GetVisibleType(this.Value.DeclaringType) : type1;
        instance = Microsoft.Scripting.Ast.Utils.Convert(instance, type5);
      }
    }
    else
      instance = Microsoft.Scripting.Ast.Utils.Convert(instance, typeof (Enum));
    return new DynamicMetaObject(instance, runtimeTypeRestriction, bindingSelf);
  }

  private MemberExpression ReadStrongBoxValue(Expression instance)
  {
    return Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(instance, this.Value.BindingSelf.GetType()), this.Value.BindingSelf.GetType().GetField("Value"));
  }

  internal static CallSignature GetReversedSignature(CallSignature signature)
  {
    return new CallSignature(ArrayUtils.Append<Argument>(signature.GetArgumentInfos(), new Argument(ArgumentType.Simple)));
  }

  public BuiltinFunction Value => (BuiltinFunction) base.Value;
}
