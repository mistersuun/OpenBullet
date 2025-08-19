// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.GeneratorOps
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;

#nullable disable
namespace Microsoft.Scripting.Generation;

public static class GeneratorOps
{
  public static object BoxGeneric<T>(T value)
  {
    Type type = typeof (T);
    if (type == typeof (int))
      return ScriptingRuntimeHelpers.Int32ToObject((int) (object) value);
    if (!(type == typeof (bool)))
      return (object) value;
    return !(bool) (object) value ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }
}
