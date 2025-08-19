// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Extensions
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Xceed.Wpf.AvalonDock;

internal static class Extensions
{
  public static bool Contains(this IEnumerable collection, object item)
  {
    foreach (object obj in collection)
    {
      if (obj == item)
        return true;
    }
    return false;
  }

  public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
  {
    foreach (T obj in collection)
      action(obj);
  }

  public static int IndexOf<T>(this T[] array, T value) where T : class
  {
    for (int index = 0; index < array.Length; ++index)
    {
      if ((object) array[index] == (object) value)
        return index;
    }
    return -1;
  }

  public static V GetValueOrDefault<V>(this WeakReference wr)
  {
    return wr == null || !wr.IsAlive ? default (V) : (V) wr.Target;
  }
}
