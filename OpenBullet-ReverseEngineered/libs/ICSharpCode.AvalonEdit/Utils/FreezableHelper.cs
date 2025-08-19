// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.FreezableHelper
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class FreezableHelper
{
  public static void ThrowIfFrozen(IFreezable freezable)
  {
    if (freezable.IsFrozen)
      throw new InvalidOperationException("Cannot mutate frozen " + freezable.GetType().Name);
  }

  public static IList<T> FreezeListAndElements<T>(IList<T> list)
  {
    if (list != null)
    {
      foreach (T obj in (IEnumerable<T>) list)
        FreezableHelper.Freeze((object) obj);
    }
    return FreezableHelper.FreezeList<T>(list);
  }

  public static IList<T> FreezeList<T>(IList<T> list)
  {
    if (list == null || list.Count == 0)
      return (IList<T>) Empty<T>.Array;
    return list.IsReadOnly ? list : (IList<T>) new ReadOnlyCollection<T>((IList<T>) list.ToArray<T>());
  }

  public static void Freeze(object item)
  {
    if (!(item is IFreezable freezable))
      return;
    freezable.Freeze();
  }

  public static T FreezeAndReturn<T>(T item) where T : IFreezable
  {
    item.Freeze();
    return item;
  }

  public static T GetFrozenClone<T>(T item) where T : IFreezable, ICloneable
  {
    if (!item.IsFrozen)
    {
      item = (T) item.Clone();
      item.Freeze();
    }
    return item;
  }
}
