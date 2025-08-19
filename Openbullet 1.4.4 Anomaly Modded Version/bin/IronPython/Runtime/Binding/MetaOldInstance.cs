// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaOldInstance
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaOldInstance(
  Expression expression,
  BindingRestrictions restrictions,
  OldInstance value) : 
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
    return this.InvokeWorker((DynamicMetaObjectBinder) pythonInvoke, codeContext, args);
  }

  public DynamicMetaObject GetMember(PythonGetMemberBinder member, DynamicMetaObject codeContext)
  {
    return this.MakeMemberAccess((DynamicMetaObjectBinder) member, member.Name, MetaOldInstance.MemberAccess.Get, (DynamicMetaObject) this);
  }

  public override DynamicMetaObject BindInvokeMember(
    InvokeMemberBinder action,
    DynamicMetaObject[] args)
  {
    return this.MakeMemberAccess((DynamicMetaObjectBinder) action, action.Name, MetaOldInstance.MemberAccess.Invoke, args);
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder member)
  {
    return this.MakeMemberAccess((DynamicMetaObjectBinder) member, member.Name, MetaOldInstance.MemberAccess.Get, (DynamicMetaObject) this);
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder member, DynamicMetaObject value)
  {
    return this.MakeMemberAccess((DynamicMetaObjectBinder) member, member.Name, MetaOldInstance.MemberAccess.Set, (DynamicMetaObject) this, value);
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder member)
  {
    return this.MakeMemberAccess((DynamicMetaObjectBinder) member, member.Name, MetaOldInstance.MemberAccess.Delete, (DynamicMetaObject) this);
  }

  public override DynamicMetaObject BindBinaryOperation(
    BinaryOperationBinder binder,
    DynamicMetaObject arg)
  {
    return PythonProtocol.Operation(binder, (DynamicMetaObject) this, arg, (DynamicMetaObject) null);
  }

  public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
  {
    return PythonProtocol.Operation(binder, (DynamicMetaObject) this, (DynamicMetaObject) null);
  }

  public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
  {
    return PythonProtocol.Index((DynamicMetaObjectBinder) binder, PythonIndexType.GetItem, ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, indexes));
  }

  public override DynamicMetaObject BindSetIndex(
    SetIndexBinder binder,
    DynamicMetaObject[] indexes,
    DynamicMetaObject value)
  {
    return PythonProtocol.Index((DynamicMetaObjectBinder) binder, PythonIndexType.SetItem, ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, ArrayUtils.Append<DynamicMetaObject>(indexes, value)));
  }

  public override DynamicMetaObject BindDeleteIndex(
    DeleteIndexBinder binder,
    DynamicMetaObject[] indexes)
  {
    return PythonProtocol.Index((DynamicMetaObjectBinder) binder, PythonIndexType.DeleteItem, ArrayUtils.Insert<DynamicMetaObject>((DynamicMetaObject) this, indexes));
  }

  public override DynamicMetaObject BindConvert(ConvertBinder conversion)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) conversion, conversion.Type, conversion.Type, conversion.Explicit ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast);
  }

  public DynamicMetaObject BindConvert(PythonConversionBinder binder)
  {
    return this.ConvertWorker((DynamicMetaObjectBinder) binder, binder.Type, binder.ReturnType, binder.ResultKind);
  }

  public DynamicMetaObject ConvertWorker(
    DynamicMetaObjectBinder binder,
    Type type,
    Type retType,
    ConversionResultKind kind)
  {
    if (!type.IsEnum())
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Object:
          if (type == typeof (BigInteger))
            return this.MakeConvertToCommon(binder, type, retType, "__long__");
          if (type == typeof (Complex))
            return this.MakeConvertToCommon(binder, type, retType, "__complex__");
          if (type == typeof (IEnumerable))
            return this.MakeConvertToIEnumerable(binder);
          if (type == typeof (IEnumerator))
            return this.MakeConvertToIEnumerator(binder);
          if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (IEnumerable<>))
            return this.MakeConvertToIEnumerable(binder, type, type.GetGenericArguments()[0]);
          if (type.IsSubclassOf(typeof (Delegate)))
            return MetaPythonObject.MakeDelegateTarget(binder, type, this.Restrict(typeof (OldInstance)));
          break;
        case TypeCode.Boolean:
          return this.MakeConvertToBool(binder);
        case TypeCode.Int32:
          return this.MakeConvertToCommon(binder, type, retType, "__int__");
        case TypeCode.Double:
          return this.MakeConvertToCommon(binder, type, retType, "__float__");
        case TypeCode.String:
          return this.MakeConvertToCommon(binder, type, retType, "__str__");
      }
    }
    return this.FallbackConvert(binder);
  }

  public override DynamicMetaObject BindInvoke(InvokeBinder invoke, DynamicMetaObject[] args)
  {
    return this.InvokeWorker((DynamicMetaObjectBinder) invoke, PythonContext.GetCodeContext((DynamicMetaObjectBinder) invoke), args);
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    foreach (object memberName in (IEnumerable<object>) ((IPythonMembersList) this.Value).GetMemberNames(DefaultContext.Default))
    {
      if (memberName is string)
        yield return (string) memberName;
    }
  }

  private DynamicMetaObject InvokeWorker(
    DynamicMetaObjectBinder invoke,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldInstance));
    Expression[] array = new Expression[args.Length + 1];
    for (int index = 0; index < args.Length; ++index)
      array[index + 1] = args[index].Expression;
    ParameterExpression left = Expression.Variable(typeof (object), "callFunc");
    array[0] = (Expression) left;
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Condition((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) Expression.Assign((Expression) left, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceTryGetBoundCustomMember"), codeContext, dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) "__call__"))), typeof (OperationFailed))), (Expression) Expression.Block((Expression) Microsoft.Scripting.Ast.Utils.Try((Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionPushFrameCodeContext"), codeContext), (Expression) Expression.Assign((Expression) left, (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(invoke).Invoke(BindingHelpers.GetCallSignature(invoke)), typeof (object), ArrayUtils.Insert<Expression>(codeContext, array)))).Finally((Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionPopFrame"))), (Expression) left), Microsoft.Scripting.Ast.Utils.Convert(BindingHelpers.InvokeFallback(invoke, codeContext, (DynamicMetaObject) this, args).Expression, typeof (object)))), dynamicMetaObject.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
  }

  private DynamicMetaObject MakeConvertToIEnumerable(DynamicMetaObjectBinder conversion)
  {
    ParameterExpression parameterExpression = Expression.Variable(typeof (IEnumerable), "res");
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldInstance));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceConvertToIEnumerableNonThrowing"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(conversion).SharedContext), dynamicMetaObject.Expression)), Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(Microsoft.Scripting.Ast.Utils.Convert(this.FallbackConvert(conversion).Expression, typeof (object)), typeof (IEnumerable)))), dynamicMetaObject.Restrictions);
  }

  private DynamicMetaObject MakeConvertToIEnumerator(DynamicMetaObjectBinder conversion)
  {
    ParameterExpression parameterExpression = Expression.Variable(typeof (IEnumerator), "res");
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldInstance));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceConvertToIEnumeratorNonThrowing"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(conversion).SharedContext), dynamicMetaObject.Expression)), Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(Microsoft.Scripting.Ast.Utils.Convert(this.FallbackConvert(conversion).Expression, typeof (object)), typeof (IEnumerator)))), dynamicMetaObject.Restrictions);
  }

  private DynamicMetaObject MakeConvertToIEnumerable(
    DynamicMetaObjectBinder conversion,
    Type toType,
    Type genericType)
  {
    ParameterExpression parameterExpression = Expression.Variable(toType, "res");
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldInstance));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceConvertToIEnumerableOfTNonThrowing").MakeGenericMethod(genericType), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(conversion).SharedContext), dynamicMetaObject.Expression)), Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(Microsoft.Scripting.Ast.Utils.Convert(this.FallbackConvert(conversion).Expression, typeof (object)), toType))), dynamicMetaObject.Restrictions);
  }

  private DynamicMetaObject MakeConvertToCommon(
    DynamicMetaObjectBinder conversion,
    Type toType,
    Type retType,
    string name)
  {
    ParameterExpression tmp = Expression.Variable(typeof (object), "convertResult");
    DynamicMetaObject self = this.Restrict(typeof (OldInstance));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      tmp
    }, (Expression) Expression.Condition((Expression) MetaOldInstance.MakeOneConvert(conversion, self, name, tmp), (Expression) Expression.Convert((Expression) tmp, retType), this.FallbackConvert(conversion).Expression)), self.Restrictions);
  }

  private static BinaryExpression MakeOneConvert(
    DynamicMetaObjectBinder conversion,
    DynamicMetaObject self,
    string name,
    ParameterExpression tmp)
  {
    return Expression.NotEqual((Expression) Expression.Assign((Expression) tmp, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceConvertNonThrowing"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(conversion).SharedContext), self.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name))), Microsoft.Scripting.Ast.Utils.Constant((object) null));
  }

  private DynamicMetaObject MakeConvertToBool(DynamicMetaObjectBinder conversion)
  {
    DynamicMetaObject dynamicMetaObject1 = this.Restrict(typeof (OldInstance));
    ParameterExpression left = Expression.Variable(typeof (bool?), "tmp");
    DynamicMetaObject dynamicMetaObject2 = this.FallbackConvert(conversion);
    Type compatibleType = BindingHelpers.GetCompatibleType(typeof (bool), dynamicMetaObject2.Expression.Type);
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) left, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceConvertToBoolNonThrowing"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(conversion).SharedContext), dynamicMetaObject1.Expression)), Microsoft.Scripting.Ast.Utils.Constant((object) null)), Microsoft.Scripting.Ast.Utils.Convert((Expression) left, compatibleType), Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject2.Expression, compatibleType))), dynamicMetaObject1.Restrictions);
  }

  private DynamicMetaObject MakeMemberAccess(
    DynamicMetaObjectBinder member,
    string name,
    MetaOldInstance.MemberAccess access,
    params DynamicMetaObject[] args)
  {
    DynamicMetaObject target = this.Restrict(typeof (OldInstance));
    CustomInstanceDictionaryStorage dict;
    int customStorageSlot = this.GetCustomStorageSlot(name, out dict);
    if (customStorageSlot == -1)
      return this.MakeDynamicMemberAccess(member, name, access, args);
    ParameterExpression left = Expression.Variable(typeof (object), "dict");
    ValidationInfo typeTest = new ValidationInfo((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) left, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceGetOptimizedDictionary"), target.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) dict.KeyVersion))), Microsoft.Scripting.Ast.Utils.Constant((object) null)));
    Expression expression;
    switch (access)
    {
      case MetaOldInstance.MemberAccess.Get:
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceDictionaryGetValueHelper"), (Expression) left, Microsoft.Scripting.Ast.Utils.Constant((object) customStorageSlot), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (object)));
        break;
      case MetaOldInstance.MemberAccess.Set:
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceDictionarySetExtraValue"), (Expression) left, Microsoft.Scripting.Ast.Utils.Constant((object) customStorageSlot), Microsoft.Scripting.Ast.Utils.Convert(args[1].Expression, typeof (object)));
        break;
      case MetaOldInstance.MemberAccess.Delete:
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceDeleteCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(member).SharedContext), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (OldInstance)), Microsoft.Scripting.Ast.Utils.Constant((object) name));
        break;
      case MetaOldInstance.MemberAccess.Invoke:
        ParameterExpression parameterExpression = Expression.Variable(typeof (object), "value");
        expression = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          parameterExpression
        }, (Expression) Expression.Condition((Expression) Expression.Call(typeof (PythonOps).GetMethod("TryOldInstanceDictionaryGetValueHelper"), (Expression) left, (Expression) Expression.Constant((object) customStorageSlot), Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (object)), (Expression) parameterExpression), Microsoft.Scripting.Ast.Utils.Convert(((InvokeMemberBinder) member).FallbackInvoke(new DynamicMetaObject((Expression) parameterExpression, BindingRestrictions.Empty), args, (DynamicMetaObject) null).Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(((InvokeMemberBinder) member).FallbackInvokeMember(target, args).Expression, typeof (object))));
        break;
      default:
        throw new InvalidOperationException();
    }
    return BindingHelpers.AddDynamicTestAndDefer(member, new DynamicMetaObject(expression, BindingRestrictions.Combine((IList<DynamicMetaObject>) args).Merge(target.Restrictions)), args, typeTest, left);
  }

  private int GetCustomStorageSlot(string name, out CustomInstanceDictionaryStorage dict)
  {
    dict = this.Value.Dictionary._storage as CustomInstanceDictionaryStorage;
    return dict == null || this.Value._class.HasSetAttr ? -1 : dict.FindKey(name);
  }

  private DynamicMetaObject MakeDynamicMemberAccess(
    DynamicMetaObjectBinder member,
    string name,
    MetaOldInstance.MemberAccess access,
    DynamicMetaObject[] args)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldInstance));
    ParameterExpression parameterExpression = Expression.Variable(typeof (object), "result");
    Expression expression;
    switch (access)
    {
      case MetaOldInstance.MemberAccess.Get:
        expression = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          parameterExpression
        }, (Expression) Expression.Condition((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceTryGetBoundCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name))), typeof (OperationFailed))), (Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(this.FallbackGet(member, args), typeof (object))));
        break;
      case MetaOldInstance.MemberAccess.Set:
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceSetCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name), Microsoft.Scripting.Ast.Utils.Convert(args[1].Expression, typeof (object)));
        break;
      case MetaOldInstance.MemberAccess.Delete:
        expression = (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceDeleteCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name));
        break;
      case MetaOldInstance.MemberAccess.Invoke:
        expression = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          parameterExpression
        }, (Expression) Expression.Condition((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceTryGetBoundCustomMember"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(member).SharedContext), dynamicMetaObject.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) name))), typeof (OperationFailed))), ((InvokeMemberBinder) member).FallbackInvoke(new DynamicMetaObject((Expression) parameterExpression, BindingRestrictions.Empty), args, (DynamicMetaObject) null).Expression, Microsoft.Scripting.Ast.Utils.Convert(((InvokeMemberBinder) member).FallbackInvokeMember((DynamicMetaObject) this, args).Expression, typeof (object))));
        break;
      default:
        throw new InvalidOperationException();
    }
    return new DynamicMetaObject(expression, dynamicMetaObject.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
  }

  private Expression FallbackGet(DynamicMetaObjectBinder member, DynamicMetaObject[] args)
  {
    if (member is GetMemberBinder getMemberBinder)
      return getMemberBinder.FallbackGetMember(args[0]).Expression;
    PythonGetMemberBinder pythonGetMemberBinder = member as PythonGetMemberBinder;
    if (pythonGetMemberBinder.IsNoThrow)
      return (Expression) Expression.Field((Expression) null, typeof (OperationFailed).GetDeclaredField("Value"));
    return member.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("AttributeError"), Microsoft.Scripting.Ast.Utils.Constant((object) "{0} instance has no attribute '{1}'"), (Expression) Expression.NewArrayInit(typeof (object), Microsoft.Scripting.Ast.Utils.Constant(this.Value._class._name), Microsoft.Scripting.Ast.Utils.Constant((object) pythonGetMemberBinder.Name))));
  }

  public OldInstance Value => (OldInstance) base.Value;

  DynamicMetaObject IPythonOperable.BindOperation(
    PythonOperationBinder action,
    DynamicMetaObject[] args)
  {
    return action.Operation == PythonOperationKind.IsCallable ? this.MakeIsCallable(action) : (DynamicMetaObject) null;
  }

  private DynamicMetaObject MakeIsCallable(PythonOperationBinder operation)
  {
    DynamicMetaObject dynamicMetaObject = this.Restrict(typeof (OldInstance));
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("OldInstanceIsCallable"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) operation).SharedContext), dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
  }

  private enum MemberAccess
  {
    Get,
    Set,
    Delete,
    Invoke,
  }
}
