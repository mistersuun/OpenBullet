// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.NamespaceTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class NamespaceTracker : 
  MemberTracker,
  IMembersList,
  IEnumerable<KeyValuePair<string, object>>,
  IEnumerable
{
  internal Dictionary<string, MemberTracker> _dict = new Dictionary<string, MemberTracker>();
  internal readonly List<Assembly> _packageAssemblies = new List<Assembly>();
  internal readonly Dictionary<Assembly, NamespaceTracker.TypeNames> _typeNames = new Dictionary<Assembly, NamespaceTracker.TypeNames>();
  private readonly string _fullName;
  private TopNamespaceTracker _topPackage;
  private int _id;
  private static int _masterId;

  protected NamespaceTracker(string name)
  {
    this.UpdateId();
    this._fullName = name;
  }

  public override string ToString() => $"{base.ToString()}:{this._fullName}";

  internal NamespaceTracker GetOrMakeChildPackage(string childName, Assembly assem)
  {
    MemberTracker memberTracker;
    if (!this._dict.TryGetValue(childName, out memberTracker) || !(memberTracker is NamespaceTracker makeChildPackage))
      return this.MakeChildPackage(childName, assem);
    if (!makeChildPackage._packageAssemblies.Contains(assem))
    {
      makeChildPackage._packageAssemblies.Add(assem);
      makeChildPackage.UpdateSubtreeIds();
    }
    return makeChildPackage;
  }

  private NamespaceTracker MakeChildPackage(string childName, Assembly assem)
  {
    NamespaceTracker namespaceTracker = new NamespaceTracker(this.GetFullChildName(childName));
    namespaceTracker.SetTopPackage(this._topPackage);
    namespaceTracker._packageAssemblies.Add(assem);
    this._dict[childName] = (MemberTracker) namespaceTracker;
    return namespaceTracker;
  }

  private string GetFullChildName(string childName)
  {
    return this._fullName == null ? childName : $"{this._fullName}.{childName}";
  }

  private static Type LoadType(Assembly assem, string fullTypeName) => assem.GetType(fullTypeName);

  internal void AddTypeName(string typeName, Assembly assem)
  {
    if (!this._typeNames.ContainsKey(assem))
      this._typeNames[assem] = new NamespaceTracker.TypeNames(assem, this._fullName);
    this._typeNames[assem].AddTypeName(typeName);
    string normalizedTypeName = ReflectionUtils.GetNormalizedTypeName(typeName);
    if (!this._dict.ContainsKey(normalizedTypeName))
      return;
    Type type = NamespaceTracker.LoadType(assem, this.GetFullChildName(typeName));
    if (!(type != (Type) null))
      return;
    if (!(this._dict[normalizedTypeName] is TypeTracker existingTypeEntity))
      this._dict[normalizedTypeName] = MemberTracker.FromMemberInfo((MemberInfo) type);
    else
      this._dict[normalizedTypeName] = (MemberTracker) TypeGroup.UpdateTypeEntity(existingTypeEntity, TypeTracker.GetTypeTracker(type));
  }

  private void LoadAllTypes()
  {
    foreach (NamespaceTracker.TypeNames typeNames in this._typeNames.Values)
    {
      foreach (string normalizedTypeName in (IEnumerable<string>) typeNames.GetNormalizedTypeNames())
      {
        if (!this.TryGetValue(normalizedTypeName, out object _))
          throw new TypeLoadException(normalizedTypeName);
      }
    }
  }

  public override string Name => this._fullName;

  protected void DiscoverAllTypes(Assembly assem)
  {
    NamespaceTracker namespaceTracker1 = (NamespaceTracker) null;
    string empty = string.Empty;
    foreach (TypeName typeName in AssemblyTypeNames.GetTypeNames(assem, this._topPackage.DomainManager.Configuration.PrivateBinding))
    {
      NamespaceTracker namespaceTracker2;
      if (typeName.Namespace == empty)
      {
        namespaceTracker2 = namespaceTracker1;
      }
      else
      {
        namespaceTracker2 = this.GetOrMakePackageHierarchy(assem, typeName.Namespace);
        empty = typeName.Namespace;
        namespaceTracker1 = namespaceTracker2;
      }
      namespaceTracker2.AddTypeName(typeName.Name, assem);
    }
  }

  private NamespaceTracker GetOrMakePackageHierarchy(Assembly assem, string fullNamespace)
  {
    if (fullNamespace == null)
      return this;
    NamespaceTracker packageHierarchy = this;
    string str = fullNamespace;
    char[] chArray = new char[1]{ '.' };
    foreach (string childName in str.Split(chArray))
      packageHierarchy = packageHierarchy.GetOrMakeChildPackage(childName, assem);
    return packageHierarchy;
  }

  private MemberTracker CheckForUnlistedType(string nameString)
  {
    string fullChildName = this.GetFullChildName(nameString);
    foreach (Assembly packageAssembly in this._packageAssemblies)
    {
      Type type = packageAssembly.GetType(fullChildName);
      if (!(type == (Type) null) && !type.IsNested() && (type.IsPublic() ? 1 : (this._topPackage.DomainManager.Configuration.PrivateBinding ? 1 : 0)) != 0)
        return (MemberTracker) TypeTracker.GetTypeTracker(type);
    }
    return (MemberTracker) null;
  }

  public bool TryGetValue(string name, out object value)
  {
    MemberTracker memberTracker;
    int num = this.TryGetValue(name, out memberTracker) ? 1 : 0;
    value = (object) memberTracker;
    return num != 0;
  }

  public bool TryGetValue(string name, out MemberTracker value)
  {
    lock (this._topPackage.HierarchyLock)
    {
      this.LoadNamespaces();
      if (this._dict.TryGetValue(name, out value))
        return true;
      MemberTracker existingTypeEntity = (MemberTracker) null;
      if (name.IndexOf('.') != -1)
      {
        value = (MemberTracker) null;
        return false;
      }
      foreach (KeyValuePair<Assembly, NamespaceTracker.TypeNames> typeName in this._typeNames)
      {
        if (typeName.Value.Contains(name))
          existingTypeEntity = typeName.Value.UpdateTypeEntity((TypeTracker) existingTypeEntity, name);
      }
      if (existingTypeEntity == null)
        existingTypeEntity = this.CheckForUnlistedType(name);
      if (existingTypeEntity == null)
        return false;
      this._dict[name] = existingTypeEntity;
      value = existingTypeEntity;
      return true;
    }
  }

  public bool ContainsKey(string name) => this.TryGetValue(name, out object _);

  public object this[string name]
  {
    get
    {
      object obj;
      if (this.TryGetValue(name, out obj))
        return obj;
      throw new KeyNotFoundException();
    }
  }

  public int Count => this._dict.Count;

  public ICollection<string> Keys
  {
    get
    {
      this.LoadNamespaces();
      lock (this._topPackage.HierarchyLock)
        return (ICollection<string>) this.AddKeys((IList) new List<string>());
    }
  }

  private IList AddKeys(IList res)
  {
    foreach (string key in this._dict.Keys)
      res.Add((object) key);
    foreach (KeyValuePair<Assembly, NamespaceTracker.TypeNames> typeName in this._typeNames)
    {
      foreach (string normalizedTypeName in (IEnumerable<string>) typeName.Value.GetNormalizedTypeNames())
      {
        if (!res.Contains((object) normalizedTypeName))
          res.Add((object) normalizedTypeName);
      }
    }
    return res;
  }

  public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
  {
    foreach (string key in (IEnumerable<string>) this.Keys)
      yield return new KeyValuePair<string, object>(key, this[key]);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    foreach (string key in (IEnumerable<string>) this.Keys)
      yield return (object) new KeyValuePair<string, object>(key, this[key]);
  }

  public IList<Assembly> PackageAssemblies
  {
    get
    {
      this.LoadNamespaces();
      return (IList<Assembly>) this._packageAssemblies;
    }
  }

  protected virtual void LoadNamespaces() => this._topPackage?.LoadNamespaces();

  protected void SetTopPackage(TopNamespaceTracker pkg) => this._topPackage = pkg;

  public int Id => this._id;

  public IList<string> GetMemberNames()
  {
    this.LoadNamespaces();
    lock (this._topPackage.HierarchyLock)
    {
      List<string> res = new List<string>();
      this.AddKeys((IList) res);
      res.Sort();
      return (IList<string>) res;
    }
  }

  public override TrackerTypes MemberType => TrackerTypes.Namespace;

  public override Type DeclaringType => (Type) null;

  private void UpdateId() => this._id = Interlocked.Increment(ref NamespaceTracker._masterId);

  protected void UpdateSubtreeIds()
  {
    this.UpdateId();
    foreach (KeyValuePair<string, MemberTracker> keyValuePair in this._dict)
    {
      if (keyValuePair.Value is NamespaceTracker namespaceTracker)
        namespaceTracker.UpdateSubtreeIds();
    }
  }

  internal class TypeNames
  {
    private List<string> _simpleTypeNames = new List<string>();
    private Dictionary<string, List<string>> _genericTypeNames = new Dictionary<string, List<string>>();
    private readonly Assembly _assembly;
    private readonly string _fullNamespace;

    internal TypeNames(Assembly assembly, string fullNamespace)
    {
      this._assembly = assembly;
      this._fullNamespace = fullNamespace;
    }

    internal bool Contains(string normalizedTypeName)
    {
      return this._simpleTypeNames.Contains(normalizedTypeName) || this._genericTypeNames.ContainsKey(normalizedTypeName);
    }

    internal MemberTracker UpdateTypeEntity(
      TypeTracker existingTypeEntity,
      string normalizedTypeName)
    {
      if (this._simpleTypeNames.Contains(normalizedTypeName))
      {
        Type type = NamespaceTracker.LoadType(this._assembly, this.GetFullChildName(normalizedTypeName));
        if (type != (Type) null)
          existingTypeEntity = TypeGroup.UpdateTypeEntity(existingTypeEntity, TypeTracker.GetTypeTracker(type));
      }
      if (this._genericTypeNames.ContainsKey(normalizedTypeName))
      {
        foreach (string childName in this._genericTypeNames[normalizedTypeName])
        {
          Type type = NamespaceTracker.LoadType(this._assembly, this.GetFullChildName(childName));
          if (type != (Type) null)
            existingTypeEntity = TypeGroup.UpdateTypeEntity(existingTypeEntity, TypeTracker.GetTypeTracker(type));
        }
      }
      return (MemberTracker) existingTypeEntity;
    }

    internal void AddTypeName(string typeName)
    {
      string normalizedTypeName = ReflectionUtils.GetNormalizedTypeName(typeName);
      if (normalizedTypeName == typeName)
      {
        this._simpleTypeNames.Add(typeName);
      }
      else
      {
        List<string> stringList;
        if (this._genericTypeNames.ContainsKey(normalizedTypeName))
        {
          stringList = this._genericTypeNames[normalizedTypeName];
        }
        else
        {
          stringList = new List<string>();
          this._genericTypeNames[normalizedTypeName] = stringList;
        }
        stringList.Add(typeName);
      }
    }

    private string GetFullChildName(string childName)
    {
      return this._fullNamespace == null ? childName : $"{this._fullNamespace}.{childName}";
    }

    internal ICollection<string> GetNormalizedTypeNames()
    {
      List<string> normalizedTypeNames = new List<string>();
      normalizedTypeNames.AddRange((IEnumerable<string>) this._simpleTypeNames);
      normalizedTypeNames.AddRange((IEnumerable<string>) this._genericTypeNames.Keys);
      return (ICollection<string>) normalizedTypeNames;
    }
  }
}
