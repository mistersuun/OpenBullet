// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.PropertyTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public abstract class PropertyTracker : MemberTracker
{
  public override TrackerTypes MemberType => TrackerTypes.Property;

  public abstract MethodInfo GetGetMethod();

  public abstract MethodInfo GetSetMethod();

  public abstract MethodInfo GetGetMethod(bool privateMembers);

  public abstract MethodInfo GetSetMethod(bool privateMembers);

  public virtual MethodInfo GetDeleteMethod() => (MethodInfo) null;

  public virtual MethodInfo GetDeleteMethod(bool privateMembers) => (MethodInfo) null;

  public abstract ParameterInfo[] GetIndexParameters();

  public abstract bool IsStatic { get; }

  public abstract Type PropertyType { get; }

  public override DynamicMetaObject GetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType)
  {
    if (!this.IsStatic || this.GetIndexParameters().Length != 0)
      return binder.ReturnMemberTracker(instanceType, (MemberTracker) this);
    MethodInfo methodInfo = this.ResolveGetter(instanceType, binder.PrivateBinding);
    if (methodInfo == (MethodInfo) null || methodInfo.ContainsGenericParameters)
      return (DynamicMetaObject) null;
    return methodInfo.IsPublic && methodInfo.DeclaringType.IsPublic() ? binder.MakeCallExpression(resolverFactory, methodInfo) : MemberTracker.FromMemberInfo((MemberInfo) methodInfo).Call(resolverFactory, binder);
  }

  public override ErrorInfo GetError(ActionBinder binder, Type instanceType)
  {
    MethodInfo methodInfo = this.ResolveGetter(instanceType, binder.PrivateBinding);
    if (methodInfo == (MethodInfo) null)
      return binder.MakeMissingMemberErrorInfo(this.DeclaringType, this.Name);
    if (methodInfo.ContainsGenericParameters)
      return binder.MakeGenericAccessError((MemberTracker) this);
    throw new InvalidOperationException();
  }

  protected internal override DynamicMetaObject GetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject instance)
  {
    if (instance != null && this.IsStatic)
      return (DynamicMetaObject) null;
    if (this.GetIndexParameters().Length != 0)
      return binder.ReturnMemberTracker(type, this.BindToInstance(instance));
    MethodInfo getMethod = this.GetGetMethod(true);
    if (getMethod == (MethodInfo) null || getMethod.ContainsGenericParameters)
      return (DynamicMetaObject) null;
    MethodInfo callableMethod = CompilerHelpers.TryGetCallableMethod(instance.GetLimitType(), getMethod);
    DefaultBinder defaultBinder = (DefaultBinder) binder;
    if (!binder.PrivateBinding && !CompilerHelpers.IsVisible((MethodBase) callableMethod))
      return DefaultBinder.MakeError(defaultBinder.MakeNonPublicMemberGetError(resolverFactory, (MemberTracker) this, type, instance), BindingRestrictions.Empty, typeof (object));
    return defaultBinder.MakeCallExpression(resolverFactory, callableMethod, instance);
  }

  public override ErrorInfo GetBoundError(
    ActionBinder binder,
    DynamicMetaObject instance,
    Type instanceType)
  {
    MethodInfo methodInfo = this.ResolveGetter(instanceType, binder.PrivateBinding);
    if (methodInfo == (MethodInfo) null)
      return binder.MakeMissingMemberErrorInfo(this.DeclaringType, this.Name);
    if (methodInfo.ContainsGenericParameters)
      return binder.MakeGenericAccessError((MemberTracker) this);
    if (!this.IsStatic)
      throw new InvalidOperationException();
    return binder.MakeStaticPropertyInstanceAccessError(this, false, instance);
  }

  public override MemberTracker BindToInstance(DynamicMetaObject instance)
  {
    return (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance);
  }

  private MethodInfo ResolveGetter(Type instanceType, bool privateBinding)
  {
    MethodInfo getMethod = this.GetGetMethod(true);
    if (getMethod != (MethodInfo) null)
    {
      MethodInfo callableMethod = CompilerHelpers.TryGetCallableMethod(instanceType, getMethod);
      if (privateBinding || CompilerHelpers.IsVisible((MethodBase) callableMethod))
        return callableMethod;
    }
    return (MethodInfo) null;
  }
}
