// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.NamespaceTrackerOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class NamespaceTrackerOps
{
  [PropertyMethod]
  [SpecialName]
  public static object Get__file__(NamespaceTracker self)
  {
    if (self.PackageAssemblies.Count == 1)
      return (object) self.PackageAssemblies[0].FullName;
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < self.PackageAssemblies.Count; ++index)
    {
      if (index != 0)
        stringBuilder.Append(", ");
      stringBuilder.Append(self.PackageAssemblies[index].FullName);
    }
    return (object) stringBuilder.ToString();
  }

  public static string __repr__(NamespaceTracker self) => NamespaceTrackerOps.__str__(self);

  public static string __str__(NamespaceTracker self)
  {
    return self.PackageAssemblies.Count != 1 ? $"<module '{NamespaceTrackerOps.Get__name__(self.Name)}' (CLS module, {self.PackageAssemblies.Count} assemblies loaded)>" : $"<module '{NamespaceTrackerOps.Get__name__(self.Name)}' (CLS module from {self.PackageAssemblies[0].FullName})>";
  }

  [PropertyMethod]
  [SpecialName]
  public static PythonDictionary Get__dict__(CodeContext context, NamespaceTracker self)
  {
    PythonDictionary dict = new PythonDictionary();
    foreach (KeyValuePair<string, object> keyValuePair in self)
    {
      if (keyValuePair.Value is TypeGroup || keyValuePair.Value is NamespaceTracker)
        dict[(object) keyValuePair.Key] = keyValuePair.Value;
      else
        dict[(object) keyValuePair.Key] = (object) DynamicHelpers.GetPythonTypeFromType(((TypeTracker) keyValuePair.Value).Type);
    }
    return dict;
  }

  [PropertyMethod]
  [SpecialName]
  public static string Get__name__(CodeContext context, NamespaceTracker self)
  {
    return NamespaceTrackerOps.Get__name__(self.Name);
  }

  private static string Get__name__(string name)
  {
    int num = name.LastIndexOf('.');
    return num == -1 ? name : name.Substring(num + 1);
  }

  [SpecialName]
  public static object GetCustomMember(CodeContext context, NamespaceTracker self, string name)
  {
    MemberTracker customMember1;
    if (self.TryGetValue(name, out customMember1))
    {
      if (customMember1.MemberType == TrackerTypes.Namespace || customMember1.MemberType == TrackerTypes.TypeGroup)
        return (object) customMember1;
      PythonTypeSlot slot = PythonTypeOps.GetSlot(new MemberGroup(new MemberTracker[1]
      {
        customMember1
      }), name, context.LanguageContext.Binder.PrivateBinding);
      object customMember2;
      if (slot != null && slot.TryGetValue(context, (object) null, TypeCache.PythonType, out customMember2))
        return customMember2;
    }
    return (object) OperationFailed.Value;
  }
}
