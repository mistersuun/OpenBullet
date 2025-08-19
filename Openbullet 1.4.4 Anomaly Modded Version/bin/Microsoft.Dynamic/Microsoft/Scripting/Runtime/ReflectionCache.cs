// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ReflectionCache
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class ReflectionCache
{
  private static readonly Dictionary<ReflectionCache.MethodBaseCache, MethodGroup> _functions = new Dictionary<ReflectionCache.MethodBaseCache, MethodGroup>();
  private static readonly Dictionary<Type, TypeTracker> _typeCache = new Dictionary<Type, TypeTracker>();

  public static MethodGroup GetMethodGroup(string name, MethodBase[] methods)
  {
    MethodGroup methodGroup1 = (MethodGroup) null;
    ReflectionCache.MethodBaseCache key1 = new ReflectionCache.MethodBaseCache(name, methods);
    lock (ReflectionCache._functions)
    {
      if (!ReflectionCache._functions.TryGetValue(key1, out methodGroup1))
      {
        Dictionary<ReflectionCache.MethodBaseCache, MethodGroup> functions = ReflectionCache._functions;
        ReflectionCache.MethodBaseCache key2 = key1;
        MethodBase[] input = methods;
        MethodGroup methodGroup2;
        methodGroup1 = methodGroup2 = new MethodGroup(ArrayUtils.ConvertAll<MethodBase, MethodTracker>(input, (Func<MethodBase, MethodTracker>) (x => (MethodTracker) MemberTracker.FromMemberInfo((MemberInfo) x))));
        functions[key2] = methodGroup2;
      }
    }
    return methodGroup1;
  }

  public static MethodGroup GetMethodGroup(string name, MemberGroup mems)
  {
    MethodGroup methodGroup = (MethodGroup) null;
    MethodBase[] members = new MethodBase[mems.Count];
    MethodTracker[] methodTrackerArray = new MethodTracker[mems.Count];
    for (int index = 0; index < members.Length; ++index)
    {
      methodTrackerArray[index] = (MethodTracker) mems[index];
      members[index] = (MethodBase) methodTrackerArray[index].Method;
    }
    if (mems.Count != 0)
    {
      ReflectionCache.MethodBaseCache key = new ReflectionCache.MethodBaseCache(name, members);
      lock (ReflectionCache._functions)
      {
        if (!ReflectionCache._functions.TryGetValue(key, out methodGroup))
          ReflectionCache._functions[key] = methodGroup = new MethodGroup(methodTrackerArray);
      }
    }
    return methodGroup;
  }

  public static TypeTracker GetTypeTracker(Type type)
  {
    TypeTracker typeTracker;
    lock (ReflectionCache._typeCache)
    {
      if (!ReflectionCache._typeCache.TryGetValue(type, out typeTracker))
        ReflectionCache._typeCache[type] = typeTracker = (TypeTracker) new NestedTypeTracker(type);
    }
    return typeTracker;
  }

  public class MethodBaseCache
  {
    private readonly MethodBase[] _members;
    private readonly string _name;

    public MethodBaseCache(string name, MethodBase[] members)
    {
      Array.Sort<MethodBase>(members, new Comparison<MethodBase>(ReflectionCache.MethodBaseCache.CompareMethods));
      this._name = name;
      this._members = members;
    }

    private static int CompareMethods(MethodBase x, MethodBase y)
    {
      Module module1 = x.Module;
      Module module2 = y.Module;
      return module1 == module2 ? x.MetadataToken - y.MetadataToken : module1.ModuleVersionId.CompareTo(module2.ModuleVersionId);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is ReflectionCache.MethodBaseCache methodBaseCache) || this._members.Length != methodBaseCache._members.Length || methodBaseCache._name != this._name)
        return false;
      for (int index1 = 0; index1 < this._members.Length; ++index1)
      {
        if (this._members[index1].DeclaringType != methodBaseCache._members[index1].DeclaringType || this._members[index1].MetadataToken != methodBaseCache._members[index1].MetadataToken || this._members[index1].IsGenericMethod != methodBaseCache._members[index1].IsGenericMethod)
          return false;
        if (this._members[index1].IsGenericMethod)
        {
          Type[] genericArguments1 = this._members[index1].GetGenericArguments();
          Type[] genericArguments2 = methodBaseCache._members[index1].GetGenericArguments();
          if (genericArguments1.Length != genericArguments2.Length)
            return false;
          for (int index2 = 0; index2 < genericArguments1.Length; ++index2)
          {
            if (genericArguments1[index2] != genericArguments2[index2])
              return false;
          }
        }
      }
      return true;
    }

    public override int GetHashCode()
    {
      int num = 6551;
      foreach (MemberInfo member in this._members)
        num ^= num << 5 ^ member.DeclaringType.GetHashCode() ^ member.MetadataToken;
      return num ^ this._name.GetHashCode();
    }
  }
}
