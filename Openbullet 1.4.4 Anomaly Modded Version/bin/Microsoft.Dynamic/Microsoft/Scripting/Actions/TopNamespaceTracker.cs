// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.TopNamespaceTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class TopNamespaceTracker : NamespaceTracker
{
  private int _lastDiscovery;
  internal readonly object HierarchyLock;
  private static Dictionary<Guid, Type> _comTypeCache = new Dictionary<Guid, Type>();

  public TopNamespaceTracker(ScriptDomainManager manager)
    : base((string) null)
  {
    ContractUtils.RequiresNotNull((object) manager, nameof (manager));
    this.SetTopPackage(this);
    this.DomainManager = manager;
    this.HierarchyLock = new object();
  }

  public NamespaceTracker TryGetPackage(string name)
  {
    return this.TryGetPackageAny(name) is NamespaceTracker packageAny ? packageAny : (NamespaceTracker) null;
  }

  public MemberTracker TryGetPackageAny(string name)
  {
    MemberTracker memberTracker;
    return this.TryGetValue(name, out memberTracker) ? memberTracker : (MemberTracker) null;
  }

  public MemberTracker TryGetPackageLazy(string name)
  {
    lock (this.HierarchyLock)
    {
      MemberTracker memberTracker;
      return this._dict.TryGetValue(name, out memberTracker) ? memberTracker : (MemberTracker) null;
    }
  }

  public bool LoadAssembly(Assembly assem)
  {
    ContractUtils.RequiresNotNull((object) assem, nameof (assem));
    lock (this.HierarchyLock)
    {
      if (this._packageAssemblies.Contains(assem))
        return false;
      this._packageAssemblies.Add(assem);
      this.UpdateSubtreeIds();
      TopNamespaceTracker.PublishComTypes(assem);
    }
    return true;
  }

  public static void PublishComTypes(Assembly interopAssembly)
  {
    lock (TopNamespaceTracker._comTypeCache)
    {
      foreach (Type type1 in ReflectionUtils.GetAllTypesFromAssembly(interopAssembly, false))
      {
        if (type1.IsImport && type1.IsInterface)
        {
          Type type2;
          if (TopNamespaceTracker._comTypeCache.TryGetValue(type1.GUID, out type2))
          {
            if (!type2.IsDefined(typeof (CoClassAttribute), false))
              TopNamespaceTracker._comTypeCache[type1.GUID] = type1;
          }
          else
            TopNamespaceTracker._comTypeCache[type1.GUID] = type1;
        }
      }
    }
  }

  protected override void LoadNamespaces()
  {
    lock (this.HierarchyLock)
    {
      for (int lastDiscovery = this._lastDiscovery; lastDiscovery < this._packageAssemblies.Count; ++lastDiscovery)
        this.DiscoverAllTypes(this._packageAssemblies[lastDiscovery]);
      this._lastDiscovery = this._packageAssemblies.Count;
    }
  }

  public ScriptDomainManager DomainManager { get; }
}
