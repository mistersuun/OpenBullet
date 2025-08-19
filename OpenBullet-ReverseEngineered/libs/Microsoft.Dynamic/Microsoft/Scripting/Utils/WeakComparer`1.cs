// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.WeakComparer`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

internal sealed class WeakComparer<T> : IEqualityComparer<T>
{
  bool IEqualityComparer<T>.Equals(T x, T y)
  {
    if (x is WeakObject weakObject1)
      x = (T) weakObject1.Target;
    if (y is WeakObject weakObject2)
      y = (T) weakObject2.Target;
    return object.Equals((object) x, (object) y);
  }

  int IEqualityComparer<T>.GetHashCode(T obj)
  {
    if (obj is WeakObject weakObject)
      return weakObject.GetHashCode();
    return (object) obj != null ? ReferenceEqualityComparer<object>.Instance.GetHashCode((object) obj) : 0;
  }
}
