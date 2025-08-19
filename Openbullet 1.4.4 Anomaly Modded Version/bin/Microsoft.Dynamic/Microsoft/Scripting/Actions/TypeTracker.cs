// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.TypeTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions;

public abstract class TypeTracker : MemberTracker, IMembersList
{
  private static readonly Dictionary<Type, TypeTracker> _typeCache = new Dictionary<Type, TypeTracker>();

  internal TypeTracker()
  {
  }

  public abstract Type Type { get; }

  public abstract bool IsGenericType { get; }

  public abstract bool IsPublic { get; }

  public virtual IList<string> GetMemberNames()
  {
    HashSet<string> stringSet = new HashSet<string>();
    TypeTracker.GetMemberNames(this.Type, stringSet);
    return (IList<string>) stringSet.ToArray<string>();
  }

  internal static void GetMemberNames(Type type, HashSet<string> result)
  {
    foreach (Type ancestor in type.Ancestors())
    {
      foreach (MemberInfo declaredMember in ancestor.GetDeclaredMembers())
      {
        if ((object) (declaredMember as ConstructorInfo) == null)
          result.Add(declaredMember.Name);
      }
    }
  }

  public static explicit operator Type(TypeTracker tracker)
  {
    if (!(tracker is TypeGroup typeGroup))
      return tracker.Type;
    Type nonGenericType;
    if (!typeGroup.TryGetNonGenericType(out nonGenericType))
      throw ScriptingRuntimeHelpers.SimpleTypeError("expected non-generic type, got generic-only type");
    return nonGenericType;
  }

  public static TypeTracker GetTypeTracker(Type type)
  {
    TypeTracker typeTracker;
    lock (TypeTracker._typeCache)
    {
      if (!TypeTracker._typeCache.TryGetValue(type, out typeTracker))
        TypeTracker._typeCache[type] = typeTracker = (TypeTracker) new NestedTypeTracker(type);
    }
    return typeTracker;
  }
}
