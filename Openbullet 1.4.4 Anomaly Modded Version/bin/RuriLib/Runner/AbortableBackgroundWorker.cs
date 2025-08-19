// Decompiled with JetBrains decompiler
// Type: RuriLib.Runner.AbortableBackgroundWorker
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System.ComponentModel;
using System.Threading;

#nullable disable
namespace RuriLib.Runner;

public class AbortableBackgroundWorker : BackgroundWorker
{
  private Thread workerThread;

  public WorkerStatus Status { get; set; }

  public int Id { get; set; }

  protected override void OnDoWork(DoWorkEventArgs e)
  {
    this.workerThread = Thread.CurrentThread;
    try
    {
      base.OnDoWork(e);
    }
    catch (ThreadAbortException ex)
    {
      e.Cancel = true;
      Thread.ResetAbort();
    }
  }

  public void Abort()
  {
    if (this.workerThread == null)
      return;
    this.workerThread.Abort();
    this.workerThread = (Thread) null;
  }
}
