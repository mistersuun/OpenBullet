// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.DefaultBinder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class DefaultBinder : ActionBinder
{
  internal static readonly DefaultBinder Instance = new DefaultBinder();

  public DynamicMetaObject ConvertTo(Type toType, ConversionResultKind kind, DynamicMetaObject arg)
  {
    return this.ConvertTo(toType, kind, arg, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this));
  }

  public DynamicMetaObject ConvertTo(
    Type toType,
    ConversionResultKind kind,
    DynamicMetaObject arg,
    OverloadResolverFactory resolverFactory)
  {
    return this.ConvertTo(toType, kind, arg, resolverFactory, (DynamicMetaObject) null);
  }

  public DynamicMetaObject ConvertTo(
    Type toType,
    ConversionResultKind kind,
    DynamicMetaObject arg,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject errorSuggestion)
  {
    ContractUtils.RequiresNotNull((object) toType, nameof (toType));
    ContractUtils.RequiresNotNull((object) arg, nameof (arg));
    BindingRestrictions restrictions = arg.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(arg.Expression, arg.GetLimitType()));
    DynamicMetaObject dynamicMetaObject = DefaultBinder.TryConvertToObject(toType, arg.Expression.Type, arg, restrictions) ?? this.TryAllConversions(resolverFactory, toType, kind, arg.Expression.Type, restrictions, arg) ?? this.TryAllConversions(resolverFactory, toType, kind, arg.GetLimitType(), restrictions, arg) ?? errorSuggestion ?? this.MakeErrorTarget(toType, kind, restrictions, arg);
    if ((kind == ConversionResultKind.ExplicitTry || kind == ConversionResultKind.ImplicitTry) && toType.IsValueType())
      dynamicMetaObject = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (object)), dynamicMetaObject.Restrictions);
    return dynamicMetaObject;
  }

  private static DynamicMetaObject TryConvertToObject(
    Type toType,
    Type knownType,
    DynamicMetaObject arg,
    BindingRestrictions restrictions)
  {
    if (!(toType == typeof (object)))
      return (DynamicMetaObject) null;
    return knownType.IsValueType() ? DefaultBinder.MakeBoxingTarget(arg, restrictions) : new DynamicMetaObject(arg.Expression, restrictions);
  }

  private DynamicMetaObject TryAllConversions(
    OverloadResolverFactory factory,
    Type toType,
    ConversionResultKind kind,
    Type knownType,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return DefaultBinder.TryAssignableConversion(toType, knownType, restrictions, arg) ?? DefaultBinder.TryExtensibleConversion(toType, knownType, restrictions, arg) ?? this.TryUserDefinedConversion(kind, toType, knownType, restrictions, arg) ?? DefaultBinder.TryImplicitNumericConversion(toType, knownType, restrictions, arg) ?? this.TryNullableConversion(factory, toType, kind, knownType, restrictions, arg) ?? DefaultBinder.TryNullConversion(toType, knownType, restrictions);
  }

  private static DynamicMetaObject TryAssignableConversion(
    Type toType,
    Type type,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return toType.IsAssignableFrom(type) || type == typeof (DynamicNull) && (toType.IsClass() || toType.IsInterface()) ? DefaultBinder.MakeSimpleConversionTarget(toType, restrictions, arg) : (DynamicMetaObject) null;
  }

  private DynamicMetaObject TryUserDefinedConversion(
    ConversionResultKind kind,
    Type toType,
    Type type,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    Type underlyingType = DefaultBinder.GetUnderlyingType(type);
    DynamicMetaObject dynamicMetaObject = this.TryOneConversion(kind, toType, type, underlyingType, "op_Implicit", true, restrictions, arg) ?? this.TryOneConversion(kind, toType, type, underlyingType, "ConvertTo" + toType.Name, true, restrictions, arg);
    if (kind == ConversionResultKind.ExplicitCast || kind == ConversionResultKind.ExplicitTry)
      dynamicMetaObject = dynamicMetaObject ?? this.TryOneConversion(kind, toType, type, underlyingType, "op_Explicit", false, restrictions, arg) ?? this.TryOneConversion(kind, toType, type, underlyingType, "ConvertTo" + toType.Name, false, restrictions, arg);
    return dynamicMetaObject;
  }

  private DynamicMetaObject TryOneConversion(
    ConversionResultKind kind,
    Type toType,
    Type type,
    Type fromType,
    string methodName,
    bool isImplicit,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    MemberGroup member1 = this.GetMember(MemberRequestKind.Convert, fromType, methodName);
    DynamicMetaObject dynamicMetaObject = DefaultBinder.TryUserDefinedConversion(kind, toType, type, member1, isImplicit, restrictions, arg);
    if (dynamicMetaObject != null)
      return dynamicMetaObject;
    MemberGroup member2 = this.GetMember(MemberRequestKind.Convert, toType, methodName);
    return DefaultBinder.TryUserDefinedConversion(kind, toType, type, member2, isImplicit, restrictions, arg);
  }

  private static DynamicMetaObject TryUserDefinedConversion(
    ConversionResultKind kind,
    Type toType,
    Type type,
    MemberGroup conversions,
    bool isImplicit,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    Type underlyingType = DefaultBinder.GetUnderlyingType(type);
    foreach (MemberTracker conversion in conversions)
    {
      if (conversion.MemberType == TrackerTypes.Method)
      {
        MethodTracker method = (MethodTracker) conversion;
        if ((!isImplicit || !method.Method.IsDefined(typeof (ExplicitConversionMethodAttribute), true)) && method.Method.ReturnType == toType)
        {
          ParameterInfo[] parameters = method.Method.GetParameters();
          if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(underlyingType))
            return type == underlyingType ? DefaultBinder.MakeConversionTarget(kind, method, type, isImplicit, restrictions, arg) : DefaultBinder.MakeExtensibleConversionTarget(kind, method, type, isImplicit, restrictions, arg);
        }
      }
    }
    return (DynamicMetaObject) null;
  }

  private static DynamicMetaObject TryExtensibleConversion(
    Type toType,
    Type type,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    Type extensibleType = typeof (Extensible<>).MakeGenericType(toType);
    return extensibleType.IsAssignableFrom(type) ? DefaultBinder.MakeExtensibleTarget(extensibleType, restrictions, arg) : (DynamicMetaObject) null;
  }

  private static DynamicMetaObject TryImplicitNumericConversion(
    Type toType,
    Type type,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    Type type1 = type;
    if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Extensible<>))
      type1 = type.GetGenericArguments()[0];
    int x1;
    int y1;
    int x2;
    int y2;
    if (!toType.IsNumeric() || !type1.IsNumeric() || !TypeUtils.GetNumericConversionOrder(toType.GetTypeCode(), out x1, out y1) || !TypeUtils.GetNumericConversionOrder(type1.GetTypeCode(), out x2, out y2) || !TypeUtils.IsImplicitlyConvertible(x2, y2, x1, y1))
      return (DynamicMetaObject) null;
    return type == type1 ? DefaultBinder.MakeSimpleConversionTarget(toType, restrictions, arg) : DefaultBinder.MakeSimpleExtensibleConversionTarget(toType, restrictions, arg);
  }

  private DynamicMetaObject TryNullableConversion(
    OverloadResolverFactory factory,
    Type toType,
    ConversionResultKind kind,
    Type knownType,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    if (toType.IsGenericType() && toType.GetGenericTypeDefinition() == typeof (Nullable<>))
    {
      if (knownType == typeof (DynamicNull))
        return DefaultBinder.MakeNullToNullableOfTTarget(toType, restrictions);
      if (knownType == toType.GetGenericArguments()[0])
        return DefaultBinder.MakeTToNullableOfTTarget(toType, knownType, restrictions, arg);
      if (kind != ConversionResultKind.ExplicitCast && kind != ConversionResultKind.ExplicitTry)
        return (DynamicMetaObject) null;
      if (knownType != typeof (object))
        return this.MakeConvertingToTToNullableOfTTarget(factory, toType, kind, restrictions, arg);
    }
    return (DynamicMetaObject) null;
  }

  private static DynamicMetaObject TryNullConversion(
    Type toType,
    Type knownType,
    BindingRestrictions restrictions)
  {
    return knownType == typeof (DynamicNull) && !toType.IsValueType() ? DefaultBinder.MakeNullTarget(toType, restrictions) : (DynamicMetaObject) null;
  }

  private DynamicMetaObject MakeErrorTarget(
    Type toType,
    ConversionResultKind kind,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    switch (kind)
    {
      case ConversionResultKind.ImplicitCast:
      case ConversionResultKind.ExplicitCast:
        return DefaultBinder.MakeError(this.MakeConversionError(toType, arg.Expression), restrictions, toType);
      case ConversionResultKind.ImplicitTry:
      case ConversionResultKind.ExplicitTry:
        return new DynamicMetaObject(DefaultBinder.GetTryConvertReturnValue(toType), restrictions);
      default:
        throw new InvalidOperationException(kind.ToString());
    }
  }

  private static DynamicMetaObject MakeBoxingTarget(
    DynamicMetaObject arg,
    BindingRestrictions restrictions)
  {
    return DefaultBinder.MakeSimpleConversionTarget(typeof (object), restrictions, arg);
  }

  private static DynamicMetaObject MakeConversionTarget(
    ConversionResultKind kind,
    MethodTracker method,
    Type fromType,
    bool isImplicit,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    Expression expression = Microsoft.Scripting.Ast.Utils.Convert(arg.Expression, fromType);
    return DefaultBinder.MakeConversionTargetWorker(kind, method, isImplicit, restrictions, expression);
  }

  private static DynamicMetaObject MakeExtensibleConversionTarget(
    ConversionResultKind kind,
    MethodTracker method,
    Type fromType,
    bool isImplicit,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return DefaultBinder.MakeConversionTargetWorker(kind, method, isImplicit, restrictions, DefaultBinder.GetExtensibleValue(fromType, arg));
  }

  private static DynamicMetaObject MakeConversionTargetWorker(
    ConversionResultKind kind,
    MethodTracker method,
    bool isImplicit,
    BindingRestrictions restrictions,
    Expression param)
  {
    return new DynamicMetaObject(DefaultBinder.WrapForThrowingTry(kind, (isImplicit ? 1 : 0) != 0, (Expression) Microsoft.Scripting.Ast.Utils.SimpleCallHelper(method.Method, param), method.Method.ReturnType), restrictions);
  }

  private static Expression WrapForThrowingTry(
    ConversionResultKind kind,
    bool isImplicit,
    Expression ret,
    Type retType)
  {
    if (!isImplicit && kind == ConversionResultKind.ExplicitTry)
    {
      Expression convertReturnValue = DefaultBinder.GetTryConvertReturnValue(retType);
      ParameterExpression left = Expression.Variable(convertReturnValue.Type == typeof (object) ? typeof (object) : ret.Type, "tmp");
      ret = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        left
      }, (Expression) Microsoft.Scripting.Ast.Utils.Try((Expression) Expression.Assign((Expression) left, Microsoft.Scripting.Ast.Utils.Convert(ret, left.Type))).Catch(typeof (Exception), (Expression) Expression.Assign((Expression) left, convertReturnValue)), (Expression) left);
    }
    return ret;
  }

  private static DynamicMetaObject MakeSimpleConversionTarget(
    Type toType,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(arg.Expression, CompilerHelpers.GetVisibleType(toType)), restrictions);
  }

  private static DynamicMetaObject MakeSimpleExtensibleConversionTarget(
    Type toType,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(DefaultBinder.GetExtensibleValue(typeof (Extensible<>).MakeGenericType(toType), arg), toType), restrictions);
  }

  private static DynamicMetaObject MakeExtensibleTarget(
    Type extensibleType,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return new DynamicMetaObject((Expression) Expression.Property((Expression) Expression.Convert(arg.Expression, extensibleType), extensibleType.GetInheritedProperties("Value").First<PropertyInfo>()), restrictions);
  }

  private static DynamicMetaObject MakeNullToNullableOfTTarget(
    Type toType,
    BindingRestrictions restrictions)
  {
    return new DynamicMetaObject((Expression) Expression.Call(typeof (ScriptingRuntimeHelpers).GetMethod("CreateInstance").MakeGenericMethod(toType)), restrictions);
  }

  private static DynamicMetaObject MakeTToNullableOfTTarget(
    Type toType,
    Type knownType,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    return new DynamicMetaObject((Expression) Expression.New(toType.GetConstructor(new Type[1]
    {
      knownType
    }), Microsoft.Scripting.Ast.Utils.Convert(arg.Expression, knownType)), restrictions);
  }

  private DynamicMetaObject MakeConvertingToTToNullableOfTTarget(
    OverloadResolverFactory resolverFactory,
    Type toType,
    ConversionResultKind kind,
    BindingRestrictions restrictions,
    DynamicMetaObject arg)
  {
    Type genericArgument = toType.GetGenericArguments()[0];
    if (kind == ConversionResultKind.ExplicitCast)
    {
      Expression expression = this.ConvertExpression(arg.Expression, genericArgument, kind, resolverFactory);
      return new DynamicMetaObject((Expression) Expression.New(toType.GetConstructor(new Type[1]
      {
        genericArgument
      }), expression), restrictions);
    }
    Expression right = this.ConvertExpression(arg.Expression, genericArgument, kind, resolverFactory);
    ParameterExpression left = Expression.Variable(typeof (object), "tmp");
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) left, right), Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) Expression.New(toType.GetConstructor(new Type[1]
    {
      genericArgument
    }), (Expression) Expression.Convert((Expression) left, genericArgument)), DefaultBinder.GetTryConvertReturnValue(toType))), restrictions);
  }

  public static Expression GetTryConvertReturnValue(Type type)
  {
    return type.IsInterface() || type.IsClass() ? (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, type) : Microsoft.Scripting.Ast.Utils.Constant((object) null);
  }

  private static Expression GetExtensibleValue(Type extType, DynamicMetaObject arg)
  {
    return (Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(arg.Expression, extType), extType.GetInheritedProperties("Value").First<PropertyInfo>());
  }

  private static Type GetUnderlyingType(Type fromType)
  {
    Type type = fromType;
    do
    {
      if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Extensible<>))
        fromType = type.GetGenericArguments()[0];
      type = type.GetBaseType();
    }
    while (type != (Type) null);
    return fromType;
  }

  private static DynamicMetaObject MakeNullTarget(Type toType, BindingRestrictions restrictions)
  {
    return new DynamicMetaObject((Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, toType), restrictions);
  }

  public override bool CanConvertFrom(
    Type fromType,
    Type toType,
    bool toNotNullable,
    NarrowingLevel level)
  {
    return toType.IsAssignableFrom(fromType);
  }

  public override Candidate PreferConvert(Type t1, Type t2) => Candidate.Ambiguous;

  public virtual ErrorInfo MakeUndeletableMemberError(Type type, string name)
  {
    return this.MakeReadOnlyMemberError(type, name);
  }

  public virtual ErrorInfo MakeNonPublicMemberGetError(
    OverloadResolverFactory resolverFactory,
    MemberTracker member,
    Type type,
    DynamicMetaObject instance)
  {
    switch (member.MemberType)
    {
      case TrackerTypes.Field:
        return ErrorInfo.FromValueNoError((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(Microsoft.Scripting.Ast.Utils.Constant((object) ((FieldTracker) member).Field), typeof (FieldInfo)), typeof (FieldInfo).GetMethod("GetValue"), Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, typeof (object))));
      case TrackerTypes.Property:
        return ErrorInfo.FromValueNoError(MemberTracker.FromMemberInfo((MemberInfo) ((PropertyTracker) member).GetGetMethod(true)).Call(resolverFactory, (ActionBinder) this, instance).Expression);
      default:
        throw new InvalidOperationException();
    }
  }

  public virtual ErrorInfo MakeReadOnlyMemberError(Type type, string name)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) name)));
  }

  public virtual ErrorInfo MakeEventValidation(
    MemberGroup members,
    DynamicMetaObject eventObject,
    DynamicMetaObject value,
    OverloadResolverFactory resolverFactory)
  {
    EventTracker member = (EventTracker) members[0];
    return ErrorInfo.FromValueNoError((Expression) Expression.Call(typeof (BinderOps).GetMethod("SetEvent"), Microsoft.Scripting.Ast.Utils.Constant((object) member), value.Expression));
  }

  public static DynamicMetaObject MakeError(ErrorInfo error, Type type)
  {
    return DefaultBinder.MakeError(error, BindingRestrictions.Empty, type);
  }

  public static DynamicMetaObject MakeError(
    ErrorInfo error,
    BindingRestrictions restrictions,
    Type type)
  {
    switch (error.Kind)
    {
      case ErrorInfoKind.Exception:
        return (DynamicMetaObject) new ErrorMetaObject(Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Throw(error.Expression), type), restrictions);
      case ErrorInfoKind.Error:
        return (DynamicMetaObject) new ErrorMetaObject(Microsoft.Scripting.Ast.Utils.Convert(error.Expression, type), restrictions);
      case ErrorInfoKind.Success:
        return (DynamicMetaObject) new ErrorMetaObject(Microsoft.Scripting.Ast.Utils.Convert(error.Expression, type), restrictions);
      default:
        throw new InvalidOperationException();
    }
  }

  private static Expression MakeAmbiguousMatchError(MemberGroup members)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (MemberTracker member in members)
    {
      if (stringBuilder.Length != 0)
        stringBuilder.Append(", ");
      stringBuilder.Append((object) member.MemberType);
      stringBuilder.Append(" : ");
      stringBuilder.Append((object) member);
    }
    return (Expression) Expression.Throw((Expression) Expression.New(typeof (AmbiguousMatchException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) stringBuilder.ToString())), typeof (object));
  }

  public TrackerTypes GetMemberType(MemberGroup members, out Expression error)
  {
    error = (Expression) null;
    TrackerTypes memberType = TrackerTypes.All;
    for (int index = 0; index < members.Count; ++index)
    {
      MemberTracker member = members[index];
      if (member.MemberType != memberType)
      {
        if (memberType != TrackerTypes.All)
        {
          error = DefaultBinder.MakeAmbiguousMatchError(members);
          return TrackerTypes.All;
        }
        memberType = member.MemberType;
      }
    }
    return memberType;
  }

  public MethodInfo GetMethod(Type type, string name)
  {
    MethodInfo method = DefaultBinder.GetSpecialNameMethod(type, name);
    if (method != (MethodInfo) null)
      return method;
    Type type1 = type;
    do
    {
      foreach (Type extensionType in (IEnumerable<Type>) this.GetExtensionTypes(type1))
      {
        MethodInfo specialNameMethod = DefaultBinder.GetSpecialNameMethod(extensionType, name);
        if (specialNameMethod != (MethodInfo) null)
          method = !(method != (MethodInfo) null) ? specialNameMethod : throw DefaultBinder.AmbiguousMatch(type, name);
      }
      if (method != (MethodInfo) null)
        return method;
      type1 = type1.GetBaseType();
    }
    while (type1 != (Type) null);
    return (MethodInfo) null;
  }

  private static MethodInfo GetSpecialNameMethod(Type type, string name)
  {
    MethodInfo specialNameMethod = (MethodInfo) null;
    foreach (MethodInfo withBindingFlag in type.GetInheritedMethods(name).WithBindingFlags(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
    {
      if (withBindingFlag.IsSpecialName)
        specialNameMethod = (object) specialNameMethod == null ? withBindingFlag : throw DefaultBinder.AmbiguousMatch(type, name);
    }
    return specialNameMethod;
  }

  private static Exception AmbiguousMatch(Type type, string name)
  {
    throw new AmbiguousMatchException($"Found multiple SpecialName methods for {name} on type {type}");
  }

  public DynamicMetaObject DeleteMember(string name, DynamicMetaObject target)
  {
    return this.DeleteMember(name, target, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this));
  }

  public DynamicMetaObject DeleteMember(
    string name,
    DynamicMetaObject target,
    OverloadResolverFactory resolutionFactory)
  {
    return this.DeleteMember(name, target, resolutionFactory, (DynamicMetaObject) null);
  }

  public DynamicMetaObject DeleteMember(
    string name,
    DynamicMetaObject target,
    OverloadResolverFactory resolutionFactory,
    DynamicMetaObject errorSuggestion)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    return this.MakeDeleteMemberTarget(new DefaultBinder.SetOrDeleteMemberInfo(name, resolutionFactory), target.Restrict(target.GetLimitType()), errorSuggestion);
  }

  private DynamicMetaObject MakeDeleteMemberTarget(
    DefaultBinder.SetOrDeleteMemberInfo delInfo,
    DynamicMetaObject target,
    DynamicMetaObject errorSuggestion)
  {
    Type type = target.GetLimitType();
    BindingRestrictions bindingRestrictions = target.Restrictions;
    DynamicMetaObject dynamicMetaObject = target;
    if (typeof (TypeTracker).IsAssignableFrom(type))
    {
      bindingRestrictions = bindingRestrictions.Merge(BindingRestrictions.GetInstanceRestriction(target.Expression, target.Value));
      type = ((TypeTracker) target.Value).Type;
      dynamicMetaObject = (DynamicMetaObject) null;
    }
    delInfo.Body.Restrictions = bindingRestrictions;
    if (dynamicMetaObject == null || !this.MakeOperatorDeleteMemberBody(delInfo, dynamicMetaObject, type, "DeleteMember"))
    {
      MemberGroup member = this.GetMember(MemberRequestKind.Delete, type, delInfo.Name);
      if (member.Count != 0)
      {
        if (member[0].MemberType == TrackerTypes.Property)
        {
          MethodInfo deleteMethod = ((PropertyTracker) member[0]).GetDeleteMethod(this.PrivateBinding);
          if (deleteMethod != (MethodInfo) null)
          {
            this.MakePropertyDeleteStatement(delInfo, dynamicMetaObject, deleteMethod);
            return delInfo.Body.GetMetaObject(target);
          }
        }
        delInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeUndeletableMemberError(DefaultBinder.GetDeclaringMemberType(member), delInfo.Name), typeof (void)));
      }
      else
        delInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeMissingMemberErrorForDelete(type, dynamicMetaObject, delInfo.Name), typeof (void)));
    }
    return delInfo.Body.GetMetaObject(target);
  }

  private static Type GetDeclaringMemberType(MemberGroup group)
  {
    Type declaringMemberType = typeof (object);
    foreach (MemberTracker memberTracker in group)
    {
      if (declaringMemberType.IsAssignableFrom(memberTracker.DeclaringType))
        declaringMemberType = memberTracker.DeclaringType;
    }
    return declaringMemberType;
  }

  private void MakePropertyDeleteStatement(
    DefaultBinder.SetOrDeleteMemberInfo delInfo,
    DynamicMetaObject instance,
    MethodInfo delete)
  {
    ConditionalBuilder body1 = delInfo.Body;
    DynamicMetaObject body2;
    if (instance != null)
      body2 = this.MakeCallExpression(delInfo.ResolutionFactory, delete, instance);
    else
      body2 = this.MakeCallExpression(delInfo.ResolutionFactory, delete);
    body1.FinishCondition(body2);
  }

  private bool MakeOperatorDeleteMemberBody(
    DefaultBinder.SetOrDeleteMemberInfo delInfo,
    DynamicMetaObject instance,
    Type type,
    string name)
  {
    MethodInfo method = this.GetMethod(type, name);
    if (!(method != (MethodInfo) null))
      return false;
    DynamicMetaObject body = this.MakeCallExpression(delInfo.ResolutionFactory, method, instance, new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) delInfo.Name), BindingRestrictions.Empty, (object) delInfo.Name));
    if (method.ReturnType == typeof (bool))
      delInfo.Body.AddCondition(body.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) null));
    else
      delInfo.Body.FinishCondition(body);
    return method.ReturnType != typeof (bool);
  }

  public DynamicMetaObject GetMember(string name, DynamicMetaObject target)
  {
    return this.GetMember(name, target, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this), false, (DynamicMetaObject) null);
  }

  public DynamicMetaObject GetMember(
    string name,
    DynamicMetaObject target,
    OverloadResolverFactory resolverFactory)
  {
    return this.GetMember(name, target, resolverFactory, false, (DynamicMetaObject) null);
  }

  public DynamicMetaObject GetMember(
    string name,
    DynamicMetaObject target,
    OverloadResolverFactory resolverFactory,
    bool isNoThrow,
    DynamicMetaObject errorSuggestion)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    ContractUtils.RequiresNotNull((object) resolverFactory, nameof (resolverFactory));
    return this.MakeGetMemberTarget(new DefaultBinder.GetMemberInfo(name, resolverFactory, isNoThrow, errorSuggestion), target);
  }

  public DynamicMetaObject GetMember(
    string name,
    DynamicMetaObject target,
    bool isNoThrow,
    DynamicMetaObject errorSuggestion)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    return this.MakeGetMemberTarget(new DefaultBinder.GetMemberInfo(name, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this), isNoThrow, errorSuggestion), target);
  }

  private DynamicMetaObject MakeGetMemberTarget(
    DefaultBinder.GetMemberInfo getMemInfo,
    DynamicMetaObject target)
  {
    Type type = target.GetLimitType();
    BindingRestrictions bindingRestrictions = target.Restrictions;
    DynamicMetaObject self = target;
    target = target.Restrict(target.GetLimitType());
    MemberGroup members = MemberGroup.EmptyGroup;
    if (typeof (TypeTracker).IsAssignableFrom(type))
    {
      bindingRestrictions = bindingRestrictions.Merge(BindingRestrictions.GetInstanceRestriction(target.Expression, target.Value));
      if (!(target.Value is TypeGroup typeGroup) || typeGroup.TryGetNonGenericType(out Type _))
      {
        members = this.GetMember(MemberRequestKind.Get, ((TypeTracker) target.Value).Type, getMemInfo.Name);
        if (members.Count > 0)
        {
          type = ((TypeTracker) target.Value).Type;
          self = (DynamicMetaObject) null;
        }
      }
    }
    if (members.Count == 0)
      members = this.GetMember(MemberRequestKind.Get, type, getMemInfo.Name);
    if (members.Count == 0)
    {
      if (typeof (TypeTracker).IsAssignableFrom(type))
        DefaultBinder.EnsureTrackerRepresentsNonGenericType((TypeTracker) target.Value);
      else if (type.IsInterface())
      {
        type = typeof (object);
        members = this.GetMember(MemberRequestKind.Get, type, getMemInfo.Name);
      }
    }
    DynamicMetaObject propSelf = self;
    if (members.Count == 0 && typeof (IStrongBox).IsAssignableFrom(type) && propSelf != null)
    {
      propSelf = new DynamicMetaObject((Expression) Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(propSelf.Expression, type), type.GetInheritedFields("Value").First<FieldInfo>()), propSelf.Restrictions, ((IStrongBox) propSelf.Value).Value);
      type = type.GetGenericArguments()[0];
      members = this.GetMember(MemberRequestKind.Get, type, getMemInfo.Name);
    }
    this.MakeBodyHelper(getMemInfo, self, propSelf, type, members);
    getMemInfo.Body.Restrictions = bindingRestrictions;
    return getMemInfo.Body.GetMetaObject(target);
  }

  private static Type EnsureTrackerRepresentsNonGenericType(TypeTracker tracker) => tracker.Type;

  private void MakeBodyHelper(
    DefaultBinder.GetMemberInfo getMemInfo,
    DynamicMetaObject self,
    DynamicMetaObject propSelf,
    Type targetType,
    MemberGroup members)
  {
    if (self != null)
      this.MakeOperatorGetMemberBody(getMemInfo, propSelf, targetType, "GetCustomMember");
    Expression error;
    TrackerTypes memberType = this.GetMemberType(members, out error);
    if (error == null)
      this.MakeSuccessfulMemberAccess(getMemInfo, self, propSelf, targetType, members, memberType);
    else
      getMemInfo.Body.FinishError(getMemInfo.ErrorSuggestion?.Expression ?? error);
  }

  private void MakeSuccessfulMemberAccess(
    DefaultBinder.GetMemberInfo getMemInfo,
    DynamicMetaObject self,
    DynamicMetaObject propSelf,
    Type selfType,
    MemberGroup members,
    TrackerTypes memberType)
  {
    switch (memberType)
    {
      case TrackerTypes.Constructor:
      case TrackerTypes.Event:
      case TrackerTypes.Field:
      case TrackerTypes.Property:
      case TrackerTypes.Custom:
        this.MakeGenericBody(getMemInfo, selfType, members, propSelf);
        break;
      case TrackerTypes.Method:
        this.MakeGenericBodyWorker(getMemInfo, selfType, (MemberTracker) ReflectionCache.GetMethodGroup(getMemInfo.Name, members), self);
        break;
      case TrackerTypes.Type:
      case TrackerTypes.TypeGroup:
        this.MakeTypeBody(getMemInfo, selfType, members);
        break;
      case TrackerTypes.All:
        if (self != null)
          this.MakeOperatorGetMemberBody(getMemInfo, propSelf, selfType, "GetBoundMember");
        this.MakeMissingMemberRuleForGet(getMemInfo, self, selfType);
        break;
      default:
        throw new InvalidOperationException(memberType.ToString());
    }
  }

  private void MakeGenericBody(
    DefaultBinder.GetMemberInfo getMemInfo,
    Type instanceType,
    MemberGroup members,
    DynamicMetaObject instance)
  {
    MemberTracker member1 = members[0];
    if (members.Count > 1)
    {
      Type declaringType = members[0].DeclaringType;
      for (int index = 1; index < members.Count; ++index)
      {
        MemberTracker member2 = members[index];
        if (DefaultBinder.IsTrackerApplicableForType(instanceType, member2) && (members[index].DeclaringType.IsSubclassOf(declaringType) || !DefaultBinder.IsTrackerApplicableForType(instanceType, member1)))
        {
          member1 = members[index];
          declaringType = members[index].DeclaringType;
        }
      }
    }
    this.MakeGenericBodyWorker(getMemInfo, instanceType, member1, instance);
  }

  private static bool IsTrackerApplicableForType(Type type, MemberTracker mt)
  {
    return mt.DeclaringType == type || type.IsSubclassOf(mt.DeclaringType);
  }

  private void MakeTypeBody(
    DefaultBinder.GetMemberInfo getMemInfo,
    Type instanceType,
    MemberGroup members)
  {
    TypeTracker existingTypeEntity = (TypeTracker) members[0];
    for (int index = 1; index < members.Count; ++index)
      existingTypeEntity = TypeGroup.UpdateTypeEntity(existingTypeEntity, (TypeTracker) members[index]);
    getMemInfo.Body.FinishCondition(existingTypeEntity.GetValue(getMemInfo.ResolutionFactory, (ActionBinder) this, instanceType));
  }

  private void MakeGenericBodyWorker(
    DefaultBinder.GetMemberInfo getMemInfo,
    Type instanceType,
    MemberTracker tracker,
    DynamicMetaObject instance)
  {
    if (instance != null)
      tracker = tracker.BindToInstance(instance);
    DynamicMetaObject body = tracker.GetValue(getMemInfo.ResolutionFactory, (ActionBinder) this, instanceType);
    if (body != null)
    {
      getMemInfo.Body.FinishCondition(body);
    }
    else
    {
      ErrorInfo error = tracker.GetError((ActionBinder) this, instanceType);
      if (error.Kind != ErrorInfoKind.Success && getMemInfo.IsNoThrow)
        getMemInfo.Body.FinishError((Expression) DefaultBinder.MakeOperationFailed());
      else
        getMemInfo.Body.FinishError(DefaultBinder.MakeError(error, typeof (object)));
    }
  }

  private void MakeOperatorGetMemberBody(
    DefaultBinder.GetMemberInfo getMemInfo,
    DynamicMetaObject instance,
    Type instanceType,
    string name)
  {
    MethodInfo method = this.GetMethod(instanceType, name);
    if (!(method != (MethodInfo) null))
      return;
    ParameterExpression parameterExpression = Expression.Variable(typeof (object), "getVal");
    getMemInfo.Body.AddVariable(parameterExpression);
    getMemInfo.Body.AddCondition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, this.MakeCallExpression(getMemInfo.ResolutionFactory, method, new DynamicMetaObject((Expression) Expression.Convert(instance.Expression, instanceType), instance.Restrictions, instance.Value), new DynamicMetaObject((Expression) Expression.Constant((object) getMemInfo.Name), BindingRestrictions.Empty, (object) getMemInfo.Name)).Expression), (Expression) Expression.Field((Expression) null, typeof (OperationFailed).GetDeclaredField("Value"))), (Expression) parameterExpression);
  }

  private void MakeMissingMemberRuleForGet(
    DefaultBinder.GetMemberInfo getMemInfo,
    DynamicMetaObject self,
    Type type)
  {
    if (getMemInfo.ErrorSuggestion != null)
      getMemInfo.Body.FinishError(getMemInfo.ErrorSuggestion.Expression);
    else if (getMemInfo.IsNoThrow)
      getMemInfo.Body.FinishError((Expression) DefaultBinder.MakeOperationFailed());
    else
      getMemInfo.Body.FinishError(DefaultBinder.MakeError(this.MakeMissingMemberError(type, self, getMemInfo.Name), typeof (object)));
  }

  private static MemberExpression MakeOperationFailed()
  {
    return Expression.Field((Expression) null, typeof (OperationFailed).GetDeclaredField("Value"));
  }

  public DynamicMetaObject Call(
    CallSignature signature,
    DynamicMetaObject target,
    params DynamicMetaObject[] args)
  {
    return this.Call(signature, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this), target, args);
  }

  public DynamicMetaObject Call(
    CallSignature signature,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject target,
    params DynamicMetaObject[] args)
  {
    return this.Call(signature, (DynamicMetaObject) null, resolverFactory, target, args);
  }

  public DynamicMetaObject Call(
    CallSignature signature,
    DynamicMetaObject errorSuggestion,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject target,
    params DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>((IList<DynamicMetaObject>) args, nameof (args));
    ContractUtils.RequiresNotNull((object) resolverFactory, nameof (resolverFactory));
    DefaultBinder.TargetInfo targetInfo = this.GetTargetInfo(target, args);
    if (targetInfo == null)
      return errorSuggestion ?? this.MakeCannotCallRule(target, target.GetLimitType());
    DynamicMetaObject dynamicMetaObject = this.MakeMetaMethodCall(signature, resolverFactory, targetInfo);
    if (dynamicMetaObject.Expression.Type.IsValueType())
      dynamicMetaObject = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (object)), dynamicMetaObject.Restrictions);
    return dynamicMetaObject;
  }

  private DynamicMetaObject MakeMetaMethodCall(
    CallSignature signature,
    OverloadResolverFactory resolverFactory,
    DefaultBinder.TargetInfo targetInfo)
  {
    BindingRestrictions restrictions = BindingRestrictions.Combine((IList<DynamicMetaObject>) targetInfo.Arguments).Merge(targetInfo.Restrictions);
    if (targetInfo.Instance != null)
      restrictions = targetInfo.Instance.Restrictions.Merge(restrictions);
    DynamicMetaObject[] args;
    CallTypes callType;
    if (targetInfo.Instance != null)
    {
      args = ArrayUtils.Insert<DynamicMetaObject>(targetInfo.Instance, targetInfo.Arguments);
      callType = CallTypes.ImplicitInstance;
    }
    else
    {
      args = targetInfo.Arguments;
      callType = CallTypes.None;
    }
    return this.CallMethod(resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) args, signature, callType), (IList<MethodBase>) targetInfo.Targets, restrictions);
  }

  private DefaultBinder.TargetInfo GetTargetInfo(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    object obj = target.Value;
    return DefaultBinder.TryGetDelegateTargets(target, args, obj as Delegate) ?? DefaultBinder.TryGetMemberGroupTargets(target, args, obj as MemberGroup) ?? DefaultBinder.TryGetMethodGroupTargets(target, args, obj as MethodGroup) ?? this.TryGetBoundMemberTargets(target, args, obj as BoundMemberTracker) ?? this.TryGetOperatorTargets(target, args, (object) target);
  }

  private static DefaultBinder.TargetInfo TryGetMethodGroupTargets(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    MethodGroup mthgrp)
  {
    if (mthgrp == null)
      return (DefaultBinder.TargetInfo) null;
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MethodTracker method in (IEnumerable<MethodTracker>) mthgrp.Methods)
      methodBaseList.Add((MethodBase) method.Method);
    return new DefaultBinder.TargetInfo((DynamicMetaObject) null, ArrayUtils.Insert<DynamicMetaObject>(target, args), BindingRestrictions.GetInstanceRestriction(target.Expression, (object) mthgrp), methodBaseList.ToArray());
  }

  private static DefaultBinder.TargetInfo TryGetMemberGroupTargets(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    MemberGroup mg)
  {
    if (mg == null)
      return (DefaultBinder.TargetInfo) null;
    List<MethodInfo> methodInfoList = new List<MethodInfo>();
    foreach (MemberTracker memberTracker in mg)
    {
      if (memberTracker.MemberType == TrackerTypes.Method)
        methodInfoList.Add(((MethodTracker) memberTracker).Method);
    }
    MethodBase[] array = (MethodBase[]) methodInfoList.ToArray();
    return new DefaultBinder.TargetInfo((DynamicMetaObject) null, ArrayUtils.Insert<DynamicMetaObject>(target, args), array);
  }

  private DefaultBinder.TargetInfo TryGetBoundMemberTargets(
    DynamicMetaObject self,
    DynamicMetaObject[] args,
    BoundMemberTracker bmt)
  {
    if (bmt == null)
      return (DefaultBinder.TargetInfo) null;
    DynamicMetaObject instance = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert((Expression) Expression.Property((Expression) Expression.Convert(self.Expression, typeof (BoundMemberTracker)), typeof (BoundMemberTracker).GetDeclaredProperty("ObjectInstance")), bmt.BoundTo.DeclaringType), self.Restrictions).Restrict(CompilerHelpers.GetType(bmt.ObjectInstance));
    BindingRestrictions expressionRestriction = BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Convert(self.Expression, typeof (BoundMemberTracker)), typeof (BoundMemberTracker).GetDeclaredProperty("BoundTo")), Microsoft.Scripting.Ast.Utils.Constant((object) bmt.BoundTo)));
    MethodBase[] methodBaseArray;
    switch (bmt.BoundTo.MemberType)
    {
      case TrackerTypes.Method:
        methodBaseArray = new MethodBase[1]
        {
          (MethodBase) ((MethodTracker) bmt.BoundTo).Method
        };
        break;
      case TrackerTypes.MethodGroup:
        methodBaseArray = ((MethodGroup) bmt.BoundTo).GetMethodBases();
        break;
      default:
        throw new InvalidOperationException();
    }
    return new DefaultBinder.TargetInfo(instance, args, expressionRestriction, methodBaseArray);
  }

  private static DefaultBinder.TargetInfo TryGetDelegateTargets(
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    Delegate d)
  {
    if ((object) d == null)
      return (DefaultBinder.TargetInfo) null;
    return new DefaultBinder.TargetInfo(target, args, new MethodBase[1]
    {
      (MethodBase) d.GetType().GetMethod("Invoke")
    });
  }

  private DefaultBinder.TargetInfo TryGetOperatorTargets(
    DynamicMetaObject self,
    DynamicMetaObject[] args,
    object target)
  {
    MemberGroup member = this.GetMember(MemberRequestKind.Invoke, CompilerHelpers.GetType(target), "Call");
    List<MethodBase> methodBaseList = new List<MethodBase>();
    foreach (MemberTracker memberTracker in member)
    {
      if (memberTracker.MemberType == TrackerTypes.Method)
      {
        MethodInfo method = ((MethodTracker) memberTracker).Method;
        if (method.IsSpecialName)
          methodBaseList.Add((MethodBase) method);
      }
    }
    return methodBaseList.Count > 0 ? new DefaultBinder.TargetInfo((DynamicMetaObject) null, ArrayUtils.Insert<DynamicMetaObject>(self, args), methodBaseList.ToArray()) : (DefaultBinder.TargetInfo) null;
  }

  private DynamicMetaObject MakeCannotCallRule(DynamicMetaObject self, Type type)
  {
    return DefaultBinder.MakeError(ErrorInfo.FromException((Expression) Expression.New(typeof (ArgumentTypeException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) (this.GetTypeName(type) + " is not callable")))), self.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, type)), typeof (object));
  }

  public DynamicMetaObject CallMethod(DefaultOverloadResolver resolver, IList<MethodBase> targets)
  {
    return this.CallMethod(resolver, targets, BindingRestrictions.Empty, (string) null);
  }

  public DynamicMetaObject CallMethod(
    DefaultOverloadResolver resolver,
    IList<MethodBase> targets,
    string name)
  {
    return this.CallMethod(resolver, targets, BindingRestrictions.Empty, name);
  }

  public DynamicMetaObject CallMethod(
    DefaultOverloadResolver resolver,
    IList<MethodBase> targets,
    BindingRestrictions restrictions)
  {
    return this.CallMethod(resolver, targets, restrictions, (string) null);
  }

  public DynamicMetaObject CallMethod(
    DefaultOverloadResolver resolver,
    IList<MethodBase> targets,
    BindingRestrictions restrictions,
    string name)
  {
    return this.CallMethod(resolver, targets, restrictions, name, NarrowingLevel.None, NarrowingLevel.All, out BindingTarget _);
  }

  public DynamicMetaObject CallMethod(
    DefaultOverloadResolver resolver,
    IList<MethodBase> targets,
    BindingRestrictions restrictions,
    string name,
    NarrowingLevel minLevel,
    NarrowingLevel maxLevel,
    out BindingTarget target)
  {
    ContractUtils.RequiresNotNull((object) resolver, nameof (resolver));
    ContractUtils.RequiresNotNullItems<MethodBase>(targets, nameof (targets));
    ContractUtils.RequiresNotNull((object) restrictions, nameof (restrictions));
    target = resolver.ResolveOverload(name ?? DefaultBinder.GetTargetName(targets), targets, minLevel, maxLevel);
    return target.Success ? new DynamicMetaObject(target.MakeExpression(), restrictions.Merge(DefaultBinder.MakeSplatTests(resolver.CallType, resolver.Signature, resolver.Arguments).Merge(target.RestrictedArguments.GetAllRestrictions()))) : this.MakeInvalidParametersRule(resolver, restrictions, target);
  }

  internal static string GetTargetName(IList<MethodBase> targets)
  {
    return !targets[0].IsConstructor ? targets[0].Name : targets[0].DeclaringType.Name;
  }

  private DynamicMetaObject MakeInvalidParametersRule(
    DefaultOverloadResolver binder,
    BindingRestrictions restrictions,
    BindingTarget bt)
  {
    IList<DynamicMetaObject> arguments = binder.Arguments;
    BindingRestrictions restrictions1 = DefaultBinder.MakeSplatTests(binder.CallType, binder.Signature, true, arguments);
    for (int index = 0; index < arguments.Count; ++index)
      arguments[index] = arguments[index].Restrict(arguments[index].GetLimitType());
    return DefaultBinder.MakeError(binder.MakeInvalidParametersError(bt), restrictions.Merge(BindingRestrictions.Combine(arguments).Merge(restrictions1)), typeof (object));
  }

  private static BindingRestrictions MakeSplatTests(
    CallTypes callType,
    CallSignature signature,
    IList<DynamicMetaObject> args)
  {
    return DefaultBinder.MakeSplatTests(callType, signature, false, args);
  }

  private static BindingRestrictions MakeSplatTests(
    CallTypes callType,
    CallSignature signature,
    bool testTypes,
    IList<DynamicMetaObject> args)
  {
    BindingRestrictions bindingRestrictions = BindingRestrictions.Empty;
    if (signature.HasListArgument())
      bindingRestrictions = DefaultBinder.MakeParamsArrayTest(callType, signature, testTypes, args);
    if (signature.HasDictionaryArgument())
      bindingRestrictions = bindingRestrictions.Merge(DefaultBinder.MakeParamsDictionaryTest(args, testTypes));
    return bindingRestrictions;
  }

  private static BindingRestrictions MakeParamsArrayTest(
    CallTypes callType,
    CallSignature signature,
    bool testTypes,
    IList<DynamicMetaObject> args)
  {
    int index = signature.IndexOf(ArgumentType.List);
    if (callType == CallTypes.ImplicitInstance)
      ++index;
    return DefaultBinder.MakeParamsTest(args[index], testTypes);
  }

  private static BindingRestrictions MakeParamsTest(DynamicMetaObject splattee, bool testTypes)
  {
    if (!(splattee.Value is IList<object> objectList))
      return splattee.Value == null ? BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal(splattee.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) null))) : BindingRestrictions.GetTypeRestriction(splattee.Expression, splattee.Value.GetType());
    BindingRestrictions bindingRestrictions = BindingRestrictions.GetExpressionRestriction((Expression) Expression.AndAlso((Expression) Expression.TypeIs(splattee.Expression, typeof (IList<object>)), (Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Convert(splattee.Expression, typeof (IList<object>)), typeof (ICollection<object>).GetDeclaredProperty("Count")), Microsoft.Scripting.Ast.Utils.Constant((object) objectList.Count))));
    if (testTypes)
    {
      for (int index = 0; index < objectList.Count; ++index)
        bindingRestrictions = bindingRestrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(splattee.Expression, typeof (IList<object>)), typeof (IList<object>).GetMethod("get_Item"), Microsoft.Scripting.Ast.Utils.Constant((object) index)), CompilerHelpers.GetType(objectList[index])));
    }
    return bindingRestrictions;
  }

  private static BindingRestrictions MakeParamsDictionaryTest(
    IList<DynamicMetaObject> args,
    bool testTypes)
  {
    IDictionary dictionary = (IDictionary) args[args.Count - 1].Value;
    IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
    string[] strArray = new string[dictionary.Count];
    Type[] typeArray = testTypes ? new Type[dictionary.Count] : (Type[]) null;
    int index = 0;
    while (enumerator.MoveNext())
    {
      if (!(enumerator.Entry.Key is string key))
        throw ScriptingRuntimeHelpers.SimpleTypeError($"expected string for dictionary argument got {enumerator.Entry.Key}");
      strArray[index] = key;
      if (typeArray != null)
        typeArray[index] = CompilerHelpers.GetType(enumerator.Entry.Value);
      ++index;
    }
    return BindingRestrictions.GetExpressionRestriction((Expression) Expression.AndAlso((Expression) Expression.TypeIs(args[args.Count - 1].Expression, typeof (IDictionary)), (Expression) Expression.Call(typeof (BinderOps).GetMethod("CheckDictionaryMembers"), (Expression) Expression.Convert(args[args.Count - 1].Expression, typeof (IDictionary)), Microsoft.Scripting.Ast.Utils.Constant((object) strArray), testTypes ? Microsoft.Scripting.Ast.Utils.Constant((object) typeArray) : (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, typeof (Type[])))));
  }

  [Obsolete("You should use the overload which takes ExpressionType instead")]
  public DynamicMetaObject DoOperation(string operation, params DynamicMetaObject[] args)
  {
    return this.DoOperation(operation, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this), args);
  }

  [Obsolete("You should use the overload which takes ExpressionType instead")]
  public DynamicMetaObject DoOperation(
    string operation,
    OverloadResolverFactory resolverFactory,
    params DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) operation, nameof (operation));
    ContractUtils.RequiresNotNull((object) resolverFactory, nameof (resolverFactory));
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>((IList<DynamicMetaObject>) args, nameof (args));
    return this.MakeGeneralOperatorRule(operation, resolverFactory, args);
  }

  public DynamicMetaObject DoOperation(ExpressionType operation, params DynamicMetaObject[] args)
  {
    return this.DoOperation(operation, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this), args);
  }

  public DynamicMetaObject DoOperation(
    ExpressionType operation,
    OverloadResolverFactory resolverFactory,
    params DynamicMetaObject[] args)
  {
    ContractUtils.RequiresNotNull((object) resolverFactory, nameof (resolverFactory));
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>((IList<DynamicMetaObject>) args, nameof (args));
    return this.MakeGeneralOperatorRule(operation, resolverFactory, args);
  }

  public DynamicMetaObject GetIndex(DynamicMetaObject[] args)
  {
    return this.GetIndex((OverloadResolverFactory) new DefaultOverloadResolverFactory(this), args);
  }

  public DynamicMetaObject GetIndex(
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    return args[0].GetLimitType().IsArray ? this.MakeArrayIndexRule(resolverFactory, DefaultBinder.IndexType.Get, args) : this.MakeMethodIndexRule(DefaultBinder.IndexType.Get, resolverFactory, args);
  }

  public DynamicMetaObject SetIndex(DynamicMetaObject[] args)
  {
    return this.SetIndex((OverloadResolverFactory) new DefaultOverloadResolverFactory(this), args);
  }

  public DynamicMetaObject SetIndex(
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    return args[0].LimitType.IsArray ? this.MakeArrayIndexRule(resolverFactory, DefaultBinder.IndexType.Set, args) : this.MakeMethodIndexRule(DefaultBinder.IndexType.Set, resolverFactory, args);
  }

  public DynamicMetaObject GetDocumentation(DynamicMetaObject target)
  {
    BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType);
    DocumentationAttribute customAttribute = CustomAttributeExtensions.GetCustomAttribute<DocumentationAttribute>((MemberInfo) target.LimitType);
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant(customAttribute != null ? (object) customAttribute.Documentation : (object) string.Empty), typeRestriction);
  }

  public DynamicMetaObject GetMemberNames(DynamicMetaObject target)
  {
    BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType);
    HashSet<string> stringSet = new HashSet<string>();
    TypeTracker.GetMemberNames(target.LimitType, stringSet);
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) stringSet.ToArray<string>()), typeRestriction);
  }

  public DynamicMetaObject GetCallSignatures(DynamicMetaObject target)
  {
    return DefaultBinder.MakeCallSignatureResult(CompilerHelpers.GetMethodTargets((object) target.LimitType), target);
  }

  public DynamicMetaObject GetIsCallable(DynamicMetaObject target)
  {
    BindingRestrictions typeRestriction = BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType);
    bool flag = false;
    if (typeof (Delegate).IsAssignableFrom(target.LimitType) || typeof (MethodGroup).IsAssignableFrom(target.LimitType))
      flag = true;
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) flag), typeRestriction);
  }

  private DynamicMetaObject MakeGeneralOperatorRule(
    string operation,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    OperatorInfo operatorInfo = OperatorInfo.GetOperatorInfo(operation);
    return this.MakeGeneratorOperatorRule(resolverFactory, args, operatorInfo);
  }

  private DynamicMetaObject MakeGeneralOperatorRule(
    ExpressionType operation,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    OperatorInfo operatorInfo = OperatorInfo.GetOperatorInfo(operation);
    return this.MakeGeneratorOperatorRule(resolverFactory, args, operatorInfo);
  }

  private DynamicMetaObject MakeGeneratorOperatorRule(
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args,
    OperatorInfo info)
  {
    return !CompilerHelpers.IsComparisonOperator(info.Operator) ? this.MakeOperatorRule(info, resolverFactory, args) : this.MakeComparisonRule(info, resolverFactory, args);
  }

  private DynamicMetaObject MakeComparisonRule(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    return this.TryComparisonMethod(info, resolverFactory, args[0], args) ?? this.TryComparisonMethod(info, resolverFactory, args[0], args) ?? this.TryNumericComparison(info, resolverFactory, args) ?? this.TryInvertedComparison(info, resolverFactory, args[0], args) ?? this.TryInvertedComparison(info, resolverFactory, args[0], args) ?? DefaultBinder.TryNullComparisonRule(args) ?? DefaultBinder.TryPrimitiveCompare(info, args) ?? DefaultBinder.MakeOperatorError(info, args);
  }

  private DynamicMetaObject TryComparisonMethod(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    MethodInfo[] applicableMembers = this.GetApplicableMembers(target.GetLimitType(), info);
    return applicableMembers.Length != 0 ? this.TryMakeBindingTarget(resolverFactory, applicableMembers, args, BindingRestrictions.Empty) : (DynamicMetaObject) null;
  }

  private static DynamicMetaObject MakeOperatorError(OperatorInfo info, DynamicMetaObject[] args)
  {
    return new DynamicMetaObject((Expression) Expression.Throw(Microsoft.Scripting.Ast.Utils.ComplexCallHelper(typeof (BinderOps).GetMethod("BadArgumentsForOperation"), ArrayUtils.Insert<Expression>(Microsoft.Scripting.Ast.Utils.Constant((object) info.Operator), DynamicUtils.GetExpressions(args)))), BindingRestrictions.Combine((IList<DynamicMetaObject>) args));
  }

  private DynamicMetaObject TryNumericComparison(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    MethodInfo[] methods = DefaultBinder.FilterNonMethods(args[0].GetLimitType(), this.GetMember(MemberRequestKind.Operation, args[0].GetLimitType(), "Compare"));
    if (methods.Length != 0)
    {
      BindingTarget bindingTarget = resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) args, new CallSignature(args.Length), CallTypes.None).ResolveOverload(methods[0].Name, (IList<MethodBase>) methods, NarrowingLevel.None, NarrowingLevel.All);
      if (bindingTarget.Success)
      {
        Expression expression = Microsoft.Scripting.Ast.Utils.Convert(bindingTarget.MakeExpression(), typeof (int));
        switch (info.Operator)
        {
          case ExpressionType.Equal:
            expression = (Expression) Expression.Equal(expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
            break;
          case ExpressionType.GreaterThan:
            expression = (Expression) Expression.GreaterThan(expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
            break;
          case ExpressionType.GreaterThanOrEqual:
            expression = (Expression) Expression.GreaterThanOrEqual(expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
            break;
          case ExpressionType.LessThan:
            expression = (Expression) Expression.LessThan(expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
            break;
          case ExpressionType.LessThanOrEqual:
            expression = (Expression) Expression.LessThanOrEqual(expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
            break;
          case ExpressionType.NotEqual:
            expression = (Expression) Expression.NotEqual(expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
            break;
        }
        return new DynamicMetaObject(expression, bindingTarget.RestrictedArguments.GetAllRestrictions());
      }
    }
    return (DynamicMetaObject) null;
  }

  private DynamicMetaObject TryInvertedComparison(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    OperatorInfo operatorInfo = OperatorInfo.GetOperatorInfo(DefaultBinder.GetInvertedOperator(info.Operator));
    MethodBase[] applicableMembers = (MethodBase[]) this.GetApplicableMembers(target.GetLimitType(), operatorInfo);
    return applicableMembers.Length != 0 ? this.TryMakeInvertedBindingTarget(resolverFactory, applicableMembers, args) : (DynamicMetaObject) null;
  }

  private static DynamicMetaObject TryNullComparisonRule(DynamicMetaObject[] args)
  {
    Type limitType = args[1].GetLimitType();
    BindingRestrictions restrictions = BindingRestrictionsHelpers.GetRuntimeTypeRestriction(args[0].Expression, args[0].GetLimitType()).Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args));
    if (args[0].GetLimitType() == typeof (DynamicNull))
    {
      if (!limitType.IsValueType())
        return new DynamicMetaObject((Expression) Expression.Equal(args[0].Expression, Microsoft.Scripting.Ast.Utils.Constant((object) null)), restrictions);
      if (limitType.GetGenericTypeDefinition() == typeof (Nullable<>))
        return new DynamicMetaObject((Expression) Expression.Property(args[0].Expression, limitType.GetDeclaredProperty("HasValue")), restrictions);
    }
    else if (limitType == typeof (DynamicNull))
    {
      if (!args[0].GetLimitType().IsValueType())
        return new DynamicMetaObject((Expression) Expression.Equal(args[0].Expression, Microsoft.Scripting.Ast.Utils.Constant((object) null)), restrictions);
      if (args[0].GetLimitType().GetGenericTypeDefinition() == typeof (Nullable<>))
        return new DynamicMetaObject((Expression) Expression.Property(args[0].Expression, limitType.GetDeclaredProperty("HasValue")), restrictions);
    }
    return (DynamicMetaObject) null;
  }

  private static DynamicMetaObject TryPrimitiveCompare(OperatorInfo info, DynamicMetaObject[] args)
  {
    if (!(args[0].GetLimitType().GetNonNullableType() == args[1].GetLimitType().GetNonNullableType()) || !args[0].GetLimitType().IsNumeric())
      return (DynamicMetaObject) null;
    Expression expression1 = args[0].Expression;
    Expression expression2 = args[1].Expression;
    Expression expression3;
    switch (info.Operator)
    {
      case ExpressionType.Equal:
        expression3 = (Expression) Expression.Equal(expression1, expression2);
        break;
      case ExpressionType.GreaterThan:
        expression3 = (Expression) Expression.GreaterThan(expression1, expression2);
        break;
      case ExpressionType.GreaterThanOrEqual:
        expression3 = (Expression) Expression.GreaterThanOrEqual(expression1, expression2);
        break;
      case ExpressionType.LessThan:
        expression3 = (Expression) Expression.LessThan(expression1, expression2);
        break;
      case ExpressionType.LessThanOrEqual:
        expression3 = (Expression) Expression.LessThanOrEqual(expression1, expression2);
        break;
      case ExpressionType.NotEqual:
        expression3 = (Expression) Expression.NotEqual(expression1, expression2);
        break;
      default:
        throw new InvalidOperationException();
    }
    return new DynamicMetaObject(expression3, BindingRestrictionsHelpers.GetRuntimeTypeRestriction(expression1, args[0].GetLimitType()).Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(expression2, args[0].GetLimitType())).Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args)));
  }

  private DynamicMetaObject MakeOperatorRule(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    return this.TryForwardOperator(info, resolverFactory, args) ?? this.TryReverseOperator(info, resolverFactory, args) ?? DefaultBinder.TryPrimitiveOperator(info, args) ?? DefaultBinder.TryMakeDefaultUnaryRule(info, args) ?? DefaultBinder.MakeOperatorError(info, args);
  }

  private static DynamicMetaObject TryPrimitiveOperator(OperatorInfo info, DynamicMetaObject[] args)
  {
    if (args.Length != 2 || !(args[0].GetLimitType().GetNonNullableType() == args[1].GetLimitType().GetNonNullableType()) || !args[0].GetLimitType().IsArithmetic())
      return (DynamicMetaObject) null;
    DynamicMetaObject dynamicMetaObject1 = args[0].Restrict(args[0].GetLimitType());
    DynamicMetaObject dynamicMetaObject2 = args[1].Restrict(args[0].GetLimitType());
    Expression expression;
    switch (info.Operator)
    {
      case ExpressionType.Add:
        expression = (Expression) Expression.Add(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.And:
        expression = (Expression) Expression.And(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.Divide:
        expression = (Expression) Expression.Divide(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.ExclusiveOr:
        expression = (Expression) Expression.ExclusiveOr(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.LeftShift:
        expression = (Expression) Expression.LeftShift(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.Modulo:
        expression = (Expression) Expression.Modulo(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.Multiply:
        expression = (Expression) Expression.Multiply(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.Or:
        expression = (Expression) Expression.Or(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.RightShift:
        expression = (Expression) Expression.RightShift(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      case ExpressionType.Subtract:
        expression = (Expression) Expression.Subtract(dynamicMetaObject1.Expression, dynamicMetaObject2.Expression);
        break;
      default:
        throw new InvalidOperationException();
    }
    return new DynamicMetaObject(expression, dynamicMetaObject1.Restrictions.Merge(dynamicMetaObject2.Restrictions));
  }

  private DynamicMetaObject TryForwardOperator(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    MethodInfo[] applicableMembers = this.GetApplicableMembers(args[0].GetLimitType(), info);
    BindingRestrictions empty = BindingRestrictions.Empty;
    return applicableMembers.Length != 0 ? this.TryMakeBindingTarget(resolverFactory, applicableMembers, args, empty) : (DynamicMetaObject) null;
  }

  private DynamicMetaObject TryReverseOperator(
    OperatorInfo info,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    if (args.Length != 0)
    {
      MethodInfo[] applicableMembers = this.GetApplicableMembers(args[0].LimitType, info);
      if (applicableMembers.Length != 0)
        return this.TryMakeBindingTarget(resolverFactory, applicableMembers, args, BindingRestrictions.Empty);
    }
    return (DynamicMetaObject) null;
  }

  private static DynamicMetaObject TryMakeDefaultUnaryRule(
    OperatorInfo info,
    DynamicMetaObject[] args)
  {
    if (args.Length == 1)
    {
      BindingRestrictions restrictions = BindingRestrictionsHelpers.GetRuntimeTypeRestriction(args[0].Expression, args[0].GetLimitType()).Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args));
      switch (info.Operator)
      {
        case ExpressionType.Negate:
          if (args[0].GetLimitType().IsArithmetic())
            return new DynamicMetaObject((Expression) Expression.Negate(args[0].Expression), restrictions);
          break;
        case ExpressionType.Not:
          if (args[0].GetLimitType().IsIntegerOrBool())
            return new DynamicMetaObject((Expression) Expression.Not(args[0].Expression), restrictions);
          break;
        case ExpressionType.IsTrue:
          if (args[0].GetLimitType() == typeof (bool))
            return args[0];
          break;
      }
    }
    return (DynamicMetaObject) null;
  }

  private static DynamicMetaObject MakeCallSignatureResult(
    MethodBase[] methods,
    DynamicMetaObject target)
  {
    List<string> stringList = new List<string>();
    if (methods != null)
    {
      foreach (MethodBase method in methods)
      {
        StringBuilder stringBuilder = new StringBuilder();
        string str = "";
        foreach (ParameterInfo parameter in method.GetParameters())
        {
          stringBuilder.Append(str);
          stringBuilder.Append(parameter.ParameterType.Name);
          stringBuilder.Append(" ");
          stringBuilder.Append(parameter.Name);
          str = ", ";
        }
        stringList.Add(stringBuilder.ToString());
      }
    }
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) stringList.ToArray()), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(target.Expression, target.GetLimitType()).Merge(target.Restrictions));
  }

  private static Type GetArgType(DynamicMetaObject[] args, int index)
  {
    return !args[index].HasValue ? args[index].Expression.Type : args[index].GetLimitType();
  }

  private DynamicMetaObject MakeMethodIndexRule(
    DefaultBinder.IndexType oper,
    OverloadResolverFactory resolverFactory,
    DynamicMetaObject[] args)
  {
    MethodInfo[] methodsFromDefaults = this.GetMethodsFromDefaults((IEnumerable<MemberInfo>) args[0].GetLimitType().GetDefaultMembers(), oper);
    if (methodsFromDefaults.Length == 0)
      return (DynamicMetaObject) null;
    DynamicMetaObject[] args1 = args;
    ParameterExpression left = (ParameterExpression) null;
    if (oper == DefaultBinder.IndexType.Set)
    {
      left = Expression.Variable(args[2].Expression.Type, "arg2Temp");
      args[2] = new DynamicMetaObject((Expression) Expression.Assign((Expression) left, args[2].Expression), args[2].Restrictions);
    }
    BindingRestrictions restrictions = BindingRestrictions.Combine((IList<DynamicMetaObject>) args);
    DefaultOverloadResolver overloadResolver = resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) args1, new CallSignature(args1.Length), CallTypes.ImplicitInstance);
    BindingTarget target = overloadResolver.ResolveOverload(oper == DefaultBinder.IndexType.Get ? "get_Item" : "set_Item", (IList<MethodBase>) methodsFromDefaults, NarrowingLevel.None, NarrowingLevel.All);
    if (!target.Success)
      return DefaultBinder.MakeError(overloadResolver.MakeInvalidParametersError(target), restrictions, typeof (object));
    if (oper == DefaultBinder.IndexType.Get)
      return new DynamicMetaObject(target.MakeExpression(), restrictions.Merge(target.RestrictedArguments.GetAllRestrictions()));
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, target.MakeExpression(), (Expression) left), restrictions.Merge(target.RestrictedArguments.GetAllRestrictions()));
  }

  private DynamicMetaObject MakeArrayIndexRule(
    OverloadResolverFactory factory,
    DefaultBinder.IndexType oper,
    DynamicMetaObject[] args)
  {
    if (!this.CanConvertFrom(DefaultBinder.GetArgType(args, 1), typeof (int), false, NarrowingLevel.All))
      return (DynamicMetaObject) null;
    BindingRestrictions restrictions = BindingRestrictionsHelpers.GetRuntimeTypeRestriction(args[0].Expression, args[0].GetLimitType()).Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args));
    return oper == DefaultBinder.IndexType.Get ? new DynamicMetaObject((Expression) Expression.ArrayAccess(args[0].Expression, this.ConvertIfNeeded(factory, args[1].Expression, typeof (int))), restrictions) : new DynamicMetaObject((Expression) Expression.Assign((Expression) Expression.ArrayAccess(args[0].Expression, this.ConvertIfNeeded(factory, args[1].Expression, typeof (int))), this.ConvertIfNeeded(factory, args[2].Expression, args[0].GetLimitType().GetElementType())), restrictions.Merge(args[1].Restrictions));
  }

  private MethodInfo[] GetMethodsFromDefaults(
    IEnumerable<MemberInfo> defaults,
    DefaultBinder.IndexType op)
  {
    List<MethodInfo> methodInfoList = new List<MethodInfo>();
    foreach (MemberInfo memberInfo in defaults)
    {
      PropertyInfo propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != (PropertyInfo) null)
      {
        switch (op)
        {
          case DefaultBinder.IndexType.Get:
            MethodInfo getMethod = propertyInfo.GetGetMethod(this.PrivateBinding);
            if (getMethod != (MethodInfo) null)
            {
              methodInfoList.Add(getMethod);
              continue;
            }
            continue;
          case DefaultBinder.IndexType.Set:
            MethodInfo setMethod = propertyInfo.GetSetMethod(this.PrivateBinding);
            if (setMethod != (MethodInfo) null)
            {
              methodInfoList.Add(setMethod);
              continue;
            }
            continue;
          default:
            continue;
        }
      }
    }
    Dictionary<MethodSignatureInfo, MethodInfo> dictionary = new Dictionary<MethodSignatureInfo, MethodInfo>();
    foreach (MethodInfo info in methodInfoList)
    {
      MethodSignatureInfo key = new MethodSignatureInfo(info);
      MethodInfo methodInfo;
      if (dictionary.TryGetValue(key, out methodInfo))
      {
        if (methodInfo.DeclaringType.IsAssignableFrom(info.DeclaringType))
          dictionary[key] = info;
      }
      else
        dictionary[key] = info;
    }
    return new List<MethodInfo>((IEnumerable<MethodInfo>) dictionary.Values).ToArray();
  }

  private DynamicMetaObject TryMakeBindingTarget(
    OverloadResolverFactory resolverFactory,
    MethodInfo[] targets,
    DynamicMetaObject[] args,
    BindingRestrictions restrictions)
  {
    BindingTarget bindingTarget = resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) args, new CallSignature(args.Length), CallTypes.None).ResolveOverload(targets[0].Name, (IList<MethodBase>) targets, NarrowingLevel.None, NarrowingLevel.All);
    return bindingTarget.Success ? new DynamicMetaObject(bindingTarget.MakeExpression(), restrictions.Merge(bindingTarget.RestrictedArguments.GetAllRestrictions())) : (DynamicMetaObject) null;
  }

  private DynamicMetaObject TryMakeInvertedBindingTarget(
    OverloadResolverFactory resolverFactory,
    MethodBase[] targets,
    DynamicMetaObject[] args)
  {
    BindingTarget bindingTarget = resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) args, new CallSignature(args.Length), CallTypes.None).ResolveOverload(targets[0].Name, (IList<MethodBase>) targets, NarrowingLevel.None, NarrowingLevel.All);
    return bindingTarget.Success ? new DynamicMetaObject((Expression) Expression.Not(bindingTarget.MakeExpression()), bindingTarget.RestrictedArguments.GetAllRestrictions()) : (DynamicMetaObject) null;
  }

  private static ExpressionType GetInvertedOperator(ExpressionType op)
  {
    switch (op)
    {
      case ExpressionType.Equal:
        return ExpressionType.NotEqual;
      case ExpressionType.GreaterThan:
        return ExpressionType.LessThanOrEqual;
      case ExpressionType.GreaterThanOrEqual:
        return ExpressionType.LessThan;
      case ExpressionType.LessThan:
        return ExpressionType.GreaterThanOrEqual;
      case ExpressionType.LessThanOrEqual:
        return ExpressionType.GreaterThan;
      case ExpressionType.NotEqual:
        return ExpressionType.Equal;
      default:
        throw new InvalidOperationException();
    }
  }

  private Expression ConvertIfNeeded(
    OverloadResolverFactory factory,
    Expression expression,
    Type type)
  {
    return expression.Type != type ? this.ConvertExpression(expression, type, ConversionResultKind.ExplicitCast, factory) : expression;
  }

  private MethodInfo[] GetApplicableMembers(Type t, OperatorInfo info)
  {
    MemberGroup member = this.GetMember(MemberRequestKind.Operation, t, info.Name);
    if (member.Count == 0 && info.AlternateName != null)
      member = this.GetMember(MemberRequestKind.Operation, t, info.AlternateName);
    return DefaultBinder.FilterNonMethods(t, member);
  }

  private static MethodInfo[] FilterNonMethods(Type t, MemberGroup members)
  {
    List<MethodInfo> methodInfoList = new List<MethodInfo>(members.Count);
    foreach (MemberTracker member in members)
    {
      if (member.MemberType == TrackerTypes.Method)
      {
        MethodInfo method = ((MethodTracker) member).Method;
        if (method.DeclaringType != typeof (object) || t != typeof (DynamicNull))
          methodInfoList.Add(method);
      }
    }
    return methodInfoList.ToArray();
  }

  public DynamicMetaObject SetMember(
    string name,
    DynamicMetaObject target,
    DynamicMetaObject value)
  {
    return this.SetMember(name, target, value, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this));
  }

  public DynamicMetaObject SetMember(
    string name,
    DynamicMetaObject target,
    DynamicMetaObject value,
    OverloadResolverFactory resolverFactory)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    ContractUtils.RequiresNotNull((object) value, nameof (value));
    ContractUtils.RequiresNotNull((object) resolverFactory, nameof (resolverFactory));
    return this.MakeSetMemberTarget(new DefaultBinder.SetOrDeleteMemberInfo(name, resolverFactory), target, value, (DynamicMetaObject) null);
  }

  public DynamicMetaObject SetMember(
    string name,
    DynamicMetaObject target,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    return this.SetMember(name, target, value, errorSuggestion, (OverloadResolverFactory) new DefaultOverloadResolverFactory(this));
  }

  public DynamicMetaObject SetMember(
    string name,
    DynamicMetaObject target,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion,
    OverloadResolverFactory resolverFactory)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    ContractUtils.RequiresNotNull((object) value, nameof (value));
    ContractUtils.RequiresNotNull((object) resolverFactory, nameof (resolverFactory));
    return this.MakeSetMemberTarget(new DefaultBinder.SetOrDeleteMemberInfo(name, resolverFactory), target, value, errorSuggestion);
  }

  private DynamicMetaObject MakeSetMemberTarget(
    DefaultBinder.SetOrDeleteMemberInfo memInfo,
    DynamicMetaObject target,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    Type type = target.GetLimitType();
    DynamicMetaObject self = target;
    target = target.Restrict(target.GetLimitType());
    memInfo.Body.Restrictions = target.Restrictions;
    if (typeof (TypeTracker).IsAssignableFrom(type))
    {
      type = ((TypeTracker) target.Value).Type;
      self = (DynamicMetaObject) null;
      memInfo.Body.Restrictions = memInfo.Body.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(target.Expression, target.Value));
    }
    this.MakeSetMemberRule(memInfo, type, self, value, errorSuggestion);
    return memInfo.Body.GetMetaObject(target, value);
  }

  private void MakeSetMemberRule(
    DefaultBinder.SetOrDeleteMemberInfo memInfo,
    Type type,
    DynamicMetaObject self,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    if (this.MakeOperatorSetMemberBody(memInfo, self, value, type, "SetMember"))
      return;
    MemberGroup member = this.GetMember(MemberRequestKind.Set, type, memInfo.Name);
    if (self != null && member.Count == 0 && typeof (IStrongBox).IsAssignableFrom(type))
    {
      self = new DynamicMetaObject((Expression) Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(self.Expression, type), type.GetInheritedFields("Value").First<FieldInfo>()), BindingRestrictions.Empty, ((IStrongBox) self.Value).Value);
      type = type.GetGenericArguments()[0];
      member = this.GetMember(MemberRequestKind.Set, type, memInfo.Name);
    }
    Expression error;
    TrackerTypes memberType = this.GetMemberType(member, out error);
    if (error == null)
    {
      switch (memberType)
      {
        case TrackerTypes.Constructor:
        case TrackerTypes.Method:
        case TrackerTypes.Type:
        case TrackerTypes.TypeGroup:
          memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeReadOnlyMemberError(type, memInfo.Name), BindingRestrictions.Empty, typeof (object)));
          break;
        case TrackerTypes.Event:
          memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeEventValidation(member, self, value, memInfo.ResolutionFactory), BindingRestrictions.Empty, typeof (object)));
          break;
        case TrackerTypes.Field:
          this.MakeFieldRule(memInfo, self, value, type, member, errorSuggestion);
          break;
        case TrackerTypes.Property:
          this.MakePropertyRule(memInfo, self, value, type, member, errorSuggestion);
          break;
        case TrackerTypes.Custom:
          this.MakeGenericBody(memInfo, self, value, type, member[0], errorSuggestion);
          break;
        case TrackerTypes.All:
          if (this.MakeOperatorSetMemberBody(memInfo, self, value, type, "SetMemberAfter"))
            break;
          memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeMissingMemberErrorForAssign(type, self, memInfo.Name), BindingRestrictions.Empty, typeof (object)));
          break;
        default:
          throw new InvalidOperationException();
      }
    }
    else
      memInfo.Body.FinishError(error);
  }

  private void MakeGenericBody(
    DefaultBinder.SetOrDeleteMemberInfo memInfo,
    DynamicMetaObject instance,
    DynamicMetaObject target,
    Type instanceType,
    MemberTracker tracker,
    DynamicMetaObject errorSuggestion)
  {
    if (instance != null)
      tracker = tracker.BindToInstance(instance);
    DynamicMetaObject body = tracker.SetValue(memInfo.ResolutionFactory, (ActionBinder) this, instanceType, target, errorSuggestion);
    if (body != null)
      memInfo.Body.FinishCondition(body);
    else
      memInfo.Body.FinishError(DefaultBinder.MakeError(tracker.GetError((ActionBinder) this, instanceType), typeof (object)));
  }

  private void MakePropertyRule(
    DefaultBinder.SetOrDeleteMemberInfo memInfo,
    DynamicMetaObject instance,
    DynamicMetaObject target,
    Type targetType,
    MemberGroup properties,
    DynamicMetaObject errorSuggestion)
  {
    PropertyTracker property = (PropertyTracker) properties[0];
    MethodInfo methodInfo = property.GetSetMethod(true);
    if (methodInfo != (MethodInfo) null && !methodInfo.IsPublic && !methodInfo.IsProtected() && !this.PrivateBinding)
      methodInfo = (MethodInfo) null;
    if (methodInfo != (MethodInfo) null)
    {
      methodInfo = CompilerHelpers.TryGetCallableMethod(targetType, methodInfo);
      if (!this.PrivateBinding && !CompilerHelpers.IsVisible((MethodBase) methodInfo))
        methodInfo = (MethodInfo) null;
    }
    if (methodInfo != (MethodInfo) null)
    {
      if (property.IsStatic != (instance == null))
      {
        ConditionalBuilder body1 = memInfo.Body;
        DynamicMetaObject body2 = errorSuggestion;
        if (body2 == null)
          body2 = DefaultBinder.MakeError(this.MakeStaticPropertyInstanceAccessError(property, true, instance, target), typeof (object));
        body1.FinishError(body2);
      }
      else if (property.IsStatic && property.DeclaringType != targetType)
        memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeStaticAssignFromDerivedTypeError(targetType, instance, (MemberTracker) property, target, memInfo.ResolutionFactory), typeof (object)));
      else if (methodInfo.ContainsGenericParameters)
        memInfo.Body.FinishCondition(DefaultBinder.MakeGenericPropertyExpression(memInfo));
      else if (methodInfo.IsPublic && !methodInfo.DeclaringType.IsValueType())
      {
        if (instance == null)
          memInfo.Body.FinishCondition((Expression) Expression.Block((Expression) Microsoft.Scripting.Ast.Utils.SimpleCallHelper(methodInfo, this.ConvertExpression(target.Expression, methodInfo.GetParameters()[0].ParameterType, ConversionResultKind.ExplicitCast, memInfo.ResolutionFactory)), (Expression) Expression.Constant((object) null)));
        else
          memInfo.Body.FinishCondition(this.MakeReturnValue(this.MakeCallExpression(memInfo.ResolutionFactory, methodInfo, instance, target), target));
      }
      else
        memInfo.Body.FinishCondition(this.MakeReturnValue((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Constant((object) ((ReflectedPropertyTracker) property).Property), typeof (PropertyInfo).GetMethod("SetValue", new Type[3]
        {
          typeof (object),
          typeof (object),
          typeof (object[])
        }), instance == null ? Microsoft.Scripting.Ast.Utils.Constant((object) null) : Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(this.ConvertExpression(target.Expression, methodInfo.GetParameters()[0].ParameterType, ConversionResultKind.ExplicitCast, memInfo.ResolutionFactory), typeof (object)), (Expression) Expression.NewArrayInit(typeof (object))), target));
    }
    else
      memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeMissingMemberErrorForAssignReadOnlyProperty(targetType, instance, memInfo.Name), typeof (object)));
  }

  private void MakeFieldRule(
    DefaultBinder.SetOrDeleteMemberInfo memInfo,
    DynamicMetaObject instance,
    DynamicMetaObject target,
    Type targetType,
    MemberGroup fields,
    DynamicMetaObject errorSuggestion)
  {
    FieldTracker field = (FieldTracker) fields[0];
    if (instance != null && field.DeclaringType.IsGenericType() && field.DeclaringType.GetGenericTypeDefinition() == typeof (StrongBox<>))
    {
      Type[] genericArguments = field.DeclaringType.GetGenericArguments();
      memInfo.Body.FinishCondition(this.MakeReturnValue((Expression) Expression.Assign((Expression) Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, field.DeclaringType), field.DeclaringType.GetDeclaredField("Value")), Microsoft.Scripting.Ast.Utils.Convert(target.Expression, genericArguments[0])), target));
    }
    else if (field.IsInitOnly || field.IsLiteral)
      memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeReadOnlyMemberError(targetType, memInfo.Name), typeof (object)));
    else if (field.IsStatic && targetType != field.DeclaringType)
      memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeStaticAssignFromDerivedTypeError(targetType, instance, (MemberTracker) field, target, memInfo.ResolutionFactory), typeof (object)));
    else if (field.DeclaringType.IsValueType() && !field.IsStatic)
      memInfo.Body.FinishError(errorSuggestion ?? DefaultBinder.MakeError(this.MakeSetValueTypeFieldError(field, instance, target), typeof (object)));
    else if (field.IsPublic && field.DeclaringType.IsVisible())
    {
      if (!field.IsStatic && instance == null)
        memInfo.Body.FinishError((Expression) Expression.Throw((Expression) Expression.New(typeof (ArgumentException).GetConstructor(new Type[1]
        {
          typeof (string)
        }), Microsoft.Scripting.Ast.Utils.Constant((object) "assignment to instance field w/o instance")), typeof (object)));
      else
        memInfo.Body.FinishCondition(this.MakeReturnValue((Expression) Expression.Assign((Expression) Expression.Field(field.IsStatic ? (Expression) null : Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, field.DeclaringType), field.Field), this.ConvertExpression(target.Expression, field.FieldType, ConversionResultKind.ExplicitCast, memInfo.ResolutionFactory)), target));
    }
    else
      memInfo.Body.FinishCondition(this.MakeReturnValue((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(Microsoft.Scripting.Ast.Utils.Constant((object) field.Field), typeof (FieldInfo)), typeof (FieldInfo).GetMethod("SetValue", new Type[2]
      {
        typeof (object),
        typeof (object)
      }), field.IsStatic ? Microsoft.Scripting.Ast.Utils.Constant((object) null) : Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(target.Expression, typeof (object))), target));
  }

  private DynamicMetaObject MakeReturnValue(DynamicMetaObject expression, DynamicMetaObject target)
  {
    return new DynamicMetaObject((Expression) Expression.Block(expression.Expression, (Expression) Expression.Convert(target.Expression, typeof (object))), target.Restrictions.Merge(expression.Restrictions));
  }

  private Expression MakeReturnValue(Expression expression, DynamicMetaObject target)
  {
    return (Expression) Expression.Block(expression, (Expression) Expression.Convert(target.Expression, typeof (object)));
  }

  private bool MakeOperatorSetMemberBody(
    DefaultBinder.SetOrDeleteMemberInfo memInfo,
    DynamicMetaObject self,
    DynamicMetaObject target,
    Type type,
    string name)
  {
    if (self != null)
    {
      MethodInfo method = this.GetMethod(type, name);
      if (method != (MethodInfo) null)
      {
        ParameterExpression parameterExpression = Expression.Variable(target.Expression.Type, "setValue");
        memInfo.Body.AddVariable(parameterExpression);
        DynamicMetaObject dynamicMetaObject = this.MakeCallExpression(memInfo.ResolutionFactory, method, self.Clone(Microsoft.Scripting.Ast.Utils.Convert(self.Expression, type)), new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) memInfo.Name), BindingRestrictions.Empty, (object) memInfo.Name), target.Clone((Expression) parameterExpression));
        BlockExpression condition = Expression.Block((Expression) Expression.Assign((Expression) parameterExpression, target.Expression), dynamicMetaObject.Expression);
        if (method.ReturnType == typeof (bool))
          memInfo.Body.AddCondition((Expression) condition, (Expression) parameterExpression);
        else
          memInfo.Body.FinishCondition((Expression) Expression.Block((Expression) condition, Microsoft.Scripting.Ast.Utils.Convert((Expression) parameterExpression, typeof (object))));
        return method.ReturnType != typeof (bool);
      }
    }
    return false;
  }

  private static Expression MakeGenericPropertyExpression(
    DefaultBinder.SetOrDeleteMemberInfo memInfo)
  {
    return (Expression) Expression.New(typeof (MemberAccessException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) memInfo.Name));
  }

  private sealed class SetOrDeleteMemberInfo
  {
    public readonly string Name;
    public readonly OverloadResolverFactory ResolutionFactory;
    public readonly ConditionalBuilder Body = new ConditionalBuilder();

    public SetOrDeleteMemberInfo(string name, OverloadResolverFactory resolutionFactory)
    {
      this.Name = name;
      this.ResolutionFactory = resolutionFactory;
    }
  }

  private sealed class GetMemberInfo
  {
    public readonly string Name;
    public readonly OverloadResolverFactory ResolutionFactory;
    public readonly bool IsNoThrow;
    public readonly ConditionalBuilder Body = new ConditionalBuilder();
    public readonly DynamicMetaObject ErrorSuggestion;

    public GetMemberInfo(
      string name,
      OverloadResolverFactory resolutionFactory,
      bool noThrow,
      DynamicMetaObject errorSuggestion)
    {
      this.Name = name;
      this.ResolutionFactory = resolutionFactory;
      this.IsNoThrow = noThrow;
      this.ErrorSuggestion = errorSuggestion;
    }
  }

  private class TargetInfo
  {
    public readonly DynamicMetaObject Instance;
    public readonly DynamicMetaObject[] Arguments;
    public readonly MethodBase[] Targets;
    public readonly BindingRestrictions Restrictions;

    public TargetInfo(
      DynamicMetaObject instance,
      DynamicMetaObject[] arguments,
      params MethodBase[] args)
      : this(instance, arguments, BindingRestrictions.Empty, args)
    {
    }

    public TargetInfo(
      DynamicMetaObject instance,
      DynamicMetaObject[] arguments,
      BindingRestrictions restrictions,
      params MethodBase[] targets)
    {
      this.Instance = instance;
      this.Arguments = arguments;
      this.Targets = targets;
      this.Restrictions = restrictions;
    }
  }

  private enum IndexType
  {
    Get,
    Set,
  }
}
