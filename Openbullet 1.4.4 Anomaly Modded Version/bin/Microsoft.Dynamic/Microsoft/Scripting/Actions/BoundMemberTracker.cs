// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.BoundMemberTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using System;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class BoundMemberTracker : MemberTracker
{
  public BoundMemberTracker(MemberTracker tracker, DynamicMetaObject instance)
  {
    this.BoundTo = tracker;
    this.Instance = instance;
  }

  public BoundMemberTracker(MemberTracker tracker, object instance)
  {
    this.BoundTo = tracker;
    this.ObjectInstance = instance;
  }

  public override TrackerTypes MemberType => TrackerTypes.Bound;

  public override Type DeclaringType => this.BoundTo.DeclaringType;

  public override string Name => this.BoundTo.Name;

  public DynamicMetaObject Instance { get; }

  public object ObjectInstance { get; }

  public MemberTracker BoundTo { get; }

  public override DynamicMetaObject GetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType)
  {
    return this.BoundTo.GetBoundValue(resolverFactory, binder, instanceType, this.Instance);
  }

  public override ErrorInfo GetError(ActionBinder binder, Type instanceType)
  {
    return this.BoundTo.GetBoundError(binder, this.Instance, instanceType);
  }

  public override DynamicMetaObject SetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject value)
  {
    return this.BoundTo.SetBoundValue(resolverFactory, binder, instanceType, value, this.Instance);
  }

  public override DynamicMetaObject SetValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type instanceType,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    return this.BoundTo.SetBoundValue(resolverFactory, binder, instanceType, value, this.Instance, errorSuggestion);
  }
}
