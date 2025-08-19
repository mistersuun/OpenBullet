// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ThreadLocal`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class ThreadLocal<T>
{
  private ThreadLocal<T>.StorageInfo[] _stores;
  private static readonly ThreadLocal<T>.StorageInfo[] Updating = new ThreadLocal<T>.StorageInfo[0];
  private readonly bool _refCounted;

  public ThreadLocal()
  {
  }

  public ThreadLocal(bool refCounted) => this._refCounted = refCounted;

  public T Value
  {
    get => this.GetStorageInfo().Value;
    set => this.GetStorageInfo().Value = value;
  }

  public T GetOrCreate(Func<T> func)
  {
    ThreadLocal<T>.StorageInfo storageInfo = this.GetStorageInfo();
    T obj = storageInfo.Value;
    if ((object) obj == null)
      storageInfo.Value = obj = func();
    return obj;
  }

  public T Update(Func<T, T> updater)
  {
    ThreadLocal<T>.StorageInfo storageInfo = this.GetStorageInfo();
    return storageInfo.Value = updater(storageInfo.Value);
  }

  public T Update(T newValue)
  {
    ThreadLocal<T>.StorageInfo storageInfo = this.GetStorageInfo();
    T obj = storageInfo.Value;
    storageInfo.Value = newValue;
    return obj;
  }

  private static int GetCurrentThreadId() => Thread.CurrentThread.ManagedThreadId;

  public ThreadLocal<T>.StorageInfo GetStorageInfo() => this.GetStorageInfo(this._stores);

  private ThreadLocal<T>.StorageInfo GetStorageInfo(ThreadLocal<T>.StorageInfo[] curStorage)
  {
    int currentThreadId = ThreadLocal<T>.GetCurrentThreadId();
    if (curStorage != null && curStorage.Length > currentThreadId)
    {
      ThreadLocal<T>.StorageInfo storageInfo = curStorage[currentThreadId];
      if (storageInfo != null && (this._refCounted || storageInfo.Thread == Thread.CurrentThread))
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
      int currentThreadId = ThreadLocal<T>.GetCurrentThreadId();
      ThreadLocal<T>.StorageInfo storageInfo = new ThreadLocal<T>.StorageInfo(Thread.CurrentThread);
      while ((storageInfoArray1 = Interlocked.Exchange<ThreadLocal<T>.StorageInfo[]>(ref this._stores, ThreadLocal<T>.Updating)) == ThreadLocal<T>.Updating)
        Thread.Sleep(0);
      if (storageInfoArray1 == null)
        storageInfoArray1 = new ThreadLocal<T>.StorageInfo[currentThreadId + 1];
      else if (storageInfoArray1.Length <= currentThreadId)
      {
        ThreadLocal<T>.StorageInfo[] storageInfoArray2 = new ThreadLocal<T>.StorageInfo[currentThreadId + 1];
        for (int index = 0; index < storageInfoArray1.Length; ++index)
        {
          if (storageInfoArray1[index] != null && storageInfoArray1[index].Thread.IsAlive)
            storageInfoArray2[index] = storageInfoArray1[index];
        }
        storageInfoArray1 = storageInfoArray2;
      }
      return storageInfoArray1[currentThreadId] = storageInfo;
    }
    finally
    {
      if (storageInfoArray1 != ThreadLocal<T>.Updating)
        Interlocked.Exchange<ThreadLocal<T>.StorageInfo[]>(ref this._stores, storageInfoArray1);
      Thread.EndCriticalRegion();
    }
  }

  public sealed class StorageInfo
  {
    internal readonly Thread Thread;
    public T Value;

    internal StorageInfo(Thread curThread) => this.Thread = curThread;
  }
}
