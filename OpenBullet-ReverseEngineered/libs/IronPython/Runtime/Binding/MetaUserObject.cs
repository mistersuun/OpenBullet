// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.MetaUserObject
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class MetaUserObject : 
  MetaPythonObject,
  IPythonInvokable,
  IPythonConvertible,
  IPythonOperable,
  IPythonGetable
{
  private readonly DynamicMetaObject _baseMetaObject;

  public MetaUserObject(
    Expression expression,
    BindingRestrictions restrictions,
    DynamicMetaObject baseMetaObject,
    IPythonObject value)
    : base(expression, restrictions, (object) value)
  {
    this._baseMetaObject = baseMetaObject;
  }

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
    return new MetaUserObject.InvokeBinderHelper(this, action, args, PythonContext.GetCodeContextMO((DynamicMetaObjectBinder) action)).Bind(PythonContext.GetPythonContext((DynamicMetaObjectBinder) action).SharedContext, action.Name);
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
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo((DynamicMetaObject) this, this.Value.PythonType);
    return BindingHelpers.AddDynamicTestAndDefer(binder, this.TryPythonConversion(binder, type) ?? this.FallbackConvert(binder), new DynamicMetaObject[1]
    {
      (DynamicMetaObject) this
    }, validationInfo, retType);
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

  public override DynamicMetaObject BindInvoke(InvokeBinder action, DynamicMetaObject[] args)
  {
    Expression codeContext = (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetPythonTypeContext"), (Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(this.Expression, typeof (IPythonObject)), "PythonType"));
    return this.InvokeWorker((DynamicMetaObjectBinder) action, codeContext, args);
  }

  public override IEnumerable<string> GetDynamicMemberNames()
  {
    foreach (object memberName in this.Value.PythonType.GetMemberNames(this.Value.PythonType.PythonContext.SharedContext, (object) this.Value))
    {
      if (memberName is string)
        yield return (string) memberName;
    }
  }

  private DynamicMetaObject InvokeWorker(
    DynamicMetaObjectBinder action,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo((DynamicMetaObject) this, this.Value.PythonType);
    if (this.Value is PythonType)
    {
      PythonContext pythonContext = PythonContext.GetPythonContext(action);
      PythonTypeSlot slot1;
      PythonTypeSlot slot2;
      if (this.Value.PythonType.TryResolveMixedSlot(pythonContext.SharedContext, "__call__", out slot1) && TypeCache.PythonType.TryResolveSlot(pythonContext.SharedContext, "__call__", out slot2) && slot1 == slot2)
        return this.InvokeFallback(action, codeContext, args);
    }
    return BindingHelpers.AddDynamicTestAndDefer(action, PythonProtocol.Call(action, (DynamicMetaObject) this, args) ?? this.InvokeFallback(action, codeContext, args), args, validationInfo);
  }

  private DynamicMetaObject InvokeFallback(
    DynamicMetaObjectBinder action,
    Expression codeContext,
    DynamicMetaObject[] args)
  {
    switch (action)
    {
      case InvokeBinder binder:
        return this._baseMetaObject != null ? this._baseMetaObject.BindInvoke(binder, args) : binder.FallbackInvoke(this.Restrict(this.GetLimitType()), args);
      case PythonInvokeBinder pythonInvoke:
        if (this._baseMetaObject is IPythonInvokable baseMetaObject)
          return baseMetaObject.Invoke(pythonInvoke, codeContext, (DynamicMetaObject) this, args);
        return this._baseMetaObject != null ? pythonInvoke.InvokeForeignObject((DynamicMetaObject) this, args) : pythonInvoke.Fallback(codeContext, (DynamicMetaObject) this, args);
      default:
        throw new InvalidOperationException();
    }
  }

  private DynamicMetaObject TryPythonConversion(DynamicMetaObjectBinder conversion, Type type)
  {
    if (!type.IsEnum())
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Object:
          if (type == typeof (Complex))
            return this.MakeConvertRuleForCall(conversion, type, (DynamicMetaObject) this, "__complex__", "ConvertToComplex", (Func<DynamicMetaObject>) (() => this.MakeConvertRuleForCall(conversion, type, (DynamicMetaObject) this, "__float__", "ConvertToFloat", (Func<DynamicMetaObject>) (() => this.FallbackConvert(conversion)), (Func<Expression, Expression>) (x => (Expression) Expression.Call((Expression) null, typeof (PythonOps).GetMethod("ConvertFloatToComplex"), x)))), (Func<Expression, Expression>) (x => x));
          if (type == typeof (BigInteger))
            return this.MakeConvertRuleForCall(conversion, type, (DynamicMetaObject) this, "__long__", "ConvertToLong");
          if (type == typeof (IEnumerable))
            return PythonConversionBinder.ConvertToIEnumerable(conversion, this.Restrict(this.Value.GetType()));
          if (type == typeof (IEnumerator))
            return PythonConversionBinder.ConvertToIEnumerator(conversion, this.Restrict(this.Value.GetType()));
          if (type.IsSubclassOf(typeof (Delegate)))
            return MetaPythonObject.MakeDelegateTarget(conversion, type, this.Restrict(this.Value.GetType()));
          break;
        case TypeCode.Boolean:
          return PythonProtocol.ConvertToBool(conversion, (DynamicMetaObject) this);
        case TypeCode.Int32:
          return this.MakeConvertRuleForCall(conversion, type, (DynamicMetaObject) this, "__int__", "ConvertToInt");
        case TypeCode.Double:
          return this.MakeConvertRuleForCall(conversion, type, (DynamicMetaObject) this, "__float__", "ConvertToFloat");
        case TypeCode.String:
          if (!typeof (Extensible<string>).IsAssignableFrom(this.LimitType))
            return this.MakeConvertRuleForCall(conversion, type, (DynamicMetaObject) this, "__str__", "ConvertToString");
          break;
      }
    }
    return (DynamicMetaObject) null;
  }

  private DynamicMetaObject MakeConvertRuleForCall(
    DynamicMetaObjectBinder convertToAction,
    Type toType,
    DynamicMetaObject self,
    string name,
    string returner,
    Func<DynamicMetaObject> fallback,
    Func<Expression, Expression> resultConverter)
  {
    PythonType pythonType = ((IPythonObject) self.Value).PythonType;
    CodeContext sharedContext = PythonContext.GetPythonContext(convertToAction).SharedContext;
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo((DynamicMetaObject) this, pythonType);
    PythonTypeSlot slot;
    if (!pythonType.TryResolveSlot(sharedContext, name, out slot) || MetaUserObject.IsBuiltinConversion(sharedContext, slot, name, pythonType))
      return fallback();
    ParameterExpression tmp = Expression.Variable(typeof (object), "func");
    Expression expression = resultConverter((Expression) Expression.Call(PythonOps.GetConversionHelper(returner, MetaUserObject.GetResultKind(convertToAction)), (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(convertToAction).InvokeNone, typeof (object), PythonContext.GetCodeContext(convertToAction), (Expression) tmp)));
    if (typeof (Extensible<>).MakeGenericType(toType).IsAssignableFrom(self.GetLimitType()))
      expression = Microsoft.Scripting.Ast.Utils.Convert(MetaUserObject.AddExtensibleSelfCheck(convertToAction, toType, self, expression), typeof (object));
    return BindingHelpers.AddDynamicTestAndDefer(convertToAction, new DynamicMetaObject((Expression) Expression.Condition((Expression) MetaPythonObject.MakeTryGetTypeMember(PythonContext.GetPythonContext(convertToAction), slot, self.Expression, tmp), expression, Microsoft.Scripting.Ast.Utils.Convert(this.ConversionFallback(convertToAction), typeof (object))), self.Restrict(self.GetRuntimeType()).Restrictions), new DynamicMetaObject[1]
    {
      (DynamicMetaObject) this
    }, validationInfo, tmp);
  }

  private DynamicMetaObject MakeConvertRuleForCall(
    DynamicMetaObjectBinder convertToAction,
    Type toType,
    DynamicMetaObject self,
    string name,
    string returner)
  {
    return this.MakeConvertRuleForCall(convertToAction, toType, self, name, returner, (Func<DynamicMetaObject>) (() => this.FallbackConvert(convertToAction)), (Func<Expression, Expression>) (x => x));
  }

  private static Expression AddExtensibleSelfCheck(
    DynamicMetaObjectBinder convertToAction,
    Type toType,
    DynamicMetaObject self,
    Expression callExpr)
  {
    ParameterExpression left = Expression.Variable(callExpr.Type, "tmp");
    Type type1;
    switch (MetaUserObject.GetResultKind(convertToAction))
    {
      case ConversionResultKind.ImplicitTry:
      case ConversionResultKind.ExplicitTry:
        type1 = typeof (object);
        break;
      default:
        type1 = toType;
        break;
    }
    Type type2 = type1;
    callExpr = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Block((Expression) Expression.Assign((Expression) left, callExpr), (Expression) Expression.Condition((Expression) Expression.Equal((Expression) left, self.Expression), Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(self.Expression, self.GetLimitType()), self.GetLimitType().GetProperty("Value")), type2), (Expression) Expression.Dynamic((CallSiteBinder) new PythonConversionBinder(PythonContext.GetPythonContext(convertToAction), toType, MetaUserObject.GetResultKind(convertToAction)), type2, (Expression) left))));
    return callExpr;
  }

  private static ConversionResultKind GetResultKind(DynamicMetaObjectBinder convertToAction)
  {
    if (convertToAction is PythonConversionBinder conversionBinder)
      return conversionBinder.ResultKind;
    return ((ConvertBinder) convertToAction).Explicit ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast;
  }

  private Expression ConversionFallback(DynamicMetaObjectBinder convertToAction)
  {
    return convertToAction is PythonConversionBinder convertToAction1 ? MetaUserObject.GetConversionFailedReturnValue(convertToAction1, (DynamicMetaObject) this).Expression : convertToAction.GetUpdateExpression(typeof (object));
  }

  private static bool IsBuiltinConversion(
    CodeContext context,
    PythonTypeSlot pts,
    string name,
    PythonType selfType)
  {
    Type type1 = selfType.UnderlyingSystemType.BaseType;
    Type type2 = type1;
    while (!type2.IsGenericType || !(type2.GetGenericTypeDefinition() == typeof (Extensible<>)))
    {
      type2 = type2.BaseType;
      if (!(type2 != (Type) null))
        goto label_4;
    }
    type1 = type2.GetGenericArguments()[0];
label_4:
    PythonTypeSlot slot;
    return DynamicHelpers.GetPythonTypeFromType(type1).TryResolveSlot(context, name, out slot) && pts == slot;
  }

  private static DynamicMetaObject GetConversionFailedReturnValue(
    PythonConversionBinder convertToAction,
    DynamicMetaObject self)
  {
    switch (convertToAction.ResultKind)
    {
      case ConversionResultKind.ImplicitCast:
      case ConversionResultKind.ExplicitCast:
        return Microsoft.Scripting.Actions.DefaultBinder.MakeError(PythonContext.GetPythonContext((DynamicMetaObjectBinder) convertToAction).Binder.MakeConversionError(convertToAction.Type, self.Expression), typeof (object));
      case ConversionResultKind.ImplicitTry:
      case ConversionResultKind.ExplicitTry:
        return new DynamicMetaObject(Microsoft.Scripting.Actions.DefaultBinder.GetTryConvertReturnValue(convertToAction.Type), BindingRestrictions.Empty);
      default:
        throw new InvalidOperationException(convertToAction.ResultKind.ToString());
    }
  }

  private DynamicMetaObject Fallback(DynamicMetaObjectBinder action, DynamicMetaObject codeContext)
  {
    if (this._baseMetaObject == null)
      return MetaPythonObject.GetMemberFallback((DynamicMetaObject) this, action, codeContext);
    if (this._baseMetaObject is IPythonGetable baseMetaObject && action is PythonGetMemberBinder member)
      return baseMetaObject.GetMember(member, codeContext);
    return action is GetMemberBinder binder ? this._baseMetaObject.BindGetMember(binder) : this._baseMetaObject.BindGetMember((GetMemberBinder) PythonContext.GetPythonContext(action).CompatGetMember(MetaPythonObject.GetGetMemberName(action), false));
  }

  private DynamicMetaObject Fallback(SetMemberBinder action, DynamicMetaObject value)
  {
    return this._baseMetaObject != null ? this._baseMetaObject.BindSetMember(action, value) : action.FallbackSetMember((DynamicMetaObject) this, value);
  }

  public IPythonObject Value => (IPythonObject) base.Value;

  DynamicMetaObject IPythonOperable.BindOperation(
    PythonOperationBinder action,
    DynamicMetaObject[] args)
  {
    if (action.Operation != PythonOperationKind.IsCallable)
      return (DynamicMetaObject) null;
    DynamicMetaObject dynamicMetaObject = this.Restrict(this.Value.GetType());
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("UserObjectIsCallable"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) action).SharedContext), dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
  }

  public DynamicMetaObject GetMember(PythonGetMemberBinder member, DynamicMetaObject codeContext)
  {
    return this.GetMemberWorker((DynamicMetaObjectBinder) member, codeContext);
  }

  public override DynamicMetaObject BindGetMember(GetMemberBinder action)
  {
    return this.GetMemberWorker((DynamicMetaObjectBinder) action, PythonContext.GetCodeContextMO((DynamicMetaObjectBinder) action));
  }

  public override DynamicMetaObject BindSetMember(SetMemberBinder action, DynamicMetaObject value)
  {
    return new MetaUserObject.MetaSetBinderHelper(this, value, action).Bind(action.Name);
  }

  public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder action)
  {
    return this.MakeDeleteMemberRule(new MetaUserObject.DeleteBindingInfo(action, new DynamicMetaObject[1]
    {
      (DynamicMetaObject) this
    }, new ConditionalBuilder((DynamicMetaObjectBinder) action), BindingHelpers.GetValidationInfo((DynamicMetaObject) this, this.PythonType)));
  }

  private DynamicMetaObject GetMemberWorker(
    DynamicMetaObjectBinder member,
    DynamicMetaObject codeContext)
  {
    return new MetaUserObject.GetBinderHelper(this, member, codeContext).Bind((CodeContext) codeContext.Value, MetaPythonObject.GetGetMemberName(member));
  }

  private static bool TryGetGetAttribute(
    CodeContext context,
    PythonType type,
    out PythonTypeSlot dts)
  {
    return type.TryResolveSlot(context, "__getattribute__", out dts) && (!(dts is BuiltinMethodDescriptor methodDescriptor) || methodDescriptor.DeclaringType != typeof (object) || methodDescriptor.Template.Targets.Count != 1 || methodDescriptor.Template.Targets[0].DeclaringType != typeof (ObjectOps) || methodDescriptor.Template.Targets[0].Name != "__getattribute__") && dts != null;
  }

  private static MethodCallExpression MakeGetAttrTestAndGet(
    MetaUserObject.GetBindingInfo info,
    Expression getattr)
  {
    return Expression.Call(PythonTypeInfo._PythonOps.SlotTryGetBoundValue, Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext(info.Action).SharedContext), Microsoft.Scripting.Ast.Utils.Convert(getattr, typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert((Expression) info.Self, typeof (object)), (Expression) Expression.Convert((Expression) Expression.Property((Expression) Expression.Convert((Expression) info.Self, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType), typeof (PythonType)), (Expression) info.Result);
  }

  private static Expression GetWeakSlot(PythonTypeSlot slot)
  {
    return Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) slot), typeof (PythonTypeSlot));
  }

  private static Expression MakeTypeError(
    DynamicMetaObjectBinder binder,
    string name,
    PythonType type)
  {
    return binder.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("AttributeErrorForMissingAttribute", new Type[2]
    {
      typeof (string),
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) type.Name), Microsoft.Scripting.Ast.Utils.Constant((object) name)), typeof (object));
  }

  private static bool IsStandardObjectMethod(PythonTypeSlot dts)
  {
    return dts is BuiltinMethodDescriptor methodDescriptor && methodDescriptor.Template.Targets[0].DeclaringType == typeof (ObjectOps);
  }

  private static void MakeSlotsDeleteTarget(
    MetaUserObject.MemberBindingInfo info,
    ReflectedSlotProperty rsp)
  {
    MetaUserObject.MakeSlotsSetTargetHelper(info, rsp, (Expression) Expression.Field((Expression) null, typeof (Uninitialized).GetField("Instance")));
  }

  private static void MakeSlotsSetTargetHelper(
    MetaUserObject.MemberBindingInfo info,
    ReflectedSlotProperty rsp,
    Expression value)
  {
    ParameterExpression parameterExpression = Expression.Variable(typeof (object), "res");
    info.Body.AddVariable(parameterExpression);
    info.Body.FinishCondition((Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Convert((Expression) Expression.Assign((Expression) Expression.ArrayAccess(MetaUserObject.GetSlots(info.Args[0]), Microsoft.Scripting.Ast.Utils.Constant((object) rsp.Index)), Microsoft.Scripting.Ast.Utils.Convert(value, typeof (object))), parameterExpression.Type)), (Expression) parameterExpression));
  }

  private static DynamicMetaObject MakeSlotSet(
    MetaUserObject.SetBindingInfo info,
    PythonTypeSlot dts)
  {
    ParameterExpression parameterExpression1 = Expression.Variable(info.Args[1].Expression.Type, "res");
    info.Body.AddVariable(parameterExpression1);
    if (dts.GetType() == typeof (PythonProperty))
    {
      Expression right = (Expression) Expression.Property((Expression) Expression.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonProperty)), "fset");
      ParameterExpression parameterExpression2 = Expression.Variable(typeof (object), "tmpSet");
      info.Body.AddVariable(parameterExpression2);
      info.Body.FinishCondition((Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression2, right), (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) parameterExpression2, Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression1, info.Args[1].Expression), (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).InvokeOne, typeof (object), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).SharedContext), (Expression) parameterExpression2, info.Args[0].Expression, Microsoft.Scripting.Ast.Utils.Convert((Expression) parameterExpression1, typeof (object))), (Expression) Expression.Convert((Expression) parameterExpression1, typeof (object))), info.Action.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("UnsetableProperty")), typeof (object)))));
      return info.Body.GetMetaObject();
    }
    CodeContext sharedContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).SharedContext;
    info.Body.AddCondition((Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression1, info.Args[1].Expression), (Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTrySetValue"), Microsoft.Scripting.Ast.Utils.Constant((object) sharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(info.Args[0].Expression, typeof (object)), (Expression) Expression.Convert((Expression) Expression.Property((Expression) Expression.Convert(info.Args[0].Expression, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType), typeof (PythonType)), Microsoft.Scripting.Ast.Utils.Convert((Expression) parameterExpression1, typeof (object)))), Microsoft.Scripting.Ast.Utils.Convert((Expression) parameterExpression1, typeof (object)));
    return (DynamicMetaObject) null;
  }

  private DynamicMetaObject MakeDeleteMemberRule(MetaUserObject.DeleteBindingInfo info)
  {
    CodeContext sharedContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).SharedContext;
    DynamicMetaObject dynamicMetaObject = info.Args[0].Restrict(info.Args[0].GetRuntimeType());
    IPythonObject self = info.Args[0].Value as IPythonObject;
    if (info.Action.Name == "__class__")
      return new DynamicMetaObject(info.Action.Throw((Expression) Expression.New(typeof (TypeErrorException).GetConstructor(new Type[1]
      {
        typeof (string)
      }), Microsoft.Scripting.Ast.Utils.Constant((object) "can't delete __class__ attribute")), typeof (object)), dynamicMetaObject.Restrictions);
    PythonTypeSlot slot;
    if (self.PythonType.TryResolveSlot(sharedContext, "__delattr__", out slot) && !MetaUserObject.IsStandardObjectMethod(slot))
      MetaUserObject.MakeDeleteAttrTarget(info, self, slot);
    self.PythonType.TryResolveSlot(sharedContext, info.Action.Name, out slot);
    if (slot is ReflectedSlotProperty rsp)
      MetaUserObject.MakeSlotsDeleteTarget((MetaUserObject.MemberBindingInfo) info, rsp);
    if (!info.Body.IsFinal && slot != null)
      MetaUserObject.MakeSlotDelete(info, slot);
    if (!info.Body.IsFinal && self.PythonType.HasDictionary)
      MetaUserObject.MakeDictionaryDeleteTarget(info);
    if (!info.Body.IsFinal)
      info.Body.FinishCondition(this.FallbackDeleteError(info.Action, info.Args).Expression);
    DynamicMetaObject metaObject = info.Body.GetMetaObject(info.Args);
    DynamicMetaObject res = new DynamicMetaObject(metaObject.Expression, dynamicMetaObject.Restrictions.Merge(metaObject.Restrictions));
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) info.Action, res, info.Args, info.Validation);
  }

  private static DynamicMetaObject MakeSlotDelete(
    MetaUserObject.DeleteBindingInfo info,
    PythonTypeSlot dts)
  {
    if (dts.GetType() == typeof (PythonProperty))
    {
      Expression right = (Expression) Expression.Property((Expression) Expression.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonProperty)), "fdel");
      ParameterExpression parameterExpression = Expression.Variable(typeof (object), "tmpDel");
      info.Body.AddVariable(parameterExpression);
      info.Body.FinishCondition((Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression, right), (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).InvokeOne, typeof (object), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).SharedContext), (Expression) parameterExpression, info.Args[0].Expression), info.Action.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("UndeletableProperty")), typeof (object)))));
      return info.Body.GetMetaObject();
    }
    info.Body.AddCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTryDeleteValue"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).SharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(info.Args[0].Expression, typeof (object)), (Expression) Expression.Convert((Expression) Expression.Property((Expression) Expression.Convert(info.Args[0].Expression, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType), typeof (PythonType))), Microsoft.Scripting.Ast.Utils.Constant((object) null));
    return (DynamicMetaObject) null;
  }

  private static void MakeDeleteAttrTarget(
    MetaUserObject.DeleteBindingInfo info,
    IPythonObject self,
    PythonTypeSlot dts)
  {
    ParameterExpression var = Expression.Variable(typeof (object), "boundVal");
    info.Body.AddVariable(var);
    info.Body.AddCondition((Expression) Expression.Call(PythonTypeInfo._PythonOps.SlotTryGetBoundValue, Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).SharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(info.Args[0].Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) self.PythonType), typeof (PythonType)), (Expression) var), (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext((DynamicMetaObjectBinder) info.Action).InvokeOne, typeof (object), PythonContext.GetCodeContext((DynamicMetaObjectBinder) info.Action), (Expression) var, Microsoft.Scripting.Ast.Utils.Constant((object) info.Action.Name)));
  }

  private static void MakeDictionaryDeleteTarget(MetaUserObject.DeleteBindingInfo info)
  {
    info.Body.FinishCondition((Expression) Expression.Call(typeof (UserTypeOps).GetMethod("RemoveDictionaryValue"), (Expression) Expression.Convert(info.Args[0].Expression, typeof (IPythonObject)), Microsoft.Scripting.Ast.Utils.Constant((object) info.Action.Name)));
  }

  private static PythonTypeSlot FindSlot(
    CodeContext context,
    string name,
    IPythonObject sdo,
    out bool isOldStyle,
    out bool systemTypeResolution,
    out bool extensionMethodResolution)
  {
    PythonTypeSlot slot = (PythonTypeSlot) null;
    isOldStyle = false;
    systemTypeResolution = false;
    foreach (PythonType pythonType in (IEnumerable<PythonType>) sdo.PythonType.ResolutionOrder)
    {
      if (pythonType.IsOldClass)
        isOldStyle = true;
      if (pythonType.TryLookupSlot(context, name, out slot))
      {
        if (!(slot is ClassMethodDescriptor))
        {
          systemTypeResolution = pythonType.IsSystemType;
          break;
        }
        break;
      }
    }
    extensionMethodResolution = false;
    if (slot == null)
    {
      extensionMethodResolution = true;
      MemberGroup member = context.ModuleContext.ExtensionMethods.GetBinder(context.LanguageContext).GetMember(MemberRequestKind.Get, sdo.PythonType.UnderlyingSystemType, name);
      if (member.Count > 0)
        slot = PythonTypeOps.GetSlot(member, name, false);
    }
    return slot;
  }

  private DynamicMetaObject FallbackGetError(
    DynamicMetaObjectBinder action,
    DynamicMetaObject codeContext)
  {
    if (this._baseMetaObject != null)
      return this.Fallback(action, codeContext);
    if (BindingHelpers.IsNoThrow(action))
      return new DynamicMetaObject((Expression) Expression.Field((Expression) null, typeof (OperationFailed).GetField("Value")), BindingRestrictions.Empty);
    return action is PythonGetMemberBinder ? new DynamicMetaObject(MetaUserObject.MakeTypeError(action, MetaPythonObject.GetGetMemberName(action), this.PythonType), BindingRestrictions.Empty) : MetaPythonObject.GetMemberFallback((DynamicMetaObject) this, action, codeContext);
  }

  private DynamicMetaObject FallbackDeleteError(DeleteMemberBinder action, DynamicMetaObject[] args)
  {
    if (this._baseMetaObject != null)
      return this._baseMetaObject.BindDeleteMember(action);
    return action is PythonDeleteMemberBinder ? new DynamicMetaObject(MetaUserObject.MakeTypeError((DynamicMetaObjectBinder) action, action.Name, ((IPythonObject) args[0].Value).PythonType), BindingRestrictions.Empty) : action.FallbackDeleteMember(this.Restrict(this.GetLimitType()));
  }

  private static Expression GetSlots(DynamicMetaObject self)
  {
    FieldInfo field = self.LimitType.GetField(".slots_and_weakref");
    return field != (FieldInfo) null ? (Expression) Expression.Field((Expression) Expression.Convert(self.Expression, self.LimitType), field) : (Expression) Expression.Call((Expression) Expression.Convert(self.Expression, typeof (IPythonObject)), typeof (IPythonObject).GetMethod(nameof (GetSlots)));
  }

  internal abstract class GetOrInvokeBinderHelper<TResult>
  {
    protected readonly IPythonObject _value;
    protected bool _extensionMethodRestriction;

    public GetOrInvokeBinderHelper(IPythonObject value) => this._value = value;

    public TResult Bind(CodeContext context, string name)
    {
      IPythonObject sdo = this.Value;
      PythonTypeSlot dts;
      if (MetaUserObject.TryGetGetAttribute(context, sdo.PythonType, out dts))
        return this.BindGetAttribute(dts);
      bool isOldStyle;
      bool systemTypeResolution;
      bool extensionMethodResolution;
      PythonTypeSlot slot1 = MetaUserObject.FindSlot(context, name, sdo, out isOldStyle, out systemTypeResolution, out extensionMethodResolution);
      this._extensionMethodRestriction = extensionMethodResolution;
      if (!isOldStyle || slot1 is ReflectedSlotProperty)
      {
        if (sdo.PythonType.HasDictionary && (slot1 == null || !slot1.IsSetDescriptor(context, sdo.PythonType)))
          this.MakeDictionaryAccess();
        if (slot1 != null)
          this.MakeSlotAccess(slot1, systemTypeResolution);
      }
      else
        this.MakeOldStyleAccess();
      if (!this.IsFinal)
      {
        PythonTypeSlot slot2;
        if (this.Value.PythonType.TryResolveSlot(context, "__getattr__", out slot2))
          this.MakeGetAttrAccess(slot2);
        this.MakeTypeError();
      }
      return this.FinishRule();
    }

    protected abstract void MakeTypeError();

    protected abstract void MakeGetAttrAccess(PythonTypeSlot getattr);

    protected abstract bool IsFinal { get; }

    protected abstract void MakeSlotAccess(PythonTypeSlot foundSlot, bool systemTypeResolution);

    protected abstract TResult BindGetAttribute(PythonTypeSlot foundSlot);

    protected abstract TResult FinishRule();

    protected abstract void MakeDictionaryAccess();

    protected abstract void MakeOldStyleAccess();

    public IPythonObject Value => this._value;
  }

  private abstract class MetaGetBinderHelper : 
    MetaUserObject.GetOrInvokeBinderHelper<DynamicMetaObject>
  {
    private readonly DynamicMetaObject _self;
    private readonly MetaUserObject.GetBindingInfo _bindingInfo;
    protected readonly MetaUserObject _target;
    private readonly DynamicMetaObjectBinder _binder;
    protected readonly DynamicMetaObject _codeContext;
    private string _resolution = "GetMember ";

    public MetaGetBinderHelper(
      MetaUserObject target,
      DynamicMetaObjectBinder binder,
      DynamicMetaObject codeContext)
      : base(target.Value)
    {
      this._target = target;
      this._self = this._target.Restrict(this.Value.GetType());
      this._binder = binder;
      this._codeContext = codeContext;
      this._bindingInfo = new MetaUserObject.GetBindingInfo(this._binder, new DynamicMetaObject[1]
      {
        (DynamicMetaObject) this._target
      }, Expression.Variable(this.Expression.Type, "self"), Expression.Variable(typeof (object), "lookupRes"), new ConditionalBuilder(this._binder), BindingHelpers.GetValidationInfo(this._self, this.Value.PythonType));
    }

    private DynamicMetaObject MakeGetAttributeRule(
      MetaUserObject.GetBindingInfo info,
      IPythonObject obj,
      PythonTypeSlot slot,
      DynamicMetaObject codeContext)
    {
      CodeContext sharedContext = PythonContext.GetPythonContext(info.Action).SharedContext;
      Type finalSystemType = obj.PythonType.FinalSystemType;
      PythonTypeSlot dts;
      if (typeof (IDynamicMetaObjectProvider).IsAssignableFrom(finalSystemType) && MetaUserObject.TryGetGetAttribute(sharedContext, DynamicHelpers.GetPythonTypeFromType(finalSystemType), out dts) && dts == slot)
        return this.FallbackError();
      PythonTypeSlot slot1;
      obj.PythonType.TryResolveSlot(sharedContext, "__getattr__", out slot1);
      DynamicMetaObject dynamicMetaObject = this._target.Restrict(this.Value.GetType());
      string name = BindingHelpers.IsNoThrow(info.Action) ? "GetAttributeNoThrow" : "GetAttribute";
      return BindingHelpers.AddDynamicTestAndDefer(info.Action, new DynamicMetaObject((Expression) Expression.Call(typeof (UserTypeOps).GetMethod(name), (Expression) Expression.Constant((object) PythonContext.GetPythonContext(info.Action).SharedContext), info.Args[0].Expression, (Expression) Expression.Constant((object) MetaPythonObject.GetGetMemberName(info.Action)), (Expression) Expression.Constant((object) slot, typeof (PythonTypeSlot)), (Expression) Expression.Constant((object) slot1, typeof (PythonTypeSlot)), (Expression) Expression.Constant((object) new SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, string, object>>>())), dynamicMetaObject.Restrictions), info.Args, info.Validation);
    }

    protected abstract DynamicMetaObject FallbackError();

    protected abstract DynamicMetaObject Fallback();

    protected virtual Expression Invoke(Expression res)
    {
      return this.Invoke(new DynamicMetaObject(res, BindingRestrictions.Empty)).Expression;
    }

    protected virtual DynamicMetaObject Invoke(DynamicMetaObject res) => res;

    protected override DynamicMetaObject BindGetAttribute(PythonTypeSlot foundSlot)
    {
      return this.Invoke(this.MakeGetAttributeRule(this._bindingInfo, this.Value, foundSlot, this._codeContext));
    }

    protected override void MakeGetAttrAccess(PythonTypeSlot getattr)
    {
      this._resolution += "GetAttr ";
      this.MakeGetAttrRule(this._bindingInfo, MetaUserObject.GetWeakSlot(getattr), this._codeContext);
    }

    protected override void MakeTypeError()
    {
      this._bindingInfo.Body.FinishCondition(this.FallbackError().Expression);
    }

    protected override bool IsFinal => this._bindingInfo.Body.IsFinal;

    protected override DynamicMetaObject FinishRule()
    {
      DynamicMetaObject metaObject = this._bindingInfo.Body.GetMetaObject((DynamicMetaObject) this._target);
      DynamicMetaObject res = new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
      {
        this._bindingInfo.Self,
        this._bindingInfo.Result
      }, (Expression) Expression.Assign((Expression) this._bindingInfo.Self, this._self.Expression), metaObject.Expression), this._self.Restrictions.Merge(metaObject.Restrictions));
      if (this._extensionMethodRestriction)
        res = new DynamicMetaObject(res.Expression, res.Restrictions.Merge(((CodeContext) this._codeContext.Value).ModuleContext.ExtensionMethods.GetRestriction(this._codeContext.Expression)));
      return BindingHelpers.AddDynamicTestAndDefer(this._binder, res, new DynamicMetaObject[1]
      {
        (DynamicMetaObject) this._target
      }, this._bindingInfo.Validation);
    }

    private void MakeGetAttrRule(
      MetaUserObject.GetBindingInfo info,
      Expression getattr,
      DynamicMetaObject codeContext)
    {
      info.Body.AddCondition((Expression) MetaUserObject.MakeGetAttrTestAndGet(info, getattr), this.Invoke(this.MakeGetAttrCall(info, codeContext)));
    }

    private Expression MakeGetAttrCall(
      MetaUserObject.GetBindingInfo info,
      DynamicMetaObject codeContext)
    {
      Expression expr = (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(info.Action).InvokeOne, typeof (object), PythonContext.GetCodeContext(info.Action), (Expression) info.Result, (Expression) Expression.Constant((object) MetaPythonObject.GetGetMemberName(info.Action)));
      return this.MaybeMakeNoThrow(info, expr);
    }

    private Expression MaybeMakeNoThrow(MetaUserObject.GetBindingInfo info, Expression expr)
    {
      if (BindingHelpers.IsNoThrow(info.Action))
      {
        DynamicMetaObject dynamicMetaObject = this.FallbackError();
        ParameterExpression left = Expression.Variable(typeof (object), "getAttrRes");
        expr = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left
        }, (Expression) Expression.Block((Expression) Microsoft.Scripting.Ast.Utils.Try((Expression) Expression.Assign((Expression) left, Microsoft.Scripting.Ast.Utils.Convert(expr, typeof (object)))).Catch(typeof (MissingMemberException), (Expression) Expression.Assign((Expression) left, Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (object)))), (Expression) left));
      }
      return expr;
    }

    protected override void MakeSlotAccess(PythonTypeSlot foundSlot, bool systemTypeResolution)
    {
      this._resolution = $"{this._resolution}{(object) CompilerHelpers.GetType((object) foundSlot)} ";
      if (systemTypeResolution)
        this._bindingInfo.Body.FinishCondition(this.Fallback().Expression);
      else
        this.MakeSlotAccess(foundSlot);
    }

    private void MakeSlotAccess(PythonTypeSlot dts)
    {
      if (dts is ReflectedSlotProperty reflectedSlotProperty)
      {
        this._bindingInfo.Body.AddCondition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) this._bindingInfo.Result, (Expression) Expression.ArrayAccess(MetaUserObject.GetSlots((DynamicMetaObject) this._target), (Expression) Expression.Constant((object) reflectedSlotProperty.Index))), (Expression) Expression.Field((Expression) null, typeof (Uninitialized).GetField("Instance"))), this.Invoke((Expression) this._bindingInfo.Result));
      }
      else
      {
        if (dts is PythonTypeUserDescriptorSlot userDescriptorSlot)
          this._bindingInfo.Body.FinishCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetUserSlotValue"), (Expression) Expression.Constant((object) PythonContext.GetPythonContext(this._bindingInfo.Action).SharedContext), (Expression) Expression.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) userDescriptorSlot), typeof (PythonTypeUserDescriptorSlot)), this._target.Expression, (Expression) Expression.Property((Expression) Expression.Convert((Expression) this._bindingInfo.Self, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType)));
        if (dts.GetType() == typeof (PythonProperty))
        {
          Expression right = (Expression) Expression.Property((Expression) Expression.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonProperty)), "fget");
          ParameterExpression parameterExpression = Expression.Variable(typeof (object), "tmpGet");
          this._bindingInfo.Body.AddVariable(parameterExpression);
          this._bindingInfo.Body.FinishCondition((Expression) Expression.Block((Expression) Expression.Assign((Expression) parameterExpression, right), (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) parameterExpression, (Expression) Expression.Constant((object) null)), this.Invoke((Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(this._bindingInfo.Action).InvokeOne, typeof (object), (Expression) Expression.Constant((object) PythonContext.GetPythonContext(this._bindingInfo.Action).SharedContext), (Expression) parameterExpression, (Expression) this._bindingInfo.Self)), this._binder.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("UnreadableProperty")), typeof (object)))));
        }
        else
        {
          Expression condition = (Expression) Expression.Call(PythonTypeInfo._PythonOps.SlotTryGetBoundValue, (Expression) Expression.Constant((object) PythonContext.GetPythonContext(this._bindingInfo.Action).SharedContext), (Expression) Expression.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert((Expression) this._bindingInfo.Self, typeof (object)), (Expression) Expression.Property((Expression) Expression.Convert((Expression) this._bindingInfo.Self, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType), (Expression) this._bindingInfo.Result);
          Expression body = this.Invoke((Expression) this._bindingInfo.Result);
          if (dts.GetAlwaysSucceeds)
            this._bindingInfo.Body.FinishCondition((Expression) Expression.Block(condition, body));
          else
            this._bindingInfo.Body.AddCondition(condition, body);
        }
      }
    }

    protected override void MakeDictionaryAccess()
    {
      this._resolution += "Dictionary ";
      FieldInfo field = this._target.LimitType.GetField(".dict");
      Expression expression = !(field != (FieldInfo) null) ? (Expression) Expression.Property((Expression) Expression.Convert((Expression) this._bindingInfo.Self, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.Dict) : (Expression) Expression.Field((Expression) Expression.Convert((Expression) this._bindingInfo.Self, this._target.LimitType), field);
      IList<string> optimizedInstanceNames = this.Value.PythonType.GetOptimizedInstanceNames();
      int num;
      if (optimizedInstanceNames != null && (num = optimizedInstanceNames.IndexOf(MetaPythonObject.GetGetMemberName(this._bindingInfo.Action))) != -1)
        this._bindingInfo.Body.AddCondition((Expression) Expression.Call(typeof (UserTypeOps).GetMethod("TryGetDictionaryValue"), expression, Microsoft.Scripting.Ast.Utils.Constant((object) MetaPythonObject.GetGetMemberName(this._bindingInfo.Action)), (Expression) Expression.Constant((object) this.Value.PythonType.GetOptimizedInstanceVersion()), (Expression) Expression.Constant((object) num), (Expression) this._bindingInfo.Result), this.Invoke(new DynamicMetaObject((Expression) this._bindingInfo.Result, BindingRestrictions.Empty)).Expression);
      else
        this._bindingInfo.Body.AddCondition((Expression) Expression.AndAlso((Expression) Expression.NotEqual(expression, (Expression) Expression.Constant((object) null)), (Expression) Expression.Call(expression, PythonTypeInfo._PythonDictionary.TryGetvalue, Microsoft.Scripting.Ast.Utils.Constant((object) MetaPythonObject.GetGetMemberName(this._bindingInfo.Action)), (Expression) this._bindingInfo.Result)), this.Invoke(new DynamicMetaObject((Expression) this._bindingInfo.Result, BindingRestrictions.Empty)).Expression);
    }

    protected override void MakeOldStyleAccess()
    {
      this._resolution += "MixedOldStyle ";
      this._bindingInfo.Body.AddCondition((Expression) Expression.Call(typeof (UserTypeOps).GetMethod("TryGetMixedNewStyleOldStyleSlot"), (Expression) Expression.Constant((object) PythonContext.GetPythonContext(this._bindingInfo.Action).SharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) this._bindingInfo.Self, typeof (object)), Microsoft.Scripting.Ast.Utils.Constant((object) MetaPythonObject.GetGetMemberName(this._bindingInfo.Action)), (Expression) this._bindingInfo.Result), this.Invoke((Expression) this._bindingInfo.Result));
    }

    public Expression Expression => this._target.Expression;
  }

  internal class FastGetBinderHelper : MetaUserObject.GetOrInvokeBinderHelper<FastGetBase>
  {
    private readonly int _version;
    private readonly PythonGetMemberBinder _binder;
    private readonly CallSite<Func<CallSite, object, CodeContext, object>> _site;
    private readonly CodeContext _context;
    private bool _dictAccess;
    private bool _noOptimizedForm;
    private PythonTypeSlot _slot;
    private PythonTypeSlot _getattrSlot;

    public FastGetBinderHelper(
      CodeContext context,
      CallSite<Func<CallSite, object, CodeContext, object>> site,
      IPythonObject value,
      PythonGetMemberBinder binder)
      : base(value)
    {
      this._version = value.PythonType.Version;
      this._binder = binder;
      this._site = site;
      this._context = context;
    }

    protected override void MakeTypeError()
    {
    }

    protected override bool IsFinal => this._slot != null && this._slot.GetAlwaysSucceeds;

    protected override void MakeSlotAccess(PythonTypeSlot foundSlot, bool systemTypeResolution)
    {
      if (systemTypeResolution)
        this._binder.Context.Binder.TryResolveSlot(this._context, this.Value.PythonType, this.Value.PythonType, this._binder.Name, out foundSlot);
      this._slot = foundSlot;
    }

    public FastBindResult<Func<CallSite, object, CodeContext, object>> GetBinding(
      CodeContext context,
      string name)
    {
      Dictionary<CachedGetKey, FastGetBase> cachedGets = this.GetCachedGets();
      CachedGetKey key = CachedGetKey.Make(name, context.ModuleContext.ExtensionMethods);
      FastGetBase fastGetBase1;
      lock (cachedGets)
      {
        if (cachedGets.TryGetValue(key, out fastGetBase1))
        {
          if (fastGetBase1.IsValid(this.Value.PythonType))
            goto label_9;
        }
        FastGetBase fastGetBase2 = this.Bind(context, name);
        if (fastGetBase2 != null)
        {
          fastGetBase1 = fastGetBase2;
          if (fastGetBase1.ShouldCache)
            cachedGets[key] = fastGetBase1;
        }
      }
label_9:
      return fastGetBase1 != null && fastGetBase1.ShouldUseNonOptimizedSite ? new FastBindResult<Func<CallSite, object, CodeContext, object>>(fastGetBase1._func, fastGetBase1.ShouldCache) : new FastBindResult<Func<CallSite, object, CodeContext, object>>();
    }

    private Dictionary<CachedGetKey, FastGetBase> GetCachedGets()
    {
      if (this._binder.IsNoThrow)
      {
        Dictionary<CachedGetKey, FastGetBase> cachedTryGets = this.Value.PythonType._cachedTryGets;
        if (cachedTryGets == null)
        {
          Interlocked.CompareExchange<Dictionary<CachedGetKey, FastGetBase>>(ref this.Value.PythonType._cachedTryGets, new Dictionary<CachedGetKey, FastGetBase>(), (Dictionary<CachedGetKey, FastGetBase>) null);
          cachedTryGets = this.Value.PythonType._cachedTryGets;
        }
        return cachedTryGets;
      }
      Dictionary<CachedGetKey, FastGetBase> cachedGets = this.Value.PythonType._cachedGets;
      if (cachedGets == null)
      {
        Interlocked.CompareExchange<Dictionary<CachedGetKey, FastGetBase>>(ref this.Value.PythonType._cachedGets, new Dictionary<CachedGetKey, FastGetBase>(), (Dictionary<CachedGetKey, FastGetBase>) null);
        cachedGets = this.Value.PythonType._cachedGets;
      }
      return cachedGets;
    }

    protected override FastGetBase FinishRule()
    {
      return this._noOptimizedForm ? (FastGetBase) null : (!(this._slot is ReflectedSlotProperty slot) ? (!this._dictAccess ? (!(this._slot is PythonTypeUserDescriptorSlot) ? (FastGetBase) new GetMemberDelegates(OptimizedGetKind.SlotOnly, this.Value.PythonType, this._binder, this._binder.Name, this._version, this._slot, this._getattrSlot, (SlotGetValue) null, this.FallbackError(), this._context.ModuleContext.ExtensionMethods) : (FastGetBase) new GetMemberDelegates(OptimizedGetKind.UserSlotOnly, this.Value.PythonType, this._binder, this._binder.Name, this._version, this._slot, this._getattrSlot, (SlotGetValue) null, this.FallbackError(), this._context.ModuleContext.ExtensionMethods)) : (!(this._slot is PythonTypeUserDescriptorSlot) ? (FastGetBase) new GetMemberDelegates(OptimizedGetKind.SlotDict, this.Value.PythonType, this._binder, this._binder.Name, this._version, this._slot, this._getattrSlot, (SlotGetValue) null, this.FallbackError(), this._context.ModuleContext.ExtensionMethods) : (FastGetBase) new GetMemberDelegates(OptimizedGetKind.UserSlotDict, this.Value.PythonType, this._binder, this._binder.Name, this._version, this._slot, this._getattrSlot, (SlotGetValue) null, this.FallbackError(), this._context.ModuleContext.ExtensionMethods))) : (FastGetBase) new GetMemberDelegates(OptimizedGetKind.PropertySlot, this.Value.PythonType, this._binder, this._binder.Name, this._version, this._slot, this._getattrSlot, slot.Getter, this.FallbackError(), this._context.ModuleContext.ExtensionMethods));
    }

    private Func<CallSite, object, CodeContext, object> FallbackError()
    {
      if (typeof (IDynamicMetaObjectProvider).IsAssignableFrom(this.Value.PythonType.FinalSystemType))
        return ((IFastGettable) this.Value).MakeGetBinding<Func<CallSite, object, CodeContext, object>>(this._site, this._binder, this._context, this._binder.Name);
      if (this._binder.IsNoThrow)
        return (Func<CallSite, object, CodeContext, object>) ((site, self, context) => (object) OperationFailed.Value);
      string name = this._binder.Name;
      return (Func<CallSite, object, CodeContext, object>) ((site, self, context) =>
      {
        throw PythonOps.AttributeErrorForMissingAttribute(((IPythonObject) self).PythonType.Name, name);
      });
    }

    protected override void MakeDictionaryAccess() => this._dictAccess = true;

    protected override FastGetBase BindGetAttribute(PythonTypeSlot foundSlot)
    {
      Type finalSystemType = this.Value.PythonType.FinalSystemType;
      PythonTypeSlot dts;
      if (typeof (IDynamicMetaObjectProvider).IsAssignableFrom(finalSystemType) && MetaUserObject.TryGetGetAttribute(this._context, DynamicHelpers.GetPythonTypeFromType(finalSystemType), out dts) && dts == foundSlot)
        return (FastGetBase) new ChainedUserGet(this._binder, this._version, this.FallbackError());
      PythonTypeSlot slot;
      this.Value.PythonType.TryResolveSlot(this._context, "__getattr__", out slot);
      return (FastGetBase) new GetAttributeDelegates(this._binder, this._binder.Name, this._version, foundSlot, slot);
    }

    protected override void MakeGetAttrAccess(PythonTypeSlot getattr)
    {
      this._getattrSlot = getattr;
    }

    protected override void MakeOldStyleAccess() => this._noOptimizedForm = true;
  }

  private class GetBinderHelper : MetaUserObject.MetaGetBinderHelper
  {
    private readonly DynamicMetaObjectBinder _binder;

    public GetBinderHelper(
      MetaUserObject target,
      DynamicMetaObjectBinder binder,
      DynamicMetaObject codeContext)
      : base(target, binder, codeContext)
    {
      this._binder = binder;
    }

    protected override DynamicMetaObject Fallback()
    {
      return MetaPythonObject.GetMemberFallback((DynamicMetaObject) this._target, this._binder, this._codeContext);
    }

    protected override DynamicMetaObject FallbackError()
    {
      return this._target.FallbackGetError(this._binder, this._codeContext);
    }
  }

  private class InvokeBinderHelper : MetaUserObject.MetaGetBinderHelper
  {
    private readonly InvokeMemberBinder _binder;
    private readonly DynamicMetaObject[] _args;

    public InvokeBinderHelper(
      MetaUserObject target,
      InvokeMemberBinder binder,
      DynamicMetaObject[] args,
      DynamicMetaObject codeContext)
      : base(target, (DynamicMetaObjectBinder) binder, codeContext)
    {
      this._binder = binder;
      this._args = args;
    }

    protected override DynamicMetaObject Fallback()
    {
      return this._binder.FallbackInvokeMember((DynamicMetaObject) this._target, this._args);
    }

    protected override DynamicMetaObject FallbackError()
    {
      return this._target._baseMetaObject != null ? this._target._baseMetaObject.BindInvokeMember(this._binder, this._args) : this.Fallback();
    }

    protected override DynamicMetaObject Invoke(DynamicMetaObject res)
    {
      return this._binder.FallbackInvoke(res, this._args, (DynamicMetaObject) null);
    }
  }

  internal abstract class SetBinderHelper<TResult>
  {
    private readonly IPythonObject _instance;
    private readonly object _value;
    protected readonly CodeContext _context;

    public SetBinderHelper(CodeContext context, IPythonObject instance, object value)
    {
      this._instance = instance;
      this._value = value;
      this._context = context;
    }

    public TResult Bind(string name)
    {
      bool flag = false;
      PythonTypeSlot slot1;
      if (this._instance.PythonType.TryResolveSlot(this._context, "__setattr__", out slot1) && !MetaUserObject.IsStandardObjectMethod(slot1) && slot1 != null)
      {
        this.MakeSetAttrTarget(slot1);
        flag = true;
      }
      if (!flag)
      {
        bool systemTypeResolution;
        PythonTypeSlot slot2 = MetaUserObject.FindSlot(this._context, name, this._instance, out bool _, out systemTypeResolution, out bool _);
        if (slot2 is ReflectedSlotProperty prop)
        {
          this.MakeSlotsSetTarget(prop);
          flag = true;
        }
        else if (slot2 != null && slot2.IsSetDescriptor(this._context, this._instance.PythonType))
        {
          this.MakeSlotSetOrFallback(slot2, systemTypeResolution);
          flag = systemTypeResolution || slot2.GetType() == typeof (PythonProperty);
        }
      }
      if (!flag)
      {
        if (this._instance.PythonType.HasDictionary)
          this.MakeDictionarySetTarget();
        else
          this.MakeFallback();
      }
      return this.Finish();
    }

    public IPythonObject Instance => this._instance;

    public object Value => this._value;

    protected abstract TResult Finish();

    protected abstract void MakeSetAttrTarget(PythonTypeSlot dts);

    protected abstract void MakeSlotsSetTarget(ReflectedSlotProperty prop);

    protected abstract void MakeSlotSetOrFallback(PythonTypeSlot dts, bool systemTypeResolution);

    protected abstract void MakeDictionarySetTarget();

    protected abstract void MakeFallback();
  }

  internal class FastSetBinderHelper<TValue> : 
    MetaUserObject.SetBinderHelper<SetMemberDelegates<TValue>>
  {
    private readonly PythonSetMemberBinder _binder;
    private readonly int _version;
    private PythonTypeSlot _setattrSlot;
    private ReflectedSlotProperty _slotProp;
    private bool _unsupported;
    private bool _dictSet;

    public FastSetBinderHelper(
      CodeContext context,
      IPythonObject self,
      object value,
      PythonSetMemberBinder binder)
      : base(context, self, value)
    {
      this._binder = binder;
      this._version = self.PythonType.Version;
    }

    protected override SetMemberDelegates<TValue> Finish()
    {
      if (this._unsupported)
        return new SetMemberDelegates<TValue>(this._context, this.Instance.PythonType, OptimizedSetKind.None, this._binder.Name, this._version, this._setattrSlot, (SlotSetValue) null);
      if (this._setattrSlot != null)
        return new SetMemberDelegates<TValue>(this._context, this.Instance.PythonType, OptimizedSetKind.SetAttr, this._binder.Name, this._version, this._setattrSlot, (SlotSetValue) null);
      if (this._slotProp != null)
        return new SetMemberDelegates<TValue>(this._context, this.Instance.PythonType, OptimizedSetKind.UserSlot, this._binder.Name, this._version, (PythonTypeSlot) null, this._slotProp.Setter);
      return this._dictSet ? new SetMemberDelegates<TValue>(this._context, this.Instance.PythonType, OptimizedSetKind.SetDict, this._binder.Name, this._version, (PythonTypeSlot) null, (SlotSetValue) null) : new SetMemberDelegates<TValue>(this._context, this.Instance.PythonType, OptimizedSetKind.Error, this._binder.Name, this._version, (PythonTypeSlot) null, (SlotSetValue) null);
    }

    public FastBindResult<Func<CallSite, object, TValue, object>> MakeSet()
    {
      Dictionary<SetMemberKey, FastSetBase> cachedSets = this.GetCachedSets();
      FastSetBase fastSetBase;
      lock (cachedSets)
      {
        SetMemberKey key = new SetMemberKey(typeof (TValue), this._binder.Name);
        if (cachedSets.TryGetValue(key, out fastSetBase))
        {
          if (fastSetBase._version == this.Instance.PythonType.Version)
            goto label_8;
        }
        fastSetBase = (FastSetBase) this.Bind(this._binder.Name);
        if (fastSetBase != null)
          cachedSets[key] = fastSetBase;
      }
label_8:
      return fastSetBase.ShouldUseNonOptimizedSite ? new FastBindResult<Func<CallSite, object, TValue, object>>((Func<CallSite, object, TValue, object>) fastSetBase._func, false) : new FastBindResult<Func<CallSite, object, TValue, object>>();
    }

    private Dictionary<SetMemberKey, FastSetBase> GetCachedSets()
    {
      Dictionary<SetMemberKey, FastSetBase> cachedSets = this.Instance.PythonType._cachedSets;
      if (cachedSets == null)
      {
        Interlocked.CompareExchange<Dictionary<SetMemberKey, FastSetBase>>(ref this.Instance.PythonType._cachedSets, new Dictionary<SetMemberKey, FastSetBase>(), (Dictionary<SetMemberKey, FastSetBase>) null);
        cachedSets = this.Instance.PythonType._cachedSets;
      }
      return cachedSets;
    }

    protected override void MakeSlotSetOrFallback(PythonTypeSlot dts, bool systemTypeResolution)
    {
      this._unsupported = true;
    }

    protected override void MakeSlotsSetTarget(ReflectedSlotProperty prop) => this._slotProp = prop;

    protected override void MakeFallback()
    {
    }

    protected override void MakeSetAttrTarget(PythonTypeSlot dts) => this._setattrSlot = dts;

    protected override void MakeDictionarySetTarget() => this._dictSet = true;
  }

  internal class MetaSetBinderHelper : MetaUserObject.SetBinderHelper<DynamicMetaObject>
  {
    private readonly MetaUserObject _target;
    private readonly DynamicMetaObject _value;
    private readonly MetaUserObject.SetBindingInfo _info;
    private DynamicMetaObject _result;
    private string _resolution = "SetMember ";

    public MetaSetBinderHelper(
      MetaUserObject target,
      DynamicMetaObject value,
      SetMemberBinder binder)
      : base(PythonContext.GetPythonContext((DynamicMetaObjectBinder) binder).SharedContext, target.Value, value.Value)
    {
      this._target = target;
      this._value = value;
      this._info = new MetaUserObject.SetBindingInfo(binder, new DynamicMetaObject[2]
      {
        (DynamicMetaObject) target,
        value
      }, new ConditionalBuilder((DynamicMetaObjectBinder) binder), BindingHelpers.GetValidationInfo((DynamicMetaObject) target, this.Instance.PythonType));
    }

    protected override void MakeSetAttrTarget(PythonTypeSlot dts)
    {
      ParameterExpression var = Expression.Variable(typeof (object), "boundVal");
      this._info.Body.AddVariable(var);
      this._info.Body.AddCondition((Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTryGetValue"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonContext.GetPythonContext((DynamicMetaObjectBinder) this._info.Action).SharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) dts), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(this._info.Args[0].Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) this.Instance.PythonType), typeof (PythonType)), (Expression) var), (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext((DynamicMetaObjectBinder) this._info.Action).Invoke(new CallSignature(2)), typeof (object), PythonContext.GetCodeContext((DynamicMetaObjectBinder) this._info.Action), (Expression) var, Microsoft.Scripting.Ast.Utils.Constant((object) this._info.Action.Name), this._info.Args[1].Expression));
      this._info.Body.FinishCondition(this.FallbackSetError(this._info.Action, this._info.Args[1]).Expression);
      this._result = this._info.Body.GetMetaObject((DynamicMetaObject) this._target, this._value);
      this._resolution += "SetAttr ";
    }

    protected override DynamicMetaObject Finish()
    {
      this._result = new DynamicMetaObject(this._result.Expression, this._target.Restrict(this.Instance.GetType()).Restrictions.Merge(this._result.Restrictions));
      return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) this._info.Action, this._result, new DynamicMetaObject[2]
      {
        (DynamicMetaObject) this._target,
        this._value
      }, this._info.Validation);
    }

    protected override void MakeFallback()
    {
      this._info.Body.FinishCondition(this.FallbackSetError(this._info.Action, this._value).Expression);
      this._result = this._info.Body.GetMetaObject((DynamicMetaObject) this._target, this._value);
    }

    protected override void MakeDictionarySetTarget()
    {
      this._resolution += "Dictionary ";
      FieldInfo field1 = this._info.Args[0].LimitType.GetField(".dict");
      if (field1 != (FieldInfo) null)
      {
        FieldInfo field2 = this._info.Args[0].LimitType.GetField(".class");
        IList<string> optimizedInstanceNames = this.Instance.PythonType.GetOptimizedInstanceNames();
        int num;
        if (field2 != (FieldInfo) null && optimizedInstanceNames != null && (num = optimizedInstanceNames.IndexOf(this._info.Action.Name)) != -1)
          this._info.Body.FinishCondition((Expression) Expression.Call(typeof (UserTypeOps).GetMethod("FastSetDictionaryValueOptimized"), (Expression) Expression.Field((Expression) Expression.Convert(this._info.Args[0].Expression, this._info.Args[0].LimitType), field2), (Expression) Expression.Field((Expression) Expression.Convert(this._info.Args[0].Expression, this._info.Args[0].LimitType), field1), Microsoft.Scripting.Ast.Utils.Constant((object) this._info.Action.Name), Microsoft.Scripting.Ast.Utils.Convert(this._info.Args[1].Expression, typeof (object)), (Expression) Expression.Constant((object) this.Instance.PythonType.GetOptimizedInstanceVersion()), (Expression) Expression.Constant((object) num)));
        else
          this._info.Body.FinishCondition((Expression) Expression.Call(typeof (UserTypeOps).GetMethod("FastSetDictionaryValue"), (Expression) Expression.Field((Expression) Expression.Convert(this._info.Args[0].Expression, this._info.Args[0].LimitType), field1), Microsoft.Scripting.Ast.Utils.Constant((object) this._info.Action.Name), Microsoft.Scripting.Ast.Utils.Convert(this._info.Args[1].Expression, typeof (object))));
      }
      else
        this._info.Body.FinishCondition((Expression) Expression.Call(typeof (UserTypeOps).GetMethod("SetDictionaryValue"), (Expression) Expression.Convert(this._info.Args[0].Expression, typeof (IPythonObject)), Microsoft.Scripting.Ast.Utils.Constant((object) this._info.Action.Name), Microsoft.Scripting.Ast.Utils.Convert(this._info.Args[1].Expression, typeof (object))));
      this._result = this._info.Body.GetMetaObject((DynamicMetaObject) this._target, this._value);
    }

    protected override void MakeSlotSetOrFallback(PythonTypeSlot dts, bool systemTypeResolution)
    {
      if (systemTypeResolution)
        this._result = this._target.Fallback(this._info.Action, this._value);
      else
        this._result = MetaUserObject.MakeSlotSet(this._info, dts);
    }

    protected override void MakeSlotsSetTarget(ReflectedSlotProperty prop)
    {
      this._resolution += "Slot ";
      MetaUserObject.MakeSlotsSetTargetHelper((MetaUserObject.MemberBindingInfo) this._info, prop, this._value.Expression);
      this._result = this._info.Body.GetMetaObject((DynamicMetaObject) this._target, this._value);
    }

    private DynamicMetaObject FallbackSetError(SetMemberBinder action, DynamicMetaObject value)
    {
      if (this._target._baseMetaObject != null)
        return this._target._baseMetaObject.BindSetMember(action, value);
      return action is PythonSetMemberBinder ? new DynamicMetaObject(MetaUserObject.MakeTypeError((DynamicMetaObjectBinder) action, action.Name, this.Instance.PythonType), BindingRestrictions.Empty) : this._info.Action.FallbackSetMember(this._target.Restrict(this._target.GetLimitType()), value);
    }
  }

  private class MemberBindingInfo
  {
    public readonly ConditionalBuilder Body;
    public readonly DynamicMetaObject[] Args;
    public readonly ValidationInfo Validation;

    public MemberBindingInfo(
      DynamicMetaObject[] args,
      ConditionalBuilder body,
      ValidationInfo validation)
    {
      this.Body = body;
      this.Validation = validation;
      this.Args = args;
    }
  }

  private class DeleteBindingInfo : MetaUserObject.MemberBindingInfo
  {
    public readonly DeleteMemberBinder Action;

    public DeleteBindingInfo(
      DeleteMemberBinder action,
      DynamicMetaObject[] args,
      ConditionalBuilder body,
      ValidationInfo validation)
      : base(args, body, validation)
    {
      this.Action = action;
    }
  }

  private class SetBindingInfo : MetaUserObject.MemberBindingInfo
  {
    public readonly SetMemberBinder Action;

    public SetBindingInfo(
      SetMemberBinder action,
      DynamicMetaObject[] args,
      ConditionalBuilder body,
      ValidationInfo validation)
      : base(args, body, validation)
    {
      this.Action = action;
    }
  }

  private class GetBindingInfo : MetaUserObject.MemberBindingInfo
  {
    public readonly DynamicMetaObjectBinder Action;
    public readonly ParameterExpression Self;
    public readonly ParameterExpression Result;

    public GetBindingInfo(
      DynamicMetaObjectBinder action,
      DynamicMetaObject[] args,
      ParameterExpression self,
      ParameterExpression result,
      ConditionalBuilder body,
      ValidationInfo validationInfo)
      : base(args, body, validationInfo)
    {
      this.Action = action;
      this.Self = self;
      this.Result = result;
    }
  }
}
