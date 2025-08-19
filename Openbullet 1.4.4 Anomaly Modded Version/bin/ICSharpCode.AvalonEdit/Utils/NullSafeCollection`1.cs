// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.NullSafeCollection`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.ObjectModel;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
public class NullSafeCollection<T> : Collection<T> where T : class
{
  protected override void InsertItem(int index, T item)
  {
    if ((object) item == null)
      throw new ArgumentNullException(nameof (item));
    base.InsertItem(index, item);
  }

  protected override void SetItem(int index, T item)
  {
    if ((object) item == null)
      throw new ArgumentNullException(nameof (item));
    base.SetItem(index, item);
  }
}
