// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.MemberTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public abstract class MemberTracker
{
  public static readonly MemberTracker[] EmptyTrackers = new MemberTracker[0];
  private static readonly Dictionary<MemberTracker.MemberKey, MemberTracker> _trackers = new Dictionary<MemberTracker.MemberKey, MemberTracker>();

  internal MemberTracker()
  {
  }

  public abstract TrackerTypes MemberType { get; }

  public abstract Type DeclaringType { get; }

  public abstract string Name { get; }

  public static MemberTracker FromMemberInfo(MemberInfo member)
  {
    return MemberTracker.FromMemberInfo(member, (Type) null);
  }

  public static MemberTracker FromMemberInfo(MemberInfo member, Type extending)
  {
    ContractUtils.RequiresNotNull((object) member, nameof (member));
    lock (MemberTracker._trackers)
    {
      MemberTracker.MemberKey key = new MemberTracker.MemberKey(member, extending);
      MemberTracker memberTracker;
      if (MemberTracker._trackers.TryGetValue(key, out memberTracker))
        return memberTracker;
      MethodInfo method;
      if ((method = member as MethodInfo) != (MethodInfo) null)
      {
        memberTracker = !(extending != (Type) null) ? (MemberTracker) new MethodTracker(method) : (MemberTracker) new ExtensionMethodTracker(method, member.IsDefined(typeof (StaticExtensionMethodAttribute), false), extending);
      }
      else
      {
        ConstructorInfo ctor;
        if ((ctor = member as ConstructorInfo) != (ConstructorInfo) null)
        {
          memberTracker = (MemberTracker) new ConstructorTracker(ctor);
        }
        else
        {
          FieldInfo field;
          if ((field = member as FieldInfo) != (FieldInfo) null)
          {
            memberTracker = (MemberTracker) new FieldTracker(field);
          }
          else
          {
            PropertyInfo property;
            if ((property = member as PropertyInfo) != (PropertyInfo) null)
            {
              memberTracker = (MemberTracker) new ReflectedPropertyTracker(property);
            }
            else
            {
              EventInfo eventInfo;
              if ((eventInfo = member as EventInfo) != (EventInfo) null)
              {
                memberTracker = (MemberTracker) new EventTracker(eventInfo);
              }
              else
              {
                Type type;
                if (!((type = member as Type) != (Type) null))
                  throw Error.UnknownMemberType((object) member);
                memberTracker = (MemberTracker) new NestedTypeTracker(type);
              }
            }
          }
        }
      }
      MemberTracker._trackers[key] = memberTracker;
      return memberTracker;
    }
  }

  public virtual DynamicMetaObject GetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType)
  {
    return binder.ReturnMemberTracker(instanceType, this);
  }

  public virtual DynamicMetaObject SetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject value)
  {
    return (DynamicMetaObject) null;
  }

  public virtual DynamicMetaObject SetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    return this.SetValue(resolverFactory, binder, instanceType, value);
  }

  internal virtual DynamicMetaObject Call(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    params DynamicMetaObject[] arguments)
  {
    return (DynamicMetaObject) null;
  }

  public virtual ErrorInfo GetError(ActionBinder binder, Type instanceType) => (ErrorInfo) null;

  public virtual ErrorInfo GetBoundError(
    ActionBinder binder,
    DynamicMetaObject instance,
    Type instanceType)
  {
    return (ErrorInfo) null;
  }

  protected internal virtual DynamicMetaObject GetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject instance)
  {
    return this.GetValue(resolverFactory, binder, instanceType);
  }

  protected internal virtual DynamicMetaObject SetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject value,
    DynamicMetaObject instance)
  {
    return this.SetValue(resolverFactory, binder, instanceType, instance);
  }

  protected internal virtual DynamicMetaObject SetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject value,
    DynamicMetaObject instance,
    DynamicMetaObject errorSuggestion)
  {
    return this.SetValue(resolverFactory, binder, instanceType, instance, errorSuggestion);
  }

  public virtual MemberTracker BindToInstance(DynamicMetaObject instance) => this;

  private class MemberKey
  {
    private readonly MemberInfo Member;
    private readonly Type Extending;

    public MemberKey(MemberInfo member, Type extending)
    {
      this.Member = member;
      this.Extending = extending;
    }

    public override int GetHashCode()
    {
      int hashCode = this.Member.GetHashCode();
      if (this.Extending != (Type) null)
        hashCode ^= this.Extending.GetHashCode();
      return hashCode;
    }

    public override bool Equals(object obj)
    {
      return obj is MemberTracker.MemberKey memberKey && memberKey.Member == this.Member && memberKey.Extending == this.Extending;
    }
  }
}
