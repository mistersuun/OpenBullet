// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.WeakHandle
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.Utils;

public struct WeakHandle(object target, bool trackResurrection)
{
  private readonly GCHandle _gcHandle = GCHandle.Alloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);

  public object Target
  {
    get
    {
      if (!this._gcHandle.IsAllocated)
        return (object) null;
      try
      {
        return this._gcHandle.Target;
      }
      catch (InvalidOperationException ex)
      {
        return (object) null;
      }
    }
  }

  public void Free()
  {
    if (!this._gcHandle.IsAllocated)
      return;
    try
    {
      this._gcHandle.Free();
    }
    catch (InvalidOperationException ex)
    {
    }
  }
}
