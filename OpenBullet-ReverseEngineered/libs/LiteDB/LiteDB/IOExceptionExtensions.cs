// Decompiled with JetBrains decompiler
// Type: LiteDB.IOExceptionExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace LiteDB;

internal static class IOExceptionExtensions
{
  private const int ERROR_SHARING_VIOLATION = 32 /*0x20*/;
  private const int ERROR_LOCK_VIOLATION = 33;

  public static bool IsLocked(this IOException ex)
  {
    int num = Marshal.GetHRForException((Exception) ex) & (int) ushort.MaxValue;
    return num == 32 /*0x20*/ || num == 33;
  }

  public static void WaitIfLocked(this IOException ex, int timer)
  {
    if (!ex.IsLocked())
      throw ex;
    if (timer <= 0)
      return;
    IOExceptionExtensions.WaitFor(timer);
  }

  public static void WaitFor(int ms) => Thread.Sleep(ms);
}
