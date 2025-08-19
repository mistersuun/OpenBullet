// Decompiled with JetBrains decompiler
// Type: RuriLib.Runner.RunnerBotViewModel
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using OpenQA.Selenium.Remote;
using RuriLib.ViewModels;

#nullable disable
namespace RuriLib.Runner;

public class RunnerBotViewModel : ViewModelBase
{
  private int id;
  private string data = "";
  private string proxy = "";
  private string status = "";

  public int Id
  {
    get => this.id;
    set
    {
      this.id = value;
      this.OnPropertyChanged(nameof (Id));
    }
  }

  public string Data
  {
    get => this.data;
    set
    {
      this.data = value;
      this.OnPropertyChanged(nameof (Data));
    }
  }

  public string Proxy
  {
    get => this.proxy;
    set
    {
      this.proxy = value;
      this.OnPropertyChanged(nameof (Proxy));
    }
  }

  public string Status
  {
    get => this.status;
    set
    {
      this.status = value;
      this.OnPropertyChanged(nameof (Status));
    }
  }

  public AbortableBackgroundWorker Worker { get; set; }

  public RemoteWebDriver Driver { get; set; }

  public bool IsDriverOpen { get; set; }

  public RunnerBotViewModel(int id)
  {
    this.Id = id;
    this.Worker = new AbortableBackgroundWorker();
    this.Worker.WorkerSupportsCancellation = true;
    this.Worker.Id = id;
  }
}
