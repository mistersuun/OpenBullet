// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.BusyManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class BusyManager
{
  [ThreadStatic]
  private static List<object> _activeObjects;

  public static BusyManager.BusyLock Enter(object obj)
  {
    List<object> objectList = BusyManager._activeObjects ?? (BusyManager._activeObjects = new List<object>());
    for (int index = 0; index < objectList.Count; ++index)
    {
      if (objectList[index] == obj)
        return BusyManager.BusyLock.Failed;
    }
    objectList.Add(obj);
    return new BusyManager.BusyLock(objectList);
  }

  public struct BusyLock : IDisposable
  {
    public static readonly BusyManager.BusyLock Failed = new BusyManager.BusyLock((List<object>) null);
    private readonly List<object> objectList;

    internal BusyLock(List<object> objectList) => this.objectList = objectList;

    public bool Success => this.objectList != null;

    public void Dispose()
    {
      if (this.objectList == null)
        return;
      this.objectList.RemoveAt(this.objectList.Count - 1);
    }
  }
}
