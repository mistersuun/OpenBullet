// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
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
using System.Numerics;
using System.Reflection;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonBinder : Microsoft.Scripting.Actions.DefaultBinder
{
  private PythonContext _context;
  private PythonBinder.SlotCache _typeMembers = new PythonBinder.SlotCache();
  private PythonBinder.SlotCache _resolvedMembers = new PythonBinder.SlotCache();
  private Dictionary<Type, IList<Type>> _dlrExtensionTypes;
  private bool _registeredInterfaceExtensions;
  private static readonly Dictionary<Type, PythonBinder.ExtensionTypeInfo> _sysTypes = PythonBinder.MakeSystemTypes();

  public DynamicMetaObject Create(
    CallSignature signature,
    DynamicMetaObject target,
    DynamicMetaObject[] args,
    Expression contextExpression)
  {
    Type targetType = PythonBinder.GetTargetType(target.Value);
    if (!(targetType != (Type) null))
      return (DynamicMetaObject) null;
    return typeof (Delegate).IsAssignableFrom(targetType) && args.Length == 1 ? new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetDelegate"), contextExpression, Microsoft.Scripting.Ast.Utils.Convert(args[0].Expression, typeof (object)), (Expression) Expression.Constant((object) targetType)), target.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(target.Expression, target.Value))) : this.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(this, (IList<DynamicMetaObject>) args, signature, contextExpression), (IList<MethodBase>) PythonTypeOps.GetConstructors(targetType, this.PrivateBinding), target.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(target.Expression, target.Value)));
  }

  private static Type GetTargetType(object target)
  {
    return target is TypeTracker typeTracker ? typeTracker.Type : target as Type;
  }

  public PythonBinder(PythonContext pythonContext, CodeContext context)
  {
    ContractUtils.RequiresNotNull((object) pythonContext, nameof (pythonContext));
    this._dlrExtensionTypes = PythonBinder.MakeExtensionTypes();
    this._context = pythonContext;
    if (context == null)
      return;
    context.LanguageContext.DomainManager.AssemblyLoaded += new EventHandler<AssemblyLoadedEventArgs>(this.DomainManager_AssemblyLoaded);
    foreach (Assembly loadedAssembly in (IEnumerable<Assembly>) pythonContext.DomainManager.GetLoadedAssemblyList())
      this.DomainManager_AssemblyLoaded((object) this, new AssemblyLoadedEventArgs(loadedAssembly));
  }

  public PythonBinder(PythonBinder binder)
  {
    this._context = binder._context;
    this._typeMembers = binder._typeMembers;
    this._resolvedMembers = binder._resolvedMembers;
    this._dlrExtensionTypes = binder._dlrExtensionTypes;
    this._registeredInterfaceExtensions = binder._registeredInterfaceExtensions;
  }

  public override Expression ConvertExpression(
    Expression expr,
    Type toType,
    ConversionResultKind kind,
    OverloadResolverFactory factory)
  {
    ContractUtils.RequiresNotNull((object) expr, nameof (expr));
    ContractUtils.RequiresNotNull((object) toType, nameof (toType));
    Type type1 = expr.Type;
    if (toType == typeof (object))
      return type1.IsValueType() ? Microsoft.Scripting.Ast.Utils.Convert(expr, toType) : expr;
    if (toType.IsAssignableFrom(type1))
      return type1.IsValueType && !toType.IsValueType && expr.NodeType == ExpressionType.Unbox ? ((UnaryExpression) expr).Operand : expr;
    Type type2 = this.Context.Binder.PrivateBinding ? toType : CompilerHelpers.GetVisibleType(toType);
    return type1 == typeof (PythonType) && type2 == typeof (Type) ? Microsoft.Scripting.Ast.Utils.Convert(expr, type2) : Binders.Convert(((PythonOverloadResolverFactory) factory)._codeContext, this._context, type2, type2 == typeof (char) ? ConversionResultKind.ImplicitCast : kind, expr);
  }

  internal static MethodInfo GetGenericConvertMethod(Type toType)
  {
    if (!toType.IsValueType())
      return typeof (Converter).GetMethod("ConvertToReferenceType");
    return toType.IsGenericType() && toType.GetGenericTypeDefinition() == typeof (Nullable<>) ? typeof (Converter).GetMethod("ConvertToNullableType") : typeof (Converter).GetMethod("ConvertToValueType");
  }

  internal static MethodInfo GetFastConvertMethod(Type toType)
  {
    if (toType == typeof (char))
      return typeof (Converter).GetMethod("ConvertToChar");
    if (toType == typeof (int))
      return typeof (Converter).GetMethod("ConvertToInt32");
    if (toType == typeof (string))
      return typeof (Converter).GetMethod("ConvertToString");
    if (toType == typeof (long))
      return typeof (Converter).GetMethod("ConvertToInt64");
    if (toType == typeof (double))
      return typeof (Converter).GetMethod("ConvertToDouble");
    if (toType == typeof (bool))
      return typeof (Converter).GetMethod("ConvertToBoolean");
    if (toType == typeof (BigInteger))
      return typeof (Converter).GetMethod("ConvertToBigInteger");
    if (toType == typeof (Complex))
      return typeof (Converter).GetMethod("ConvertToComplex");
    if (toType == typeof (IEnumerable))
      return typeof (Converter).GetMethod("ConvertToIEnumerable");
    if (toType == typeof (float))
      return typeof (Converter).GetMethod("ConvertToSingle");
    if (toType == typeof (byte))
      return typeof (Converter).GetMethod("ConvertToByte");
    if (toType == typeof (sbyte))
      return typeof (Converter).GetMethod("ConvertToSByte");
    if (toType == typeof (short))
      return typeof (Converter).GetMethod("ConvertToInt16");
    if (toType == typeof (uint))
      return typeof (Converter).GetMethod("ConvertToUInt32");
    if (toType == typeof (ulong))
      return typeof (Converter).GetMethod("ConvertToUInt64");
    if (toType == typeof (ushort))
      return typeof (Converter).GetMethod("ConvertToUInt16");
    return toType == typeof (Type) ? typeof (Converter).GetMethod("ConvertToType") : (MethodInfo) null;
  }

  public override object Convert(object obj, Type toType) => Converter.Convert(obj, toType);

  public override bool CanConvertFrom(
    Type fromType,
    Type toType,
    bool toNotNullable,
    NarrowingLevel level)
  {
    return Converter.CanConvertFrom(fromType, toType, level);
  }

  public override Candidate PreferConvert(Type t1, Type t2) => Converter.PreferConvert(t1, t2);

  public override bool PrivateBinding => this._context.DomainManager.Configuration.PrivateBinding;

  public override ErrorInfo MakeSetValueTypeFieldError(
    FieldTracker field,
    DynamicMetaObject instance,
    DynamicMetaObject value)
  {
    return ErrorInfo.FromValueNoError((Expression) Expression.Block((Expression) Expression.Call(typeof (PythonOps).GetMethod("Warn"), (Expression) Expression.Constant((object) this._context.SharedContext), (Expression) Expression.Constant((object) PythonExceptions.RuntimeWarning), (Expression) Expression.Constant((object) "Setting field {0} on value type {1} may result in updating a copy.  Use {1}.{0}.SetValue(instance, value) if this is safe.  For more information help({1}.{0}.SetValue)."), (Expression) Expression.Constant((object) new object[2]
    {
      (object) field.Name,
      (object) field.DeclaringType.Name
    })), (Expression) Expression.Assign((Expression) Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(instance.Expression, field.DeclaringType), field.Field), this.ConvertExpression(value.Expression, field.FieldType, ConversionResultKind.ExplicitCast, (OverloadResolverFactory) new PythonOverloadResolverFactory(this, (Expression) Expression.Constant((object) this._context.SharedContext))))));
  }

  public override ErrorInfo MakeConversionError(Type toType, Expression value)
  {
    return ErrorInfo.FromException((Expression) Expression.Call(typeof (PythonOps).GetMethod("TypeErrorForTypeMismatch"), Microsoft.Scripting.Ast.Utils.Constant((object) DynamicHelpers.GetPythonTypeFromType(toType).Name), Microsoft.Scripting.Ast.Utils.Convert(value, typeof (object))));
  }

  public override ErrorInfo MakeNonPublicMemberGetError(
    OverloadResolverFactory resolverFactory,
    MemberTracker member,
    Type type,
    DynamicMetaObject instance)
  {
    return this.PrivateBinding ? base.MakeNonPublicMemberGetError(resolverFactory, member, type, instance) : ErrorInfo.FromValue(BindingHelpers.TypeErrorForProtectedMember(type, member.Name));
  }

  public override ErrorInfo MakeStaticAssignFromDerivedTypeError(
    Type accessingType,
    DynamicMetaObject instance,
    MemberTracker info,
    DynamicMetaObject assignedValue,
    OverloadResolverFactory factory)
  {
    return this.MakeMissingMemberError(accessingType, instance, info.Name);
  }

  public override ErrorInfo MakeStaticPropertyInstanceAccessError(
    PropertyTracker tracker,
    bool isAssignment,
    IList<DynamicMetaObject> parameters)
  {
    ContractUtils.RequiresNotNull((object) tracker, nameof (tracker));
    ContractUtils.RequiresNotNull((object) parameters, nameof (parameters));
    return isAssignment ? ErrorInfo.FromException((Expression) Expression.Call(typeof (PythonOps).GetMethod("StaticAssignmentFromInstanceError"), Microsoft.Scripting.Ast.Utils.Constant((object) tracker), Microsoft.Scripting.Ast.Utils.Constant((object) isAssignment))) : ErrorInfo.FromValue((Expression) Expression.Property((Expression) null, tracker.GetGetMethod(this.DomainManager.Configuration.PrivateBinding)));
  }

  public override string GetTypeName(Type t) => DynamicHelpers.GetPythonTypeFromType(t).Name;

  public override MemberGroup GetMember(MemberRequestKind actionKind, Type type, string name)
  {
    MemberGroup group;
    if (!this._resolvedMembers.TryGetCachedMember(type, name, actionKind == MemberRequestKind.Get, out group))
    {
      group = PythonTypeInfo.GetMemberAll(this, actionKind, type, name);
      this._resolvedMembers.CacheSlot(type, actionKind == MemberRequestKind.Get, name, PythonTypeOps.GetSlot(group, name, this.PrivateBinding), group);
    }
    return group ?? MemberGroup.EmptyGroup;
  }

  public override ErrorInfo MakeEventValidation(
    MemberGroup members,
    DynamicMetaObject eventObject,
    DynamicMetaObject value,
    OverloadResolverFactory factory)
  {
    EventTracker member = (EventTracker) members[0];
    return ErrorInfo.FromValueNoError((Expression) Expression.Block((Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTrySetValue"), ((PythonOverloadResolverFactory) factory)._codeContext, Microsoft.Scripting.Ast.Utils.Constant((object) PythonTypeOps.GetReflectedEvent(member)), eventObject != null ? Microsoft.Scripting.Ast.Utils.Convert(eventObject.Expression, typeof (object)) : Microsoft.Scripting.Ast.Utils.Constant((object) null), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, typeof (PythonType)), Microsoft.Scripting.Ast.Utils.Convert(value.Expression, typeof (object))), (Expression) Expression.Constant((object) null)));
  }

  public override ErrorInfo MakeMissingMemberError(Type type, DynamicMetaObject self, string name)
  {
    string str = !typeof (TypeTracker).IsAssignableFrom(type) ? NameConverter.GetTypeName(type) : nameof (type);
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) $"'{str}' object has no attribute '{name}'")));
  }

  public override ErrorInfo MakeMissingMemberErrorForAssign(
    Type type,
    DynamicMetaObject self,
    string name)
  {
    if (self != null)
      return this.MakeMissingMemberError(type, self, name);
    return ErrorInfo.FromException((Expression) Expression.New(typeof (TypeErrorException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) $"can't set attributes of built-in/extension type '{NameConverter.GetTypeName(type)}'")));
  }

  public override ErrorInfo MakeMissingMemberErrorForAssignReadOnlyProperty(
    Type type,
    DynamicMetaObject self,
    string name)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) $"can't assign to read-only property {name} of type '{NameConverter.GetTypeName(type)}'")));
  }

  public override ErrorInfo MakeMissingMemberErrorForDelete(
    Type type,
    DynamicMetaObject self,
    string name)
  {
    return this.MakeMissingMemberErrorForAssign(type, self, name);
  }

  public override ErrorInfo MakeReadOnlyMemberError(Type type, string name)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) $"attribute '{name}' of '{NameConverter.GetTypeName(type)}' object is read-only")));
  }

  public override ErrorInfo MakeUndeletableMemberError(Type type, string name)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) $"cannot delete attribute '{name}' of builtin type '{NameConverter.GetTypeName(type)}'")));
  }

  internal IList<Type> GetExtensionTypesInternal(Type t)
  {
    List<Type> list = new List<Type>((IEnumerable<Type>) base.GetExtensionTypes(t));
    this.AddExtensionTypes(t, list);
    return (IList<Type>) list.ToArray();
  }

  public override bool IncludeExtensionMember(MemberInfo member)
  {
    return !member.DeclaringType.IsDefined(typeof (PythonHiddenBaseClassAttribute), false);
  }

  public override IList<Type> GetExtensionTypes(Type t)
  {
    List<Type> list = new List<Type>();
    list.Add(t);
    list.AddRange((IEnumerable<Type>) base.GetExtensionTypes(t));
    this.AddExtensionTypes(t, list);
    return (IList<Type>) list;
  }

  private void AddExtensionTypes(Type t, List<Type> list)
  {
    PythonBinder.ExtensionTypeInfo extensionTypeInfo;
    if (PythonBinder._sysTypes.TryGetValue(t, out extensionTypeInfo))
      list.Add(extensionTypeInfo.ExtensionType);
    lock (this._dlrExtensionTypes)
    {
      IList<Type> collection1;
      if (this._dlrExtensionTypes.TryGetValue(t, out collection1))
        list.AddRange((IEnumerable<Type>) collection1);
      if (this._registeredInterfaceExtensions)
      {
        foreach (Type key in t.GetInterfaces())
        {
          IList<Type> collection2;
          if (this._dlrExtensionTypes.TryGetValue(key, out collection2))
            list.AddRange((IEnumerable<Type>) collection2);
        }
      }
      if (!t.IsGenericType)
        return;
      Type genericTypeDefinition = t.GetGenericTypeDefinition();
      Type[] genericArguments = t.GetGenericArguments();
      if (!this._dlrExtensionTypes.TryGetValue(genericTypeDefinition, out collection1))
        return;
      foreach (Type type in (IEnumerable<Type>) collection1)
        list.Add(type.MakeGenericType(genericArguments));
    }
  }

  public bool HasExtensionTypes(Type t) => this._dlrExtensionTypes.ContainsKey(t);

  public override DynamicMetaObject ReturnMemberTracker(Type type, MemberTracker memberTracker)
  {
    return PythonBinder.ReturnMemberTracker(type, memberTracker, this.PrivateBinding) ?? base.ReturnMemberTracker(type, memberTracker);
  }

  private static DynamicMetaObject ReturnMemberTracker(
    Type type,
    MemberTracker memberTracker,
    bool privateBinding)
  {
    switch (memberTracker.MemberType)
    {
      case TrackerTypes.Constructor:
        MethodBase[] constructors = PythonTypeOps.GetConstructors(type, privateBinding, true);
        object obj = !PythonTypeOps.IsDefaultNew(constructors) ? (object) PythonTypeOps.GetConstructor(type, InstanceOps.NonDefaultNewInst, constructors) : (!PythonBinder.IsPythonType(type) ? (object) InstanceOps.NewCls : (object) InstanceOps.New);
        return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant(obj), BindingRestrictions.Empty, obj);
      case TrackerTypes.Event:
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeBoundEvent"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonTypeOps.GetReflectedEvent((EventTracker) memberTracker)), Microsoft.Scripting.Ast.Utils.Constant((object) null), Microsoft.Scripting.Ast.Utils.Constant((object) type)), BindingRestrictions.Empty);
      case TrackerTypes.Field:
        return new DynamicMetaObject(PythonBinder.ReturnFieldTracker((FieldTracker) memberTracker), BindingRestrictions.Empty);
      case TrackerTypes.Property:
        return new DynamicMetaObject(PythonBinder.ReturnPropertyTracker((PropertyTracker) memberTracker, privateBinding), BindingRestrictions.Empty);
      case TrackerTypes.Type:
        return PythonBinder.ReturnTypeTracker((TypeTracker) memberTracker);
      case TrackerTypes.MethodGroup:
        return new DynamicMetaObject(PythonBinder.ReturnMethodGroup((MethodGroup) memberTracker), BindingRestrictions.Empty);
      case TrackerTypes.TypeGroup:
        return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) memberTracker), BindingRestrictions.Empty, (object) memberTracker);
      case TrackerTypes.Custom:
        return new DynamicMetaObject((Expression) Microsoft.Scripting.Ast.Utils.Constant((object) ((PythonCustomTracker) memberTracker).GetSlot(), typeof (PythonTypeSlot)), BindingRestrictions.Empty, (object) ((PythonCustomTracker) memberTracker).GetSlot());
      case TrackerTypes.Bound:
        return new DynamicMetaObject(PythonBinder.ReturnBoundTracker((BoundMemberTracker) memberTracker, privateBinding), BindingRestrictions.Empty);
      default:
        return (DynamicMetaObject) null;
    }
  }

  public static PythonBinder GetBinder(CodeContext context) => context.LanguageContext.Binder;

  public bool TryLookupSlot(
    CodeContext context,
    PythonType type,
    string name,
    out PythonTypeSlot slot)
  {
    return this.TryLookupProtectedSlot(context, type, name, out slot);
  }

  internal bool TryLookupProtectedSlot(
    CodeContext context,
    PythonType type,
    string name,
    out PythonTypeSlot slot)
  {
    Type underlyingSystemType = type.UnderlyingSystemType;
    if (!this._typeMembers.TryGetCachedSlot(underlyingSystemType, true, name, out slot))
    {
      MemberGroup member = PythonTypeInfo.GetMember(this, MemberRequestKind.Get, underlyingSystemType, name);
      slot = PythonTypeOps.GetSlot(member, name, this.PrivateBinding);
      this._typeMembers.CacheSlot(underlyingSystemType, true, name, slot, member);
    }
    if (slot != null && (slot.IsAlwaysVisible || PythonOps.IsClsVisible(context)))
      return true;
    slot = (PythonTypeSlot) null;
    return false;
  }

  public bool TryResolveSlot(
    CodeContext context,
    PythonType type,
    PythonType owner,
    string name,
    out PythonTypeSlot slot)
  {
    Type underlyingSystemType = type.UnderlyingSystemType;
    if (!this._resolvedMembers.TryGetCachedSlot(underlyingSystemType, true, name, out slot))
    {
      MemberGroup memberAll = PythonTypeInfo.GetMemberAll(this, MemberRequestKind.Get, underlyingSystemType, name);
      slot = PythonTypeOps.GetSlot(memberAll, name, this.PrivateBinding);
      this._resolvedMembers.CacheSlot(underlyingSystemType, true, name, slot, memberAll);
    }
    if (slot != null && (slot.IsAlwaysVisible || PythonOps.IsClsVisible(context)))
      return true;
    slot = (PythonTypeSlot) null;
    return false;
  }

  public void LookupMembers(CodeContext context, PythonType type, PythonDictionary memberNames)
  {
    if (!this._typeMembers.IsFullyCached(type.UnderlyingSystemType, true))
    {
      Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>> members = new Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>>();
      foreach (ResolvedMember member in (IEnumerable<ResolvedMember>) PythonTypeInfo.GetMembers(this, MemberRequestKind.Get, type.UnderlyingSystemType))
      {
        if (!members.ContainsKey(member.Name))
          members[member.Name] = new KeyValuePair<PythonTypeSlot, MemberGroup>(PythonTypeOps.GetSlot(member.Member, member.Name, this.PrivateBinding), member.Member);
      }
      this._typeMembers.CacheAll(type.UnderlyingSystemType, true, members);
    }
    foreach (KeyValuePair<string, PythonTypeSlot> allMember in this._typeMembers.GetAllMembers(type.UnderlyingSystemType, true))
    {
      PythonTypeSlot pythonTypeSlot = allMember.Value;
      string key = allMember.Key;
      if (pythonTypeSlot.IsAlwaysVisible || PythonOps.IsClsVisible(context))
        memberNames[(object) key] = (object) pythonTypeSlot;
    }
  }

  public void ResolveMemberNames(
    CodeContext context,
    PythonType type,
    PythonType owner,
    Dictionary<string, string> memberNames)
  {
    if (!this._resolvedMembers.IsFullyCached(type.UnderlyingSystemType, true))
    {
      Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>> members = new Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>>();
      foreach (ResolvedMember resolvedMember in (IEnumerable<ResolvedMember>) PythonTypeInfo.GetMembersAll(this, MemberRequestKind.Get, type.UnderlyingSystemType))
      {
        if (!members.ContainsKey(resolvedMember.Name))
          members[resolvedMember.Name] = new KeyValuePair<PythonTypeSlot, MemberGroup>(PythonTypeOps.GetSlot(resolvedMember.Member, resolvedMember.Name, this.PrivateBinding), resolvedMember.Member);
      }
      this._resolvedMembers.CacheAll(type.UnderlyingSystemType, true, members);
    }
    foreach (KeyValuePair<string, PythonTypeSlot> allMember in this._resolvedMembers.GetAllMembers(type.UnderlyingSystemType, true))
    {
      PythonTypeSlot pythonTypeSlot = allMember.Value;
      string key = allMember.Key;
      if (pythonTypeSlot.IsAlwaysVisible || PythonOps.IsClsVisible(context))
        memberNames[key] = key;
    }
  }

  private static Expression ReturnFieldTracker(FieldTracker fieldTracker)
  {
    return Microsoft.Scripting.Ast.Utils.Constant((object) PythonTypeOps.GetReflectedField(fieldTracker.Field));
  }

  private static Expression ReturnMethodGroup(MethodGroup methodGroup)
  {
    return Microsoft.Scripting.Ast.Utils.Constant((object) PythonTypeOps.GetFinalSlotForFunction(PythonBinder.GetBuiltinFunction(methodGroup)));
  }

  private static Expression ReturnBoundTracker(
    BoundMemberTracker boundMemberTracker,
    bool privateBinding)
  {
    MemberTracker boundTo = boundMemberTracker.BoundTo;
    switch (boundTo.MemberType)
    {
      case TrackerTypes.Event:
        return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeBoundEvent"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonTypeOps.GetReflectedEvent((EventTracker) boundMemberTracker.BoundTo)), boundMemberTracker.Instance.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) boundMemberTracker.DeclaringType));
      case TrackerTypes.Property:
        PropertyTracker propertyTracker = (PropertyTracker) boundTo;
        return (Expression) Expression.New(typeof (ReflectedIndexer).GetConstructor(new Type[2]
        {
          typeof (ReflectedIndexer),
          typeof (object)
        }), Microsoft.Scripting.Ast.Utils.Constant((object) new ReflectedIndexer(((ReflectedPropertyTracker) propertyTracker).Property, NameType.Property, privateBinding)), boundMemberTracker.Instance.Expression);
      case TrackerTypes.MethodGroup:
        return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeBoundBuiltinFunction"), Microsoft.Scripting.Ast.Utils.Constant((object) PythonBinder.GetBuiltinFunction((MethodGroup) boundTo)), Microsoft.Scripting.Ast.Utils.Convert(boundMemberTracker.Instance.Expression, typeof (object)));
      default:
        throw new NotImplementedException();
    }
  }

  private static BuiltinFunction GetBuiltinFunction(MethodGroup mg)
  {
    MethodBase[] methods = new MethodBase[mg.Methods.Count];
    for (int index = 0; index < mg.Methods.Count; ++index)
      methods[index] = (MethodBase) mg.Methods[index].Method;
    return PythonTypeOps.GetBuiltinFunction(mg.DeclaringType, mg.Methods[0].Name, new FunctionType?(PythonTypeOps.GetMethodFunctionType(mg.DeclaringType, (MemberInfo[]) methods) & ~FunctionType.FunctionMethodMask | (mg.ContainsInstance ? FunctionType.Method : FunctionType.None) | (mg.ContainsStatic ? FunctionType.Function : FunctionType.None)), (MemberInfo[]) mg.GetMethodBases());
  }

  private static Expression ReturnPropertyTracker(
    PropertyTracker propertyTracker,
    bool privateBinding)
  {
    return Microsoft.Scripting.Ast.Utils.Constant((object) PythonTypeOps.GetReflectedProperty(propertyTracker, new MemberGroup(new MemberTracker[1]
    {
      (MemberTracker) propertyTracker
    }), (privateBinding ? 1 : 0) != 0));
  }

  private static DynamicMetaObject ReturnTypeTracker(TypeTracker memberTracker)
  {
    object pythonTypeFromType = (object) DynamicHelpers.GetPythonTypeFromType(memberTracker.Type);
    return new DynamicMetaObject((Expression) Expression.Constant(pythonTypeFromType), BindingRestrictions.Empty, pythonTypeFromType);
  }

  internal ScriptDomainManager DomainManager => this._context.DomainManager;

  internal static void AssertNotExtensionType(Type t)
  {
    foreach (PythonBinder.ExtensionTypeInfo extensionTypeInfo in PythonBinder._sysTypes.Values)
      ;
  }

  private static Dictionary<Type, IList<Type>> MakeExtensionTypes()
  {
    return new Dictionary<Type, IList<Type>>()
    {
      [typeof (DBNull)] = (IList<Type>) new Type[1]
      {
        typeof (DBNullOps)
      },
      [typeof (List<>)] = (IList<Type>) new Type[1]
      {
        typeof (ListOfTOps<>)
      },
      [typeof (Dictionary<,>)] = (IList<Type>) new Type[1]
      {
        typeof (DictionaryOfTOps<,>)
      },
      [typeof (Array)] = (IList<Type>) new Type[1]
      {
        typeof (ArrayOps)
      },
      [typeof (Assembly)] = (IList<Type>) new Type[1]
      {
        typeof (PythonAssemblyOps)
      },
      [typeof (Enum)] = (IList<Type>) new Type[1]
      {
        typeof (EnumOps)
      },
      [typeof (Delegate)] = (IList<Type>) new Type[1]
      {
        typeof (DelegateOps)
      },
      [typeof (byte)] = (IList<Type>) new Type[1]
      {
        typeof (ByteOps)
      },
      [typeof (sbyte)] = (IList<Type>) new Type[1]
      {
        typeof (SByteOps)
      },
      [typeof (short)] = (IList<Type>) new Type[1]
      {
        typeof (Int16Ops)
      },
      [typeof (ushort)] = (IList<Type>) new Type[1]
      {
        typeof (UInt16Ops)
      },
      [typeof (uint)] = (IList<Type>) new Type[1]
      {
        typeof (UInt32Ops)
      },
      [typeof (long)] = (IList<Type>) new Type[1]
      {
        typeof (Int64Ops)
      },
      [typeof (ulong)] = (IList<Type>) new Type[1]
      {
        typeof (UInt64Ops)
      },
      [typeof (char)] = (IList<Type>) new Type[1]
      {
        typeof (CharOps)
      },
      [typeof (Decimal)] = (IList<Type>) new Type[1]
      {
        typeof (DecimalOps)
      },
      [typeof (float)] = (IList<Type>) new Type[1]
      {
        typeof (SingleOps)
      }
    };
  }

  private static Dictionary<Type, PythonBinder.ExtensionTypeInfo> MakeSystemTypes()
  {
    return new Dictionary<Type, PythonBinder.ExtensionTypeInfo>()
    {
      [typeof (object)] = new PythonBinder.ExtensionTypeInfo(typeof (ObjectOps), "object"),
      [typeof (string)] = new PythonBinder.ExtensionTypeInfo(typeof (StringOps), "str"),
      [typeof (int)] = new PythonBinder.ExtensionTypeInfo(typeof (Int32Ops), "int"),
      [typeof (bool)] = new PythonBinder.ExtensionTypeInfo(typeof (BoolOps), "bool"),
      [typeof (double)] = new PythonBinder.ExtensionTypeInfo(typeof (DoubleOps), "float"),
      [typeof (ValueType)] = new PythonBinder.ExtensionTypeInfo(typeof (ValueType), "ValueType"),
      [typeof (BigInteger)] = new PythonBinder.ExtensionTypeInfo(typeof (BigIntegerOps), "long"),
      [typeof (Complex)] = new PythonBinder.ExtensionTypeInfo(typeof (ComplexOps), "complex"),
      [typeof (DynamicNull)] = new PythonBinder.ExtensionTypeInfo(typeof (NoneTypeOps), "NoneType"),
      [typeof (IDictionary<object, object>)] = new PythonBinder.ExtensionTypeInfo(typeof (DictionaryOps), "dict"),
      [typeof (NamespaceTracker)] = new PythonBinder.ExtensionTypeInfo(typeof (NamespaceTrackerOps), "namespace#"),
      [typeof (TypeGroup)] = new PythonBinder.ExtensionTypeInfo(typeof (TypeGroupOps), "type-collision"),
      [typeof (TypeTracker)] = new PythonBinder.ExtensionTypeInfo(typeof (TypeTrackerOps), "type-collision")
    };
  }

  internal static string GetTypeNameInternal(Type t)
  {
    PythonBinder.ExtensionTypeInfo extensionTypeInfo;
    if (PythonBinder._sysTypes.TryGetValue(t, out extensionTypeInfo))
      return extensionTypeInfo.PythonName;
    PythonTypeAttribute pythonTypeAttribute = CustomAttributeExtensions.GetCustomAttributes<PythonTypeAttribute>((MemberInfo) t, false).FirstOrDefault<PythonTypeAttribute>();
    return pythonTypeAttribute != null && pythonTypeAttribute.Name != null ? pythonTypeAttribute.Name : t.Name;
  }

  public static bool IsExtendedType(Type t) => PythonBinder._sysTypes.ContainsKey(t);

  public static bool IsPythonType(Type t)
  {
    return PythonBinder._sysTypes.ContainsKey(t) || t.IsDefined(typeof (PythonTypeAttribute), false);
  }

  private void DomainManager_AssemblyLoaded(object sender, AssemblyLoadedEventArgs e)
  {
    Assembly assembly = e.Assembly;
    IEnumerable<ExtensionTypeAttribute> customAttributes = CustomAttributeExtensions.GetCustomAttributes<ExtensionTypeAttribute>(assembly);
    if (customAttributes.Any<ExtensionTypeAttribute>())
    {
      lock (this._dlrExtensionTypes)
      {
        foreach (ExtensionTypeAttribute extensionTypeAttribute in customAttributes)
        {
          if (extensionTypeAttribute.Extends.IsInterface())
            this._registeredInterfaceExtensions = true;
          IList<Type> collection;
          if (!this._dlrExtensionTypes.TryGetValue(extensionTypeAttribute.Extends, out collection))
            this._dlrExtensionTypes[extensionTypeAttribute.Extends] = collection = (IList<Type>) new List<Type>();
          else if (collection.IsReadOnly)
            this._dlrExtensionTypes[extensionTypeAttribute.Extends] = collection = (IList<Type>) new List<Type>((IEnumerable<Type>) collection);
          if (!collection.Contains(extensionTypeAttribute.ExtensionType))
            collection.Add(extensionTypeAttribute.ExtensionType);
        }
      }
    }
    TopNamespaceTracker.PublishComTypes(assembly);
    ClrModule.ReferencesList referencedAssemblies = this._context.ReferencedAssemblies;
    lock (referencedAssemblies)
      referencedAssemblies.Add(assembly);
    PythonBinder.LoadScriptCode(this._context, assembly);
    this._context.LoadBuiltins(this._context.BuiltinModules, assembly, true);
    NewTypeMaker.LoadNewTypes(assembly);
  }

  private static void LoadScriptCode(PythonContext pc, Assembly asm)
  {
    foreach (ScriptCode code in SavableScriptCode.LoadFromAssembly(pc.DomainManager, asm))
      pc.GetCompiledLoader().AddScriptCode(code);
  }

  internal PythonContext Context => this._context;

  private class ExtensionTypeInfo
  {
    public Type ExtensionType;
    public string PythonName;

    public ExtensionTypeInfo(Type extensionType, string pythonName)
    {
      this.ExtensionType = extensionType;
      this.PythonName = pythonName;
    }
  }

  private class SlotCache
  {
    private Dictionary<PythonBinder.SlotCache.CachedInfoKey, PythonBinder.SlotCache.SlotCacheInfo> _cachedInfos;

    public void CacheSlot(
      Type type,
      bool isGetMember,
      string name,
      PythonTypeSlot slot,
      MemberGroup memberGroup)
    {
      this.EnsureInfo();
      lock (this._cachedInfos)
      {
        PythonBinder.SlotCache.SlotCacheInfo slotForType = this.GetSlotForType(type, isGetMember);
        if (slotForType.ResolvedAll && slot == null && memberGroup.Count == 0)
          return;
        slotForType.Members[name] = new KeyValuePair<PythonTypeSlot, MemberGroup>(slot, memberGroup);
      }
    }

    public bool TryGetCachedSlot(
      Type type,
      bool isGetMember,
      string name,
      out PythonTypeSlot slot)
    {
      if (this._cachedInfos != null)
      {
        lock (this._cachedInfos)
        {
          PythonBinder.SlotCache.SlotCacheInfo slotCacheInfo;
          if (this._cachedInfos.TryGetValue(new PythonBinder.SlotCache.CachedInfoKey(type, isGetMember), out slotCacheInfo))
          {
            if (!slotCacheInfo.TryGetSlot(name, out slot))
            {
              if (!slotCacheInfo.ResolvedAll)
                goto label_9;
            }
            return true;
          }
        }
      }
label_9:
      slot = (PythonTypeSlot) null;
      return false;
    }

    public bool TryGetCachedMember(
      Type type,
      string name,
      bool getMemberAction,
      out MemberGroup group)
    {
      if (this._cachedInfos != null)
      {
        lock (this._cachedInfos)
        {
          PythonBinder.SlotCache.SlotCacheInfo slotCacheInfo;
          if (this._cachedInfos.TryGetValue(new PythonBinder.SlotCache.CachedInfoKey(type, getMemberAction), out slotCacheInfo))
          {
            if (!slotCacheInfo.TryGetMember(name, out group))
            {
              if (getMemberAction)
              {
                if (!slotCacheInfo.ResolvedAll)
                  goto label_10;
              }
              else
                goto label_10;
            }
            return true;
          }
        }
      }
label_10:
      group = MemberGroup.EmptyGroup;
      return false;
    }

    public bool IsFullyCached(Type type, bool isGetMember)
    {
      if (this._cachedInfos != null)
      {
        lock (this._cachedInfos)
        {
          PythonBinder.SlotCache.SlotCacheInfo slotCacheInfo;
          if (this._cachedInfos.TryGetValue(new PythonBinder.SlotCache.CachedInfoKey(type, isGetMember), out slotCacheInfo))
            return slotCacheInfo.ResolvedAll;
        }
      }
      return false;
    }

    public void CacheAll(
      Type type,
      bool isGetMember,
      Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>> members)
    {
      this.EnsureInfo();
      lock (this._cachedInfos)
      {
        PythonBinder.SlotCache.SlotCacheInfo slotForType = this.GetSlotForType(type, isGetMember);
        slotForType.Members = members;
        slotForType.ResolvedAll = true;
      }
    }

    public IEnumerable<KeyValuePair<string, PythonTypeSlot>> GetAllMembers(
      Type type,
      bool isGetMember)
    {
      foreach (KeyValuePair<string, PythonTypeSlot> allSlot in this.GetSlotForType(type, isGetMember).GetAllSlots())
      {
        if (allSlot.Value != null)
          yield return allSlot;
      }
    }

    private PythonBinder.SlotCache.SlotCacheInfo GetSlotForType(Type type, bool isGetMember)
    {
      PythonBinder.SlotCache.CachedInfoKey key = new PythonBinder.SlotCache.CachedInfoKey(type, isGetMember);
      PythonBinder.SlotCache.SlotCacheInfo slotForType;
      if (!this._cachedInfos.TryGetValue(key, out slotForType))
        this._cachedInfos[key] = slotForType = new PythonBinder.SlotCache.SlotCacheInfo();
      return slotForType;
    }

    private void EnsureInfo()
    {
      if (this._cachedInfos != null)
        return;
      Interlocked.CompareExchange<Dictionary<PythonBinder.SlotCache.CachedInfoKey, PythonBinder.SlotCache.SlotCacheInfo>>(ref this._cachedInfos, new Dictionary<PythonBinder.SlotCache.CachedInfoKey, PythonBinder.SlotCache.SlotCacheInfo>(), (Dictionary<PythonBinder.SlotCache.CachedInfoKey, PythonBinder.SlotCache.SlotCacheInfo>) null);
    }

    private class CachedInfoKey : IEquatable<PythonBinder.SlotCache.CachedInfoKey>
    {
      public readonly Type Type;
      public readonly bool IsGetMember;

      public CachedInfoKey(Type type, bool isGetMember)
      {
        this.Type = type;
        this.IsGetMember = isGetMember;
      }

      public bool Equals(PythonBinder.SlotCache.CachedInfoKey other)
      {
        return other.Type == this.Type && other.IsGetMember == this.IsGetMember;
      }

      public override bool Equals(object obj)
      {
        return obj is PythonBinder.SlotCache.CachedInfoKey other && this.Equals(other);
      }

      public override int GetHashCode() => this.Type.GetHashCode() ^ (this.IsGetMember ? -1 : 0);
    }

    private class SlotCacheInfo
    {
      public Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>> Members;
      public bool ResolvedAll;

      public SlotCacheInfo()
      {
        this.Members = new Dictionary<string, KeyValuePair<PythonTypeSlot, MemberGroup>>((IEqualityComparer<string>) StringComparer.Ordinal);
      }

      public bool TryGetSlot(string name, out PythonTypeSlot slot)
      {
        KeyValuePair<PythonTypeSlot, MemberGroup> keyValuePair;
        if (this.Members.TryGetValue(name, out keyValuePair))
        {
          slot = keyValuePair.Key;
          return true;
        }
        slot = (PythonTypeSlot) null;
        return false;
      }

      public bool TryGetMember(string name, out MemberGroup group)
      {
        KeyValuePair<PythonTypeSlot, MemberGroup> keyValuePair;
        if (this.Members.TryGetValue(name, out keyValuePair))
        {
          group = keyValuePair.Value;
          return true;
        }
        group = MemberGroup.EmptyGroup;
        return false;
      }

      public IEnumerable<KeyValuePair<string, PythonTypeSlot>> GetAllSlots()
      {
        foreach (KeyValuePair<string, KeyValuePair<PythonTypeSlot, MemberGroup>> member in this.Members)
          yield return new KeyValuePair<string, PythonTypeSlot>(member.Key, member.Value.Key);
      }
    }
  }
}
