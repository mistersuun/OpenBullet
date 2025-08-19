// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.Assert
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal static class Assert
{
  [Conditional("DEBUG")]
  public static void NotNull(object var) => Debug.Assert(var != null);

  [Conditional("DEBUG")]
  public static void NotNull(object var1, object var2)
  {
    Debug.Assert(var1 != null && var2 != null);
  }

  [Conditional("DEBUG")]
  public static void NotNull(object var1, object var2, object var3)
  {
    Debug.Assert(var1 != null && var2 != null && var3 != null);
  }

  [Conditional("DEBUG")]
  public static void NotNullItems<T>(IEnumerable<T> items) where T : class
  {
    Debug.Assert(items != null);
    foreach (T obj in items)
      Debug.Assert((object) obj != null);
  }

  [Conditional("DEBUG")]
  public static void NotEmpty(string str) => Debug.Assert(!string.IsNullOrEmpty(str));
}
