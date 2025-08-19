// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.EventLoopExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Browser;

public static class EventLoopExtensions
{
  public static void Enqueue(this IEventLoop loop, Action action, TaskPriority priority = TaskPriority.Normal)
  {
    if (loop != null)
      loop.Enqueue((Action<CancellationToken>) (c => action()), priority);
    else
      action();
  }

  public static Task<T> EnqueueAsync<T>(
    this IEventLoop loop,
    Func<CancellationToken, T> action,
    TaskPriority priority = TaskPriority.Normal)
  {
    if (loop != null)
    {
      TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
      loop.Enqueue((Action<CancellationToken>) (c =>
      {
        try
        {
          tcs.SetResult(action(c));
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }), priority);
      return tcs.Task;
    }
    try
    {
      return Task.FromResult<T>(action(new CancellationToken()));
    }
    catch (Exception ex)
    {
      return Task.FromException<T>(ex);
    }
  }
}
