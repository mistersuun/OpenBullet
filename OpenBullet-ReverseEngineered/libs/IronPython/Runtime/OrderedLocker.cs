// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.OrderedLocker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public struct OrderedLocker : IDisposable
{
  private readonly object _one;
  private readonly object _two;
  private bool _oneLocked;
  private bool _twoLocked;

  public OrderedLocker(object one, object two)
  {
    this._one = one;
    this._two = two;
    this._oneLocked = false;
    this._twoLocked = false;
    if (one == two)
    {
      try
      {
      }
      finally
      {
        MonitorUtils.Enter(one, ref this._oneLocked);
      }
    }
    else
    {
      int hashCode1 = RuntimeHelpers.GetHashCode(this._one);
      int hashCode2 = RuntimeHelpers.GetHashCode(this._two);
      if (hashCode1 < hashCode2)
      {
        MonitorUtils.Enter(this._one, ref this._oneLocked);
        MonitorUtils.Enter(this._two, ref this._twoLocked);
      }
      else if (hashCode1 != hashCode2)
      {
        MonitorUtils.Enter(this._two, ref this._twoLocked);
        MonitorUtils.Enter(this._one, ref this._oneLocked);
      }
      else if (IdDispenser.GetId(this._one) < IdDispenser.GetId(this._two))
      {
        MonitorUtils.Enter(this._one, ref this._oneLocked);
        MonitorUtils.Enter(this._two, ref this._twoLocked);
      }
      else
      {
        MonitorUtils.Enter(this._two, ref this._twoLocked);
        MonitorUtils.Enter(this._one, ref this._oneLocked);
      }
    }
  }

  public void Dispose()
  {
    MonitorUtils.Exit(this._one, ref this._oneLocked);
    if (this._one == this._two)
      return;
    MonitorUtils.Exit(this._two, ref this._twoLocked);
  }
}
