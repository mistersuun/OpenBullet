// Decompiled with JetBrains decompiler
// Type: Org.Mentalis.Network.ProxySocket.IAsyncProxyResult
// Assembly: ProxySocket, Version=1.1.2.0, Culture=neutral, PublicKeyToken=966874d7118d1436
// MVID: 6B674582-76D8-40E8-8371-90C9D44D5C18
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ProxySocket.dll

using System;
using System.Threading;

#nullable disable
namespace Org.Mentalis.Network.ProxySocket;

internal class IAsyncProxyResult : IAsyncResult
{
  private bool m_Completed;
  private object m_StateObject;
  private ManualResetEvent m_WaitHandle;

  internal IAsyncProxyResult(object stateObject = null)
  {
    this.m_StateObject = stateObject;
    this.m_Completed = false;
    if (this.m_WaitHandle == null)
      return;
    this.m_WaitHandle.Reset();
  }

  internal void Reset()
  {
    this.m_StateObject = (object) null;
    this.m_Completed = true;
    if (this.m_WaitHandle == null)
      return;
    this.m_WaitHandle.Set();
  }

  public bool IsCompleted => this.m_Completed;

  public bool CompletedSynchronously => false;

  public object AsyncState => this.m_StateObject;

  public WaitHandle AsyncWaitHandle
  {
    get
    {
      if (this.m_WaitHandle == null)
        this.m_WaitHandle = new ManualResetEvent(false);
      return (WaitHandle) this.m_WaitHandle;
    }
  }
}
