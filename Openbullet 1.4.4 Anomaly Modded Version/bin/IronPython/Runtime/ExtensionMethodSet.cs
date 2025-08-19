// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ExtensionMethodSet
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

internal sealed class ExtensionMethodSet : IEquatable<ExtensionMethodSet>
{
  private PythonExtensionBinder _extBinder;
  private Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> _loadedAssemblies;
  private int _id;
  private static int _curId;
  public static readonly ExtensionMethodSet Empty = new ExtensionMethodSet();
  public const int OutOfIds = -2147483648 /*0x80000000*/;

  private ExtensionMethodSet(
    Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> dict)
  {
    this._loadedAssemblies = dict;
    if (ExtensionMethodSet._curId >= 0 && (this._id = Interlocked.Increment(ref ExtensionMethodSet._curId)) >= 0)
      return;
    this._id = int.MinValue;
  }

  public BindingRestrictions GetRestriction(Expression codeContext)
  {
    return this._id != int.MinValue ? BindingRestrictions.GetExpressionRestriction((Expression) Expression.Call(typeof (PythonOps).GetMethod("IsExtensionSet"), codeContext, (Expression) Expression.Constant((object) this._id))) : BindingRestrictions.GetInstanceRestriction((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetExtensionMethodSet"), codeContext), (object) this);
  }

  private ExtensionMethodSet()
  {
    this._loadedAssemblies = new Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo>();
  }

  public IEnumerable<MethodInfo> GetExtensionMethods(string name)
  {
    ExtensionMethodSet extensionMethodSet = this;
    lock (extensionMethodSet)
    {
      extensionMethodSet.EnsureLoaded();
      foreach (KeyValuePair<Assembly, ExtensionMethodSet.AssemblyLoadInfo> loadedAssembly in extensionMethodSet._loadedAssemblies)
      {
        foreach (PythonType type in loadedAssembly.Value.Types)
        {
          List<MethodInfo> methodInfoList;
          if (type.ExtensionMethods.TryGetValue(name, out methodInfoList))
          {
            foreach (MethodInfo extensionMethod in methodInfoList)
              yield return extensionMethod;
          }
        }
      }
    }
  }

  private void EnsureLoaded()
  {
    bool flag = false;
    foreach (ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo in this._loadedAssemblies.Values)
    {
      if (assemblyLoadInfo.Namespaces != null || assemblyLoadInfo.Types == null)
        flag = true;
    }
    if (!flag)
      return;
    this.LoadAllTypes();
  }

  public IEnumerable<MethodInfo> GetExtensionMethods(PythonType type)
  {
    ExtensionMethodSet extensionMethodSet = this;
    lock (extensionMethodSet)
    {
      extensionMethodSet.EnsureLoaded();
      foreach (KeyValuePair<Assembly, ExtensionMethodSet.AssemblyLoadInfo> loadedAssembly in extensionMethodSet._loadedAssemblies)
      {
        foreach (PythonType type1 in loadedAssembly.Value.Types)
        {
          foreach (List<MethodInfo> methodInfoList in type1.ExtensionMethods.Values)
          {
            foreach (MethodInfo extensionMethod in methodInfoList)
            {
              ParameterInfo[] parameters = extensionMethod.GetParameters();
              if (parameters.Length != 0 && PythonExtensionBinder.IsApplicableExtensionMethod(type.UnderlyingSystemType, parameters[0].ParameterType))
                yield return extensionMethod;
            }
          }
        }
      }
    }
  }

  private void LoadAllTypes()
  {
    Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> dictionary = new Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo>(this._loadedAssemblies.Count);
    foreach (KeyValuePair<Assembly, ExtensionMethodSet.AssemblyLoadInfo> loadedAssembly in this._loadedAssemblies)
    {
      ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo = loadedAssembly.Value;
      Assembly key = loadedAssembly.Key;
      dictionary[key] = assemblyLoadInfo.EnsureTypesLoaded();
    }
    this._loadedAssemblies = dictionary;
  }

  public static ExtensionMethodSet AddType(
    PythonContext context,
    ExtensionMethodSet existingSet,
    PythonType type)
  {
    lock (existingSet)
    {
      Assembly assembly = type.UnderlyingSystemType.Assembly;
      ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo;
      if (existingSet._loadedAssemblies.TryGetValue(assembly, out assemblyLoadInfo) && (assemblyLoadInfo.IsFullAssemblyLoaded || assemblyLoadInfo.Types != null && assemblyLoadInfo.Types.Contains(type) || assemblyLoadInfo.Namespaces != null && assemblyLoadInfo.Namespaces.Contains(type.UnderlyingSystemType.Namespace)))
        return existingSet;
      Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> dict = ExtensionMethodSet.NewInfoOrCopy(existingSet);
      if (!dict.TryGetValue(assembly, out assemblyLoadInfo))
        dict[assembly] = assemblyLoadInfo = new ExtensionMethodSet.AssemblyLoadInfo(assembly);
      if (assemblyLoadInfo.Types == null)
        assemblyLoadInfo.Types = new HashSet<PythonType>();
      assemblyLoadInfo.Types.Add(type);
      return context.UniqifyExtensions(new ExtensionMethodSet(dict));
    }
  }

  public static ExtensionMethodSet AddNamespace(
    PythonContext context,
    ExtensionMethodSet existingSet,
    NamespaceTracker ns)
  {
    lock (existingSet)
    {
      Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> dict = (Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo>) null;
      foreach (Assembly packageAssembly in (IEnumerable<Assembly>) ns.PackageAssemblies)
      {
        ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo1;
        if (existingSet != (ExtensionMethodSet) null && existingSet._loadedAssemblies.TryGetValue(packageAssembly, out assemblyLoadInfo1))
        {
          if (!assemblyLoadInfo1.IsFullAssemblyLoaded && (assemblyLoadInfo1.Namespaces == null || !assemblyLoadInfo1.Namespaces.Contains(ns.Name)))
          {
            if (dict == null)
              dict = ExtensionMethodSet.NewInfoOrCopy(existingSet);
            if (dict[packageAssembly].Namespaces == null)
              dict[packageAssembly].Namespaces = new HashSet<string>();
            dict[packageAssembly].Namespaces.Add(ns.Name);
          }
        }
        else
        {
          if (dict == null)
            dict = ExtensionMethodSet.NewInfoOrCopy(existingSet);
          ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo2 = dict[packageAssembly] = new ExtensionMethodSet.AssemblyLoadInfo(packageAssembly);
          assemblyLoadInfo2.Namespaces = new HashSet<string>();
          assemblyLoadInfo2.Namespaces.Add(ns.Name);
        }
      }
      return dict != null ? context.UniqifyExtensions(new ExtensionMethodSet(dict)) : existingSet;
    }
  }

  public static bool operator ==(ExtensionMethodSet set1, ExtensionMethodSet set2)
  {
    return (object) set1 != null ? set1.Equals(set2) : (object) set2 == null;
  }

  public static bool operator !=(ExtensionMethodSet set1, ExtensionMethodSet set2)
  {
    return !(set1 == set2);
  }

  public PythonExtensionBinder GetBinder(PythonContext context)
  {
    if (this._extBinder == null)
      this._extBinder = new PythonExtensionBinder(context.Binder, this);
    return this._extBinder;
  }

  private static Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> NewInfoOrCopy(
    ExtensionMethodSet existingSet)
  {
    Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo> dictionary = new Dictionary<Assembly, ExtensionMethodSet.AssemblyLoadInfo>();
    if (existingSet != (ExtensionMethodSet) null)
    {
      foreach (KeyValuePair<Assembly, ExtensionMethodSet.AssemblyLoadInfo> loadedAssembly in existingSet._loadedAssemblies)
      {
        ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo = new ExtensionMethodSet.AssemblyLoadInfo(loadedAssembly.Key);
        if (loadedAssembly.Value.Namespaces != null)
          assemblyLoadInfo.Namespaces = new HashSet<string>((IEnumerable<string>) loadedAssembly.Value.Namespaces);
        if (loadedAssembly.Value.Types != null)
          assemblyLoadInfo.Types = new HashSet<PythonType>((IEnumerable<PythonType>) loadedAssembly.Value.Types);
        dictionary[loadedAssembly.Key] = assemblyLoadInfo;
      }
    }
    return dictionary;
  }

  public int Id => this._id;

  public override bool Equals(object obj)
  {
    ExtensionMethodSet other = obj as ExtensionMethodSet;
    return other != (ExtensionMethodSet) null && this.Equals(other);
  }

  public bool Equals(ExtensionMethodSet other)
  {
    if (other == (ExtensionMethodSet) null)
      return false;
    if ((object) this == (object) other)
      return true;
    if (this._loadedAssemblies.Count != other._loadedAssemblies.Count)
      return false;
    foreach (KeyValuePair<Assembly, ExtensionMethodSet.AssemblyLoadInfo> loadedAssembly in this._loadedAssemblies)
    {
      Assembly key = loadedAssembly.Key;
      ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo1 = loadedAssembly.Value;
      ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo2;
      if (!other._loadedAssemblies.TryGetValue(key, out assemblyLoadInfo2) || assemblyLoadInfo2 != assemblyLoadInfo1)
        return false;
    }
    return true;
  }

  public override int GetHashCode()
  {
    int hashCode = 6551;
    foreach (Assembly key in this._loadedAssemblies.Keys)
      hashCode ^= key.GetHashCode();
    return hashCode;
  }

  private sealed class AssemblyLoadInfo : IEquatable<ExtensionMethodSet.AssemblyLoadInfo>
  {
    private static IEqualityComparer<HashSet<PythonType>> TypeComparer = CollectionUtils.CreateSetComparer<PythonType>();
    private static IEqualityComparer<HashSet<string>> StringComparer = CollectionUtils.CreateSetComparer<string>();
    public HashSet<PythonType> Types;
    public HashSet<string> Namespaces;
    public bool IsFullAssemblyLoaded;
    private readonly Assembly _asm;

    public AssemblyLoadInfo(Assembly asm) => this._asm = asm;

    public override int GetHashCode() => this._asm != (Assembly) null ? this._asm.GetHashCode() : 0;

    public override bool Equals(object obj)
    {
      return obj is ExtensionMethodSet.AssemblyLoadInfo other && this.Equals(other);
    }

    public ExtensionMethodSet.AssemblyLoadInfo EnsureTypesLoaded()
    {
      if (this.Namespaces == null && this.Types != null)
        return this;
      HashSet<PythonType> pythonTypeSet = new HashSet<PythonType>();
      HashSet<string> namespaces = this.Namespaces;
      foreach (Type exportedType in this._asm.GetExportedTypes())
      {
        if (exportedType.IsExtension() && namespaces != null && namespaces.Contains(exportedType.Namespace))
          pythonTypeSet.Add(DynamicHelpers.GetPythonTypeFromType(exportedType));
      }
      ExtensionMethodSet.AssemblyLoadInfo assemblyLoadInfo = new ExtensionMethodSet.AssemblyLoadInfo(this._asm);
      assemblyLoadInfo.Types = pythonTypeSet;
      if (namespaces == null)
        assemblyLoadInfo.IsFullAssemblyLoaded = true;
      return assemblyLoadInfo;
    }

    public bool Equals(ExtensionMethodSet.AssemblyLoadInfo other)
    {
      if (this == other)
        return true;
      if (other == null || this._asm != other._asm)
        return false;
      if (this.IsFullAssemblyLoaded && other.IsFullAssemblyLoaded)
        return true;
      return ExtensionMethodSet.AssemblyLoadInfo.TypeComparer.Equals(this.Types, other.Types) && ExtensionMethodSet.AssemblyLoadInfo.StringComparer.Equals(this.Namespaces, other.Namespaces);
    }
  }
}
