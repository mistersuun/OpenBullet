// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.MethodGroup
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class MethodGroup : MemberTracker
{
  private MethodTracker[] _methods;
  private Dictionary<MethodGroup.TypeList, MethodGroup> _boundGenerics;

  internal MethodGroup(params MethodTracker[] methods) => this._methods = methods;

  public override TrackerTypes MemberType => TrackerTypes.MethodGroup;

  public override Type DeclaringType => this._methods[0].DeclaringType;

  public override string Name => this._methods[0].Name;

  public bool ContainsInstance
  {
    get
    {
      foreach (MethodTracker method in this._methods)
      {
        if (!method.IsStatic)
          return true;
      }
      return false;
    }
  }

  public bool ContainsStatic
  {
    get
    {
      foreach (MethodTracker method in this._methods)
      {
        if (method.IsStatic)
          return true;
      }
      return false;
    }
  }

  public IList<MethodTracker> Methods => (IList<MethodTracker>) this._methods;

  public MethodBase[] GetMethodBases()
  {
    MethodBase[] methodBases = new MethodBase[this.Methods.Count];
    for (int index = 0; index < this.Methods.Count; ++index)
      methodBases[index] = (MethodBase) this.Methods[index].Method;
    return methodBases;
  }

  public override MemberTracker BindToInstance(DynamicMetaObject instance)
  {
    return this.ContainsInstance ? (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance) : (MemberTracker) this;
  }

  protected internal override DynamicMetaObject GetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject instance)
  {
    return binder.ReturnMemberTracker(type, this.BindToInstance(instance));
  }

  public MethodGroup MakeGenericMethod(Type[] types)
  {
    MethodGroup.TypeList key = new MethodGroup.TypeList(types);
    if (this._boundGenerics != null)
    {
      lock (this._boundGenerics)
      {
        MethodGroup methodGroup;
        if (this._boundGenerics.TryGetValue(key, out methodGroup))
          return methodGroup;
      }
    }
    List<MethodTracker> methodTrackerList = new List<MethodTracker>(this.Methods.Count);
    foreach (MethodTracker method1 in (IEnumerable<MethodTracker>) this.Methods)
    {
      MethodInfo method2 = method1.Method;
      if (method2.ContainsGenericParameters && method2.GetGenericArguments().Length == types.Length)
        methodTrackerList.Add((MethodTracker) MemberTracker.FromMemberInfo((MemberInfo) method2.MakeGenericMethod(types)));
    }
    if (methodTrackerList.Count == 0)
      return (MethodGroup) null;
    MethodGroup methodGroup1 = new MethodGroup(methodTrackerList.ToArray());
    this.EnsureBoundGenericDict();
    lock (this._boundGenerics)
      this._boundGenerics[key] = methodGroup1;
    return methodGroup1;
  }

  private void EnsureBoundGenericDict()
  {
    if (this._boundGenerics != null)
      return;
    Interlocked.CompareExchange<Dictionary<MethodGroup.TypeList, MethodGroup>>(ref this._boundGenerics, new Dictionary<MethodGroup.TypeList, MethodGroup>(1), (Dictionary<MethodGroup.TypeList, MethodGroup>) null);
  }

  private class TypeList
  {
    private Type[] _types;

    public TypeList(Type[] types) => this._types = types;

    public override bool Equals(object obj)
    {
      if (!(obj is MethodGroup.TypeList typeList) || this._types.Length != typeList._types.Length)
        return false;
      for (int index = 0; index < this._types.Length; ++index)
      {
        if (this._types[index] != typeList._types[index])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = 6551;
      foreach (Type type in this._types)
        hashCode = hashCode << 5 ^ type.GetHashCode();
      return hashCode;
    }
  }
}
