// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.DefinitionCollectionBase`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.ObjectModel;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public abstract class DefinitionCollectionBase<T> : ObservableCollection<T> where T : DefinitionBase
{
  internal DefinitionCollectionBase()
  {
  }

  protected override void InsertItem(int index, T item)
  {
    if ((object) item == null)
      throw new InvalidOperationException("Cannot insert null items in the collection.");
    item.Lock();
    base.InsertItem(index, item);
  }

  protected override void SetItem(int index, T item)
  {
    if ((object) item == null)
      throw new InvalidOperationException("Cannot insert null items in the collection.");
    item.Lock();
    base.SetItem(index, item);
  }
}
