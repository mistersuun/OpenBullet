// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.TypeTrackerOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using System.Collections;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class TypeTrackerOps
{
  [PropertyMethod]
  [SpecialName]
  public static IDictionary Get__dict__(CodeContext context, TypeTracker self)
  {
    return (IDictionary) new DictProxy(DynamicHelpers.GetPythonTypeFromType(self.Type));
  }
}
