// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.Publisher`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class Publisher<TKey, TValue>
{
  private readonly Dictionary<TKey, Publisher<TKey, TValue>.PublishInfo<TValue>> data = new Dictionary<TKey, Publisher<TKey, TValue>.PublishInfo<TValue>>();

  public TValue GetOrCreateValue(TKey key, Func<TValue> create)
  {
    bool lockTaken = false;
    try
    {
      MonitorUtils.Enter((object) this.data, ref lockTaken);
      Publisher<TKey, TValue>.PublishInfo<TValue> publishInfo;
      if (this.data.TryGetValue(key, out publishInfo))
      {
        if ((object) publishInfo.Value == null && publishInfo.Exception == null)
        {
          publishInfo.PrepareForWait();
          MonitorUtils.Exit((object) this.data, ref lockTaken);
          try
          {
            publishInfo.WaitForPublish();
          }
          finally
          {
            MonitorUtils.Enter((object) this.data, ref lockTaken);
            publishInfo.FinishWait();
          }
        }
        return publishInfo.Exception == null ? publishInfo.Value : throw new Exception("Error", publishInfo.Exception);
      }
      this.data[key] = publishInfo = new Publisher<TKey, TValue>.PublishInfo<TValue>();
      MonitorUtils.Exit((object) this.data, ref lockTaken);
      TValue obj;
      try
      {
        try
        {
          obj = create();
        }
        finally
        {
          MonitorUtils.Enter((object) this.data, ref lockTaken);
        }
      }
      catch (Exception ex)
      {
        publishInfo.PublishError(ex);
        throw;
      }
      publishInfo.PublishValue(obj);
      return obj;
    }
    finally
    {
      if (lockTaken)
        Monitor.Exit((object) this.data);
    }
  }

  public IEnumerable<TKey> Keys => (IEnumerable<TKey>) this.data.Keys;

  private class PublishInfo<T>
  {
    public T Value;
    public Exception Exception;
    private ManualResetEvent _waitEvent;
    private int _waiters;

    public void PublishValue(T value)
    {
      this.Value = value;
      this._waitEvent?.Set();
    }

    public void PublishError(Exception e) => this.Exception = e;

    public void PrepareForWait()
    {
      if (this._waitEvent == null)
      {
        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        if (Interlocked.CompareExchange<ManualResetEvent>(ref this._waitEvent, manualResetEvent, (ManualResetEvent) null) != null)
          manualResetEvent.Dispose();
      }
      ++this._waiters;
    }

    public void WaitForPublish() => this._waitEvent.WaitOne();

    public void FinishWait()
    {
      --this._waiters;
      if (this._waiters != 0)
        return;
      this._waitEvent.Dispose();
    }
  }
}
