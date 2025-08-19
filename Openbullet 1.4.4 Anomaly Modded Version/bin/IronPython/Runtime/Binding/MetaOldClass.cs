// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaOldClass
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaOldClass(
  Expression expression,
  BindingRestrictions restrictions,
  OldClass value) : 
  MetaPythonObject(expression, BindingRestrictions.Empty, (object) value),
  IPythonInvokable,
  IPythonGetable,
  IPythonOperable,
  IPythonConvertible
{
  public DynamicMetaObject Invoke(
    PythonInvokeBinder pythonInvoke,
    Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return this.MakeCallRule((DynamicMetaObjectBinder) pythonInvoke, codeContext, args);
  }

  public DynamicMetaObject GetMember(PythonGetMemberBinder member, DynamicMetaObject codeContext)
  {
    return this.MakeGetMember((DynamicMetaObjectBinder) member, codeContext);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder action,
    DynamicMetaObject[] args)
  {
    return BindingHelpers.GenericInvokeMember(action, (ValidationInfo) null, (DynamicMetaObject) this, args);
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder call, DynamicMetaObject[] args)
  {
    return this.MakeCallRule((DynamicMetaObjectBinder) call, Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) call).SharedContext), args);
  }

  public override DynamicMetaObject BindCreateInstance(
    CreateInstanceBinder create,
    DynamicMetaObject[] args)
  {
    return this.MakeCallRule((DynamicMetaObjectBinder) create, Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) create).SharedContext), args);
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder member)
  {
    return this.MakeGetMember((DynamicMetaObjectBinder) member, PythonContext.GetCodeContextMO((DynamicMetaObjectBinder) member));
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder member, DynamicMetaObject value)
  {
    return this.MakeSetMember(member.Name, value);
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder member)
  {
    return this.MakeDeleteMember(member);
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
    return toType.IsSubclassOf(typeof (Delegate)) ? MetaPythonObject.MakeDelegateTarget(binder, toType, this.Restrict(typeof (OldClass))) : this.FallbackConvert(binder);
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    foreach (object memberName in (IEnumerable<object>) ((IPythonMembersList) this.Value).GetMemberNames(DefaultContext.Default))
    {
      if (memberName is string)
        yield return (string) memberName;
    }
  }

  private DynamicMetaObject MakeCallRule(
    DynamicMetaObjectBinder call,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    CallSignature callSignature = BindingHelpers.GetCallSignature(call);
    Expression[] array = new Expression[args.Length];
    for (int index = 0; index < args.Length; ++index)
      array[index] = args[index].Expression;
    ParameterExpression left1 = Expression.Variable(typeof (object), "init");
    ParameterExpression left2 = Expression.Variable(typeof (object), "inst");
    DynamicMetaObject self = this.Restrict(typeof (OldClass));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      left1,
      left2
    }, (Expression) Expression.Assign((Expression) left2, (Expression) Expression.New(typeof (OldInstance).GetConstructor(new Type[2]
    {
      typeof (CodeContext),
      typeof (OldClass)
    }), codeContext, self.Expression)), (Expression) Expression.Condition((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) Expression.Assign((Expression) left1, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassTryLookupInit"), self.Expression, (Expression) left2)), typeof (OperationFailed))), (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(call).Invoke(callSignature), typeof (object), ArrayUtils.Insert<Expression>(codeContext, (Expression) left1, array)), MetaOldClass.NoInitCheckNoArgs(callSignature, self, args)), (Expression) left2), self.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
  }

  private static Expression NoInitCheckNoArgs(
    CallSignature signature,
    DynamicMetaObject self,
    DynamicMetaObject[] args)
  {
    int length = args.Length;
    Expression argumentExpression1 = MetaOldClass.GetArgumentExpression(signature, ArgumentType.Dictionary, ref length, args);
    Expression argumentExpression2 = MetaOldClass.GetArgumentExpression(signature, ArgumentType.List, ref length, args);
    if (!signature.IsSimple && length <= 0)
      return (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassCheckCallError"), self.Expression, argumentExpression1, argumentExpression2);
    return args.Length != 0 ? (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassMakeCallError"), self.Expression) : Microsoft.Scripting.Ast.Utils.Constant((object) null);
  }

  private static Expression GetArgumentExpression(
    CallSignature signature,
    ArgumentType kind,
    ref int unusedCount,
    DynamicMetaObject[] args)
  {
    int index = signature.IndexOf(kind);
    if (index == -1)
      return Microsoft.Scripting.Ast.Utils.Constant((object) null);
    --unusedCount;
    return args[index].Expression;
  }

  public static object MakeCallError()
  {
    throw PythonOps.TypeError("this constructor takes no arguments");
  }

  private DynamicMetaObject MakeSetMember(string name, DynamicMetaObject value)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldClass));
    Expression expression1 = Microsoft.Scripting.Ast.Utils.Convert(value.Expression, typeof (object));
    Expression expression2;
    switch (name)
    {
      case "__bases__":
        expression2 = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassSetBases"), dynamicMetaObject.Expression, expression1);
        break;
      case "__name__":
        expression2 = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassSetName"), dynamicMetaObject.Expression, expression1);
        break;
      case "__dict__":
        expression2 = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassSetDictionary"), dynamicMetaObject.Expression, expression1);
        break;
      default:
        expression2 = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassSetNameHelper"), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name), expression1);
        break;
    }
    return new DynamicMetaObject(expression2, dynamicMetaObject.Restrictions.Merge(value.Restrictions));
  }

  private DynamicMetaObject MakeDeleteMember(DeleteMemberBinder member)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldClass));
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassDeleteMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) member.Name)), dynamicMetaObject.Restrictions);
  }

  private DynamicMetaObject MakeGetMember(
    DynamicMetaObjectBinder member,
    DynamicMetaObject codeContext)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldClass));
    string getMemberName = MetaPythonObject.GetGetMemberName(member);
    Expression expression;
    switch (getMemberName)
    {
      case "__dict__":
        expression = (Expression) Expression.Block((Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassDictionaryIsPublic"), dynamicMetaObject.Expression), (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassGetDictionary"), dynamicMetaObject.Expression));
        break;
      case "__bases__":
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassGetBaseClasses"), dynamicMetaObject.Expression);
        break;
      case "__name__":
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassGetName"), dynamicMetaObject.Expression);
        break;
      default:
        ParameterExpression parameterExpression = Expression.Variable(typeof (object), "lookupVal");
        return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          parameterExpression
        }, (Expression) Expression.Condition((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldClassTryLookupValue"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) getMemberName))), typeof (OperationFailed))), (Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(MetaPythonObject.GetMemberFallback((DynamicMetaObject) this, member, codeContext).Expression, typeof (object)))), dynamicMetaObject.Restrictions);
    }
    return new DynamicMetaObject(expression, dynamicMetaObject.Restrictions);
  }

  public OldClass Value => (OldClass) base.Value;

  DynamicMetaObject IPythonOperable.BindOperation(
    PythonOperationBinder action,
    DynamicMetaObject[] args)
  {
    return action.Operation == PythonOperationKind.IsCallable ? new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) true), this.Restrictions.Merge(BindingRestrictions.GetTypeRestriction(this.Expression, typeof (OldClass)))) : (DynamicMetaObject) null;
  }
}
