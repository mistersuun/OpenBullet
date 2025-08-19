// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.ThreadLocal`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal class ThreadLocal<T>
{
  private ThreadLocal<T>.StorageInfo[] _stores;
  private static readonly ThreadLocal<T>.StorageInfo[] Updating = new ThreadLocal<T>.StorageInfo[0];

  internal T Value
  {
    get => this.GetStorageInfo().Value;
    set => this.GetStorageInfo().Value = value;
  }

  internal T[] Values
  {
    get
    {
      List<T> objList = new List<T>(this._stores.Length);
      foreach (ThreadLocal<T>.StorageInfo store in this._stores)
      {
        if (store != null && store.Thread.IsAlive)
          objList.Add(store.Value);
      }
      return objList.ToArray();
    }
  }

  private ThreadLocal<T>.StorageInfo GetStorageInfo() => this.GetStorageInfo(this._stores);

  private ThreadLocal<T>.StorageInfo GetStorageInfo(ThreadLocal<T>.StorageInfo[] curStorage)
  {
    int managedThreadId = Thread.CurrentThread.ManagedThreadId;
    if (curStorage != null && curStorage.Length > managedThreadId)
    {
      ThreadLocal<T>.StorageInfo storageInfo = curStorage[managedThreadId];
      if (storageInfo != null && storageInfo.Thread == Thread.CurrentThread)
        return storageInfo;
    }
    return this.RetryOrCreateStorageInfo(curStorage);
  }

  private ThreadLocal<T>.StorageInfo RetryOrCreateStorageInfo(
    ThreadLocal<T>.StorageInfo[] curStorage)
  {
    if (curStorage != ThreadLocal<T>.Updating)
      return this.CreateStorageInfo();
    while ((curStorage = this._stores) == ThreadLocal<T>.Updating)
      Thread.Sleep(0);
    return this.GetStorageInfo(curStorage);
  }

  private ThreadLocal<T>.StorageInfo CreateStorageInfo()
  {
    Thread.BeginCriticalRegion();
    ThreadLocal<T>.StorageInfo[] storageInfoArray1 = ThreadLocal<T>.Updating;
    try
    {
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      ThreadLocal<T>.StorageInfo storageInfo = new ThreadLocal<T>.StorageInfo(Thread.CurrentThread);
      while ((storageInfoArray1 = Interlocked.Exchange<ThreadLocal<T>.StorageInfo[]>(ref this._stores, ThreadLocal<T>.Updating)) == ThreadLocal<T>.Updating)
        Thread.Sleep(0);
      if (storageInfoArray1 == null)
        storageInfoArray1 = new ThreadLocal<T>.StorageInfo[managedThreadId + 1];
      else if (storageInfoArray1.Length <= managedThreadId)
      {
        ThreadLocal<T>.StorageInfo[] storageInfoArray2 = new ThreadLocal<T>.StorageInfo[managedThreadId + 1];
        for (int index = 0; index < storageInfoArray1.Length; ++index)
        {
          if (storageInfoArray1[index] != null && storageInfoArray1[index].Thread.IsAlive)
            storageInfoArray2[index] = storageInfoArray1[index];
        }
        storageInfoArray1 = storageInfoArray2;
      }
      return storageInfoArray1[managedThreadId] = storageInfo;
    }
    finally
    {
      if (storageInfoArray1 != ThreadLocal<T>.Updating)
        Interlocked.Exchange<ThreadLocal<T>.StorageInfo[]>(ref this._stores, storageInfoArray1);
      Thread.EndCriticalRegion();
    }
  }

  private class StorageInfo
  {
    public readonly Thread Thread;
    public T Value;

    public StorageInfo(Thread curThread) => this.Thread = curThread;
  }
}
