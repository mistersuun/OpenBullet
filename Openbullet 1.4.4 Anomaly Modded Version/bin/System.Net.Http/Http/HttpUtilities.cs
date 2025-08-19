// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpUtilities
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

internal static class HttpUtilities
{
  internal static readonly Version DefaultRequestVersion = HttpVersion.Version11;
  internal static readonly Version DefaultResponseVersion = HttpVersion.Version11;

  internal static bool IsHttpUri(Uri uri)
  {
    string scheme = uri.Scheme;
    return string.Equals("http", scheme, StringComparison.OrdinalIgnoreCase) || string.Equals("https", scheme, StringComparison.OrdinalIgnoreCase);
  }

  internal static bool HandleFaultsAndCancelation<T>(Task task, TaskCompletionSource<T> tcs)
  {
    if (task.IsFaulted)
    {
      tcs.TrySetException(task.Exception.GetBaseException());
      return true;
    }
    if (!task.IsCanceled)
      return false;
    tcs.TrySetCanceled();
    return true;
  }

  internal static Task ContinueWithStandard(this Task task, Action<Task> continuation)
  {
    return task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
  }

  internal static Task ContinueWithStandard(
    this Task task,
    object state,
    Action<Task, object> continuation)
  {
    return task.ContinueWith(continuation, state, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
  }

  internal static Task ContinueWithStandard<T>(this Task<T> task, Action<Task<T>> continuation)
  {
    return task.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
  }

  internal static Task ContinueWithStandard<T>(
    this Task<T> task,
    object state,
    Action<Task<T>, object> continuation)
  {
    return task.ContinueWith(continuation, state, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
  }
}
