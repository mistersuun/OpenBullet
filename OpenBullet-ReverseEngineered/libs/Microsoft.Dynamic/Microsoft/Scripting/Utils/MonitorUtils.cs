// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.MonitorUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class MonitorUtils
{
  public static void Enter(object obj, ref bool lockTaken) => Monitor.Enter(obj, ref lockTaken);

  public static bool TryEnter(object obj, ref bool lockTaken)
  {
    Monitor.TryEnter(obj, ref lockTaken);
    return lockTaken;
  }

  public static void Exit(object obj, ref bool lockTaken)
  {
    try
    {
    }
    finally
    {
      lockTaken = false;
      Monitor.Exit(obj);
    }
  }
}
