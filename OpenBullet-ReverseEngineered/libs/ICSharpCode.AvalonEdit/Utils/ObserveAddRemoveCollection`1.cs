// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.ObserveAddRemoveCollection`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.ObjectModel;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class ObserveAddRemoveCollection<T> : Collection<T>
{
  private readonly Action<T> onAdd;
  private readonly Action<T> onRemove;

  public ObserveAddRemoveCollection(Action<T> onAdd, Action<T> onRemove)
  {
    if (onAdd == null)
      throw new ArgumentNullException(nameof (onAdd));
    if (onRemove == null)
      throw new ArgumentNullException(nameof (onRemove));
    this.onAdd = onAdd;
    this.onRemove = onRemove;
  }

  protected override void ClearItems()
  {
    if (this.onRemove != null)
    {
      foreach (T obj in (Collection<T>) this)
        this.onRemove(obj);
    }
    base.ClearItems();
  }

  protected override void InsertItem(int index, T item)
  {
    if (this.onAdd != null)
      this.onAdd(item);
    base.InsertItem(index, item);
  }

  protected override void RemoveItem(int index)
  {
    if (this.onRemove != null)
      this.onRemove(this[index]);
    base.RemoveItem(index);
  }

  protected override void SetItem(int index, T item)
  {
    if (this.onRemove != null)
      this.onRemove(this[index]);
    try
    {
      if (this.onAdd != null)
        this.onAdd(item);
    }
    catch
    {
      this.RemoveAt(index);
      throw;
    }
    base.SetItem(index, item);
  }
}
