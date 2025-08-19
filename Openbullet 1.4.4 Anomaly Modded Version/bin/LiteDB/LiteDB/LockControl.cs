// Decompiled with JetBrains decompiler
// Type: LiteDB.LockControl
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

public class LockControl : IDisposable
{
  private Action _dispose;

  public bool Changed { get; private set; }

  internal LockControl(bool changed, Action dispose)
  {
    this.Changed = changed;
    this._dispose = dispose;
  }

  public void Dispose()
  {
    if (this._dispose == null)
      return;
    this._dispose();
  }
}
