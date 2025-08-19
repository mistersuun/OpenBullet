// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.TypeMemberCache`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Utils;

public sealed class TypeMemberCache<T> where T : MemberInfo
{
  private readonly ConditionalWeakTable<Type, Dictionary<string, List<T>>> _typeMembersByName = new ConditionalWeakTable<Type, Dictionary<string, List<T>>>();
  private readonly Func<Type, IEnumerable<T>> _reflector;

  private Dictionary<string, List<T>> GetMembers(Type type)
  {
    return this._typeMembersByName.GetValue(type, (ConditionalWeakTable<Type, Dictionary<string, List<T>>>.CreateValueCallback) (t => this.ReflectMembers(t)));
  }

  public TypeMemberCache(Func<Type, IEnumerable<T>> reflector) => this._reflector = reflector;

  public IEnumerable<T> GetMembers(Type type, string name = null, bool inherited = false)
  {
    Dictionary<string, List<T>> members = this.GetMembers(type);
    if (name == null)
    {
      IEnumerable<T> source = members.Values.SelectMany<List<T>, T>((Func<List<T>, IEnumerable<T>>) (memberList => (IEnumerable<T>) memberList));
      return inherited ? source : source.Where<T>((Func<T, bool>) (overload => overload.DeclaringType == type));
    }
    List<T> objList;
    if (!members.TryGetValue(name, out objList))
      return Enumerable.Empty<T>();
    return inherited ? (IEnumerable<T>) new ReadOnlyCollection<T>((IList<T>) objList) : objList.Where<T>((Func<T, bool>) (overload => overload.DeclaringType == type));
  }

  private Dictionary<string, List<T>> ReflectMembers(Type type)
  {
    Dictionary<string, List<T>> dictionary = new Dictionary<string, List<T>>();
    foreach (T obj in this._reflector(type))
    {
      List<T> objList;
      if (!dictionary.TryGetValue(obj.Name, out objList))
        dictionary.Add(obj.Name, objList = new List<T>());
      objList.Add(obj);
    }
    foreach (List<T> objList in dictionary.Values)
      objList.TrimExcess();
    return dictionary;
  }
}
