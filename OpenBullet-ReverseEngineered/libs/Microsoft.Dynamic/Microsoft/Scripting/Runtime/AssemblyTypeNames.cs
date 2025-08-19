// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.AssemblyTypeNames
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Runtime;

internal static class AssemblyTypeNames
{
  public static IEnumerable<TypeName> GetTypeNames(Assembly assem, bool includePrivateTypes)
  {
    return ReflectionUtils.GetAllTypesFromAssembly(assem, includePrivateTypes).Where<Type>((Func<Type, bool>) (t => !t.IsNested)).Select<Type, TypeName>((Func<Type, TypeName>) (t => new TypeName(t)));
  }

  private static IEnumerable<TypeName> GetTypeNames(string[] namespaces, string[][] types)
  {
    for (int i = 0; i < namespaces.Length; ++i)
    {
      for (int j = 0; j < types[i].Length; ++j)
        yield return new TypeName(namespaces[i], types[i][j]);
    }
  }
}
