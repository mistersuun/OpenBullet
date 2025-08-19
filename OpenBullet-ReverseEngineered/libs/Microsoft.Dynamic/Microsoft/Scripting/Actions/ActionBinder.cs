// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ActionBinder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public abstract class ActionBinder
{
  private ScriptDomainManager _manager;

  public virtual bool PrivateBinding
  {
    get => this._manager != null && this._manager.Configuration.PrivateBinding;
  }

  protected ActionBinder()
  {
  }

  [Obsolete("ScriptDomainManager is no longer required by ActionBinder and will go away, you should call the default constructor instead.  You should also override PrivateBinding which is the only thing which previously used the ScriptDomainManager.")]
  protected ActionBinder(ScriptDomainManager manager) => this._manager = manager;

  [Obsolete("ScriptDomainManager is no longer required by ActionBinder and will no longer be available on the base class.")]
  public ScriptDomainManager Manager => this._manager;

  public virtual object Convert(object obj, Type toType)
  {
    if (obj == null)
    {
      if (!toType.IsValueType())
        return (object) null;
    }
    else if (toType.IsValueType())
    {
      if (toType == obj.GetType())
        return obj;
    }
    else if (toType.IsAssignableFrom(obj.GetType()))
      return obj;
    throw Microsoft.Scripting.Error.InvalidCast((object) (obj?.GetType().Name ?? "(null)"), (object) toType.Name);
  }

  public abstract bool CanConvertFrom(
    Type fromType,
    Type toType,
    bool toNotNullable,
    NarrowingLevel level);

  public abstract Candidate PreferConvert(Type t1, Type t2);

  public virtual Expression ConvertExpression(
    Expression expr,
    Type toType,
    ConversionResultKind kind,
    OverloadResolverFactory resolverFactory)
  {
    ContractUtils.RequiresNotNull((object) expr, nameof (expr));
    ContractUtils.RequiresNotNull((object) toType, nameof (toType));
    Type type = expr.Type;
    return toType == typeof (object) ? (type.IsValueType() ? Microsoft.Scripting.Ast.Utils.Convert(expr, toType) : expr) : (toType.IsAssignableFrom(type) ? expr : (Expression) Expression.Convert(expr, CompilerHelpers.GetVisibleType(toType)));
  }

  public virtual MemberGroup GetMember(MemberRequestKind action, Type type, string name)
  {
    IEnumerable<MemberInfo> memberInfos = type.GetInheritedMembers(name);
    if (!this.PrivateBinding)
      memberInfos = CompilerHelpers.FilterNonVisibleMembers(type, memberInfos);
    MemberGroup collection = new MemberGroup(memberInfos.ToArray<MemberInfo>());
    string str = name + "`";
    List<Type> typeList = (List<Type>) null;
    foreach (Type declaredNestedType in type.GetDeclaredNestedTypes())
    {
      if (declaredNestedType.IsPublic && declaredNestedType.Name.StartsWith(str))
      {
        if (typeList == null)
          typeList = new List<Type>();
        typeList.Add(declaredNestedType);
      }
    }
    if (typeList != null)
    {
      List<MemberTracker> memberTrackerList = new List<MemberTracker>((IEnumerable<MemberTracker>) collection);
      foreach (Type member in typeList)
        memberTrackerList.Add(MemberTracker.FromMemberInfo((MemberInfo) member));
      return MemberGroup.CreateInternal(memberTrackerList.ToArray());
    }
    if (collection.Count == 0)
    {
      collection = new MemberGroup(type.GetInheritedMembers(name, true).WithBindingFlags(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).ToArray<MemberInfo>());
      if (collection.Count == 0)
        collection = this.GetAllExtensionMembers(type, name);
    }
    return collection;
  }

  public virtual ErrorInfo MakeContainsGenericParametersError(MemberTracker tracker)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (InvalidOperationException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) Microsoft.Scripting.Strings.InvalidOperation_ContainsGenericParameters((object) tracker.DeclaringType.Name, (object) tracker.Name))));
  }

  public virtual ErrorInfo MakeMissingMemberErrorInfo(Type type, string name)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) name)));
  }

  public virtual ErrorInfo MakeGenericAccessError(MemberTracker info)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MemberAccessException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) info.Name)));
  }

  public ErrorInfo MakeStaticPropertyInstanceAccessError(
    PropertyTracker tracker,
    bool isAssignment,
    params DynamicMetaObject[] parameters)
  {
    return this.MakeStaticPropertyInstanceAccessError(tracker, isAssignment, (IList<DynamicMetaObject>) parameters);
  }

  public virtual ErrorInfo MakeStaticAssignFromDerivedTypeError(
    Type accessingType,
    DynamicMetaObject self,
    MemberTracker assigning,
    DynamicMetaObject assignedValue,
    OverloadResolverFactory context)
  {
    switch (assigning.MemberType)
    {
      case TrackerTypes.Field:
        FieldTracker fieldTracker = (FieldTracker) assigning;
        return ErrorInfo.FromValueNoError((Expression) Expression.Assign((Expression) Expression.Field((Expression) null, fieldTracker.Field), this.ConvertExpression(assignedValue.Expression, fieldTracker.FieldType, ConversionResultKind.ExplicitCast, context)));
      case TrackerTypes.Property:
        PropertyTracker propertyTracker = (PropertyTracker) assigning;
        MethodInfo setMethod = propertyTracker.GetSetMethod();
        if ((object) setMethod == null)
          setMethod = propertyTracker.GetSetMethod(true);
        MethodInfo method = setMethod;
        return ErrorInfo.FromValueNoError((Expression) Microsoft.Scripting.Ast.Utils.SimpleCallHelper(method, this.ConvertExpression(assignedValue.Expression, method.GetParameters()[0].ParameterType, ConversionResultKind.ExplicitCast, context)));
      default:
        throw new InvalidOperationException();
    }
  }

  public virtual ErrorInfo MakeStaticPropertyInstanceAccessError(
    PropertyTracker tracker,
    bool isAssignment,
    IList<DynamicMetaObject> parameters)
  {
    ContractUtils.RequiresNotNull((object) tracker, nameof (tracker));
    ContractUtils.Requires(tracker.IsStatic, nameof (tracker), Microsoft.Scripting.Strings.ExpectedStaticProperty);
    ContractUtils.RequiresNotNull((object) parameters, nameof (parameters));
    ContractUtils.RequiresNotNullItems<DynamicMetaObject>(parameters, nameof (parameters));
    string str = isAssignment ? Microsoft.Scripting.Strings.StaticAssignmentFromInstanceError((object) tracker.Name, (object) tracker.DeclaringType.Name) : Microsoft.Scripting.Strings.StaticAccessFromInstanceError((object) tracker.Name, (object) tracker.DeclaringType.Name);
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) str)));
  }

  public virtual ErrorInfo MakeSetValueTypeFieldError(
    FieldTracker field,
    DynamicMetaObject instance,
    DynamicMetaObject value)
  {
    return ErrorInfo.FromException((Expression) Expression.Throw((Expression) Expression.New(typeof (ArgumentException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) "cannot assign to value types")), typeof (object)));
  }

  public virtual ErrorInfo MakeConversionError(Type toType, Expression value)
  {
    return ErrorInfo.FromException((Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<Type, object, Exception>(ScriptingRuntimeHelpers.CannotConvertError)), Microsoft.Scripting.Ast.Utils.Constant((object) toType), Microsoft.Scripting.Ast.Utils.Convert(value, typeof (object))));
  }

  public virtual ErrorInfo MakeMissingMemberError(Type type, DynamicMetaObject self, string name)
  {
    return ErrorInfo.FromException((Expression) Expression.New(typeof (MissingMemberException).GetConstructor(new Type[1]
    {
      typeof (string)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) name)));
  }

  public virtual ErrorInfo MakeMissingMemberErrorForAssign(
    Type type,
    DynamicMetaObject self,
    string name)
  {
    return this.MakeMissingMemberError(type, self, name);
  }

  public virtual ErrorInfo MakeMissingMemberErrorForAssignReadOnlyProperty(
    Type type,
    DynamicMetaObject self,
    string name)
  {
    return this.MakeMissingMemberError(type, self, name);
  }

  public virtual ErrorInfo MakeMissingMemberErrorForDelete(
    Type type,
    DynamicMetaObject self,
    string name)
  {
    return this.MakeMissingMemberError(type, self, name);
  }

  public virtual string GetTypeName(Type t) => t.Name;

  public virtual string GetObjectTypeName(object arg)
  {
    return this.GetTypeName(CompilerHelpers.GetType(arg));
  }

  public MemberGroup GetAllExtensionMembers(Type type, string name)
  {
    foreach (Type ancestor in type.Ancestors())
    {
      MemberGroup extensionMembers = this.GetExtensionMembers(ancestor, name);
      if (extensionMembers.Count != 0)
        return extensionMembers;
    }
    return MemberGroup.EmptyGroup;
  }

  public MemberGroup GetExtensionMembers(Type declaringType, string name)
  {
    IList<Type> extensionTypes = this.GetExtensionTypes(declaringType);
    List<MemberTracker> memberTrackerList = new List<MemberTracker>();
    foreach (Type type in (IEnumerable<Type>) extensionTypes)
    {
      foreach (MemberInfo withBindingFlag in type.GetDeclaredMembers(name).WithBindingFlags(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
      {
        MemberInfo member = withBindingFlag;
        if ((this.PrivateBinding || (member = CompilerHelpers.TryGetVisibleMember(type, withBindingFlag)) != (MemberInfo) null) && this.IncludeExtensionMember(member))
        {
          if (type != declaringType)
            memberTrackerList.Add(MemberTracker.FromMemberInfo(member, declaringType));
          else
            memberTrackerList.Add(MemberTracker.FromMemberInfo(member));
        }
      }
      MethodInfo extensionOperator1 = this.GetExtensionOperator(type, "Get" + name);
      MethodInfo extensionOperator2 = this.GetExtensionOperator(type, "Set" + name);
      MethodInfo extensionOperator3 = this.GetExtensionOperator(type, "Delete" + name);
      if (extensionOperator1 != (MethodInfo) null || extensionOperator2 != (MethodInfo) null || extensionOperator3 != (MethodInfo) null)
        memberTrackerList.Add((MemberTracker) new ExtensionPropertyTracker(name, extensionOperator1, extensionOperator2, extensionOperator3, declaringType));
    }
    return memberTrackerList.Count != 0 ? MemberGroup.CreateInternal(memberTrackerList.ToArray()) : MemberGroup.EmptyGroup;
  }

  private MethodInfo GetExtensionOperator(Type ext, string name)
  {
    return ext.GetInheritedMethods(name).WithBindingFlags(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Where<MethodInfo>((Func<MethodInfo, bool>) (mi => mi.IsDefined(typeof (PropertyMethodAttribute), false))).SingleOrDefault<MethodInfo>();
  }

  public virtual bool IncludeExtensionMember(MemberInfo member) => true;

  public virtual IList<Type> GetExtensionTypes(Type t) => (IList<Type>) new Type[0];

  public virtual DynamicMetaObject ReturnMemberTracker(Type type, MemberTracker memberTracker)
  {
    if (memberTracker.MemberType != TrackerTypes.Bound)
      return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) memberTracker), BindingRestrictions.Empty, (object) memberTracker);
    BoundMemberTracker boundMemberTracker = (BoundMemberTracker) memberTracker;
    return new DynamicMetaObject((Expression) Expression.New(typeof (BoundMemberTracker).GetConstructor(new Type[2]
    {
      typeof (MemberTracker),
      typeof (object)
    }), Microsoft.Scripting.Ast.Utils.Constant((object) boundMemberTracker.BoundTo), boundMemberTracker.Instance.Expression), BindingRestrictions.Empty);
  }

  public DynamicMetaObject MakeCallExpression(
    OverloadResolverFactory resolverFactory,
    MethodInfo method,
    params DynamicMetaObject[] parameters)
  {
    OverloadResolver overloadResolver = !method.IsStatic ? (OverloadResolver) resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) parameters, new CallSignature(parameters.Length - 1), CallTypes.ImplicitInstance) : (OverloadResolver) resolverFactory.CreateOverloadResolver((IList<DynamicMetaObject>) parameters, new CallSignature(parameters.Length), CallTypes.None);
    BindingTarget target = overloadResolver.ResolveOverload(method.Name, (IList<MethodBase>) new MethodBase[1]
    {
      (MethodBase) method
    }, NarrowingLevel.None, NarrowingLevel.All);
    if (target.Success)
      return new DynamicMetaObject(target.MakeExpression(), target.RestrictedArguments.GetAllRestrictions());
    BindingRestrictions restrictions = BindingRestrictions.Combine((IList<DynamicMetaObject>) parameters);
    foreach (DynamicMetaObject parameter in parameters)
      restrictions = restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(parameter.Expression, parameter.GetLimitType()));
    return DefaultBinder.MakeError(overloadResolver.MakeInvalidParametersError(target), restrictions, typeof (object));
  }
}
