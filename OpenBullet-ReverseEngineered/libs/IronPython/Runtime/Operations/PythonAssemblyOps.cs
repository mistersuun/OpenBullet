// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.PythonAssemblyOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class PythonAssemblyOps
{
  private static readonly object _key = new object();

  private static Dictionary<Assembly, TopNamespaceTracker> GetAssemblyMap(PythonContext context)
  {
    return context.GetOrCreateModuleState<Dictionary<Assembly, TopNamespaceTracker>>(PythonAssemblyOps._key, (Func<Dictionary<Assembly, TopNamespaceTracker>>) (() => new Dictionary<Assembly, TopNamespaceTracker>()));
  }

  [SpecialName]
  public static object GetBoundMember(CodeContext context, Assembly self, string name)
  {
    TopNamespaceTracker reflectedAssembly = PythonAssemblyOps.GetReflectedAssembly(context, self);
    if (name == "__dict__")
      return (object) new PythonDictionary((DictionaryStorage) new WrapperDictionaryStorage(reflectedAssembly));
    MemberTracker packageAny = reflectedAssembly.TryGetPackageAny(name);
    if (packageAny == null)
      return (object) OperationFailed.Value;
    return packageAny.MemberType == TrackerTypes.Type ? (object) DynamicHelpers.GetPythonTypeFromType(((TypeTracker) packageAny).Type) : (object) packageAny;
  }

  [SpecialName]
  public static List GetMemberNames(CodeContext context, Assembly self)
  {
    List memberNames = DynamicHelpers.GetPythonTypeFromType(self.GetType()).GetMemberNames(context);
    foreach (object key in (IEnumerable<string>) PythonAssemblyOps.GetReflectedAssembly(context, self).Keys)
    {
      if (key is string)
        memberNames.AddNoLock((object) (string) key);
    }
    return memberNames;
  }

  public static object __repr__(Assembly self) => (object) $"<Assembly {self.FullName}>";

  private static TopNamespaceTracker GetReflectedAssembly(CodeContext context, Assembly assem)
  {
    Dictionary<Assembly, TopNamespaceTracker> assemblyMap = PythonAssemblyOps.GetAssemblyMap(context.LanguageContext);
    lock (assemblyMap)
    {
      TopNamespaceTracker reflectedAssembly;
      if (assemblyMap.TryGetValue(assem, out reflectedAssembly))
        return reflectedAssembly;
      reflectedAssembly = new TopNamespaceTracker(context.LanguageContext.DomainManager);
      reflectedAssembly.LoadAssembly(assem);
      assemblyMap[assem] = reflectedAssembly;
      return reflectedAssembly;
    }
  }
}
