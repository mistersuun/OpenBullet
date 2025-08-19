// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Download
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io;

internal sealed class Download : IDownload, ICancellable<IResponse>, ICancellable
{
  private readonly CancellationTokenSource _cts;
  private readonly System.Threading.Tasks.Task<IResponse> _task;
  private readonly Url _target;
  private readonly object _source;

  public Download(System.Threading.Tasks.Task<IResponse> task, CancellationTokenSource cts, Url target, object source)
  {
    this._task = task;
    this._cts = cts;
    this._target = target;
    this._source = source;
  }

  public object Source => this._source;

  public Url Target => this._target;

  public System.Threading.Tasks.Task<IResponse> Task => this._task;

  public bool IsRunning => this._task.Status == TaskStatus.Running;

  public bool IsCompleted
  {
    get
    {
      return this._task.Status == TaskStatus.Faulted || this._task.Status == TaskStatus.RanToCompletion || this._task.Status == TaskStatus.Canceled;
    }
  }

  public void Cancel() => this._cts.Cancel();
}
