// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.Assert
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class Assert
{
  public static Exception Unreachable
  {
    get
    {
      Debug.Assert(false, nameof (Unreachable));
      return (Exception) new InvalidOperationException("Code supposed to be unreachable");
    }
  }

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
  public static void NotNull(object var1, object var2, object var3, object var4)
  {
    Debug.Assert(var1 != null && var2 != null && var3 != null && var4 != null);
  }

  [Conditional("DEBUG")]
  public static void NotEmpty(string str) => Debug.Assert(!string.IsNullOrEmpty(str));

  [Conditional("DEBUG")]
  public static void NotEmpty<T>(ICollection<T> array)
  {
    Debug.Assert(array != null && array.Count > 0);
  }

  [Conditional("DEBUG")]
  public static void NotNullItems<T>(IEnumerable<T> items) where T : class
  {
    Debug.Assert(items != null);
    foreach (T obj in items)
      Debug.Assert((object) obj != null);
  }

  [Conditional("DEBUG")]
  public static void IsTrue(Func<bool> predicate)
  {
    ContractUtils.RequiresNotNull((object) predicate, nameof (predicate));
    Debug.Assert(predicate());
  }
}
