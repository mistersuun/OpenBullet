// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComEventSinksContainer
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ComEventSinksContainer : List<ComEventSink>, IDisposable
{
  private static readonly object _ComObjectEventSinksKey = new object();

  private ComEventSinksContainer()
  {
  }

  public static ComEventSinksContainer FromRuntimeCallableWrapper(object rcw, bool createIfNotFound)
  {
    object comObjectData1 = Marshal.GetComObjectData(rcw, ComEventSinksContainer._ComObjectEventSinksKey);
    if (comObjectData1 != null || !createIfNotFound)
      return (ComEventSinksContainer) comObjectData1;
    lock (ComEventSinksContainer._ComObjectEventSinksKey)
    {
      object comObjectData2 = Marshal.GetComObjectData(rcw, ComEventSinksContainer._ComObjectEventSinksKey);
      if (comObjectData2 != null)
        return (ComEventSinksContainer) comObjectData2;
      ComEventSinksContainer data = new ComEventSinksContainer();
      if (!Marshal.SetComObjectData(rcw, ComEventSinksContainer._ComObjectEventSinksKey, (object) data))
        throw Error.SetComObjectDataFailed();
      return data;
    }
  }

  public void Dispose()
  {
    this.DisposeAll();
    GC.SuppressFinalize((object) this);
  }

  private void DisposeAll()
  {
    foreach (ComEventSink comEventSink in (List<ComEventSink>) this)
      comEventSink.Dispose();
  }

  ~ComEventSinksContainer() => this.DisposeAll();
}
