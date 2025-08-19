// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ThreadingUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class ThreadingUtils
{
  private static int id;
  private static System.Threading.ThreadLocal<int> threadIds = new System.Threading.ThreadLocal<int>((Func<int>) (() => Interlocked.Increment(ref ThreadingUtils.id)));

  public static int GetCurrentThreadId() => ThreadingUtils.threadIds.Value;
}
