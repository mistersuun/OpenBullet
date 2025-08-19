// Decompiled with JetBrains decompiler
// Type: RuriLib.Runner.RunnerViewModel
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Newtonsoft.Json;
using RuriLib.LS;
using RuriLib.Models;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

#nullable disable
namespace RuriLib.Runner;

public class RunnerViewModel : ViewModelBase, IRunnerMessaging
{
  private int botsNumber = 1;
  private int startingPoint = 1;
  private int cpm;
  private double _balance;
  private ProxyMode proxyMode;
  private int retryCount;

  public RunnerViewModel(
    EnvironmentSettings environment,
    RLSettingsViewModel settings,
    Random random)
  {
    this.Env = environment;
    this.Settings = settings;
    this.Random = random;
    this.OnPropertyChanged(nameof (Busy));
    this.OnPropertyChanged(nameof (ControlsEnabled));
  }

  private RLSettingsViewModel Settings { get; set; }

  private EnvironmentSettings Env { get; set; }

  private Random Random { get; set; }

  public AbortableBackgroundWorker Master { get; set; } = new AbortableBackgroundWorker();

  public WorkerStatus WorkerStatus => this.Master.Status;

  public ObservableCollection<RunnerBotViewModel> Bots { get; } = new ObservableCollection<RunnerBotViewModel>();

  public bool Busy => this.Master.Status != 0;

  public bool ControlsEnabled => !this.Busy;

  public int BotsNumber
  {
    get => this.botsNumber;
    set
    {
      this.botsNumber = value;
      this.OnPropertyChanged(nameof (BotsNumber));
    }
  }

  public int StartingPoint
  {
    get => this.startingPoint;
    set
    {
      this.startingPoint = value;
      this.OnPropertyChanged(nameof (StartingPoint));
      this.OnPropertyChanged("Progress");
    }
  }

  public int ProgressCount => this.TestedCount + this.StartingPoint - 1;

  public int Progress
  {
    get
    {
      int a = 0;
      try
      {
        a = (int) ((double) (this.TestedCount + this.StartingPoint - 1) / (double) this.DataPool.Size * 100.0);
      }
      catch
      {
      }
      return this.Clamp(a, 0, 100);
    }
  }

  public int CPM
  {
    get
    {
      if (this.TestedCount == 0)
      {
        this.cpm = 0;
        return this.cpm;
      }
      if (this.IsCPMLocked)
        return this.cpm;
      try
      {
        int num = 0;
        this.IsCPMLocked = true;
        TimeSpan timeSpan;
        for (int index = this.FailedList.Count<ValidData>() - 1; index >= 0; --index)
        {
          timeSpan = DateTime.Now - this.FailedList[index].Time;
          if (timeSpan.TotalSeconds <= 60.0)
            ++num;
          else
            break;
        }
        for (int index = this.HitsList.Count<ValidData>() - 1; index >= 0; --index)
        {
          timeSpan = DateTime.Now - this.HitsList[index].Time;
          if (timeSpan.TotalSeconds <= 60.0)
            ++num;
          else
            break;
        }
        for (int index = this.CustomList.Count<ValidData>() - 1; index >= 0; --index)
        {
          timeSpan = DateTime.Now - this.CustomList[index].Time;
          if (timeSpan.TotalSeconds <= 60.0)
            ++num;
          else
            break;
        }
        for (int index = this.ToCheckList.Count<ValidData>() - 1; index >= 0; --index)
        {
          timeSpan = DateTime.Now - this.ToCheckList[index].Time;
          if (timeSpan.TotalSeconds <= 60.0)
            ++num;
          else
            break;
        }
        this.cpm = num;
        this.IsCPMLocked = false;
      }
      catch
      {
      }
      finally
      {
        this.IsCPMLocked = false;
      }
      return this.cpm;
    }
  }

  public double Balance
  {
    get => this._balance;
    set
    {
      this._balance = value;
      this.OnPropertyChanged("BalanceString");
    }
  }

  public string BalanceString => $"${this._balance}";

  public ProxyMode ProxyMode
  {
    get => this.proxyMode;
    set
    {
      this.proxyMode = value;
      this.OnPropertyChanged(nameof (ProxyMode));
    }
  }

  public bool UseProxies
  {
    get
    {
      return this.Config == null || this.Config.Settings.NeedsProxies && this.ProxyMode == ProxyMode.Default || this.ProxyMode == ProxyMode.On;
    }
  }

  public Config Config { get; private set; }

  public string ConfigName => this.Config != null ? this.Config.Settings.Name : "None";

  public Wordlist Wordlist { get; private set; }

  public string WordlistName => this.Wordlist != null ? this.Wordlist.Name : "None";

  public int WordlistSize => this.Wordlist != null ? this.Wordlist.Total : 0;

  public ProxyPool ProxyPool { get; set; } = new ProxyPool(new List<CProxy>());

  public int TotalProxiesCount => this.ProxyPool.Proxies.Count;

  public int AliveProxiesCount => this.ProxyPool.Alive.Count;

  public int AvailableProxiesCount => this.ProxyPool.Available.Count;

  public int BannedProxiesCount => this.ProxyPool.Banned.Count;

  public int BadProxiesCount => this.ProxyPool.Bad.Count;

  public DataPool DataPool { get; set; }

  public int DataSize => this.DataPool != null ? this.DataPool.Size : 0;

  public List<KeyValuePair<string, string>> CustomInputs { get; set; } = new List<KeyValuePair<string, string>>();

  public VariableList GlobalVariables { get; set; } = new VariableList();

  public CookieDictionary GlobalCookies { get; set; } = new CookieDictionary();

  public Action<BotData> CustomAction { get; set; }

  public List<ValidData> FailedList { get; set; } = new List<ValidData>();

  public ObservableCollection<ValidData> HitsList { get; set; } = new ObservableCollection<ValidData>();

  public ObservableCollection<ValidData> CustomList { get; set; } = new ObservableCollection<ValidData>();

  public ObservableCollection<ValidData> ToCheckList { get; set; } = new ObservableCollection<ValidData>();

  public ObservableCollection<ValidData> EmptyList { get; set; } = new ObservableCollection<ValidData>();

  public BotStatus ResultsFilter { get; set; } = BotStatus.SUCCESS;

  public int FailCount => this.FailedList.Count;

  public int HitCount => this.HitsList.Count;

  public int CustomCount => this.CustomList.Count;

  public int ToCheckCount => this.ToCheckList.Count;

  public int RetryCount
  {
    get => this.retryCount;
    set
    {
      this.retryCount = value;
      this.OnPropertyChanged(nameof (RetryCount));
    }
  }

  public int TestedCount => this.FailCount + this.HitCount + this.CustomCount + this.ToCheckCount;

  private bool IsReloadingProxies { get; set; }

  private bool IsCPMLocked { get; set; }

  private bool NoProxyWarningSent { get; set; }

  public bool CustomInputsInitialized { get; set; }

  private Stopwatch Timer { get; set; } = new Stopwatch();

  public string TimerDays => this.Timer.Elapsed.Days.ToString();

  public string TimerHours => this.Timer.Elapsed.Hours.ToString("D2");

  public string TimerMinutes => this.Timer.Elapsed.Minutes.ToString("D2");

  public string TimerSeconds => this.Timer.Elapsed.Seconds.ToString("D2");

  public string TimeLeft
  {
    get
    {
      if (this.Wordlist == null)
        return "Unknown time left";
      int num1 = this.Wordlist.Total - this.StartingPoint - this.TestedCount;
      if (this.CPM == 0)
        return "+inf";
      int num2 = num1 / this.CPM * 60;
      string str = "seconds";
      if (num2 > 60)
      {
        num2 /= 60;
        str = "minutes";
      }
      if (num2 > 60)
      {
        num2 /= 60;
        str = "hours";
      }
      if (num2 > 24)
      {
        num2 /= 24;
        str = "days";
      }
      return $"{num2} {str} left";
    }
  }

  public void SetConfig(Config config, bool setRecommended)
  {
    this.Config = config;
    if (setRecommended)
      this.BotsNumber = this.Clamp(config.Settings.SuggestedBots, 1, 200);
    this.OnPropertyChanged("ConfigName");
  }

  public void SetWordlist(Wordlist wordlist)
  {
    this.Wordlist = wordlist;
    this.OnPropertyChanged("WordlistName");
    this.OnPropertyChanged("WordlistSize");
  }

  public void Start()
  {
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, "Setting up the background worker");
    this.Master = new AbortableBackgroundWorker();
    this.Master.DoWork += new DoWorkEventHandler(this.Run);
    this.Master.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.RunCompleted);
    this.Master.WorkerSupportsCancellation = true;
    this.Timer.Reset();
    this.Timer.Start();
    if (!this.Master.IsBusy)
    {
      this.Master.Status = WorkerStatus.Running;
      this.OnPropertyChanged("Busy");
      this.OnPropertyChanged("ControlsEnabled");
      this.RaiseWorkerStatusChanged();
      this.Master.RunWorkerAsync();
      this.RaiseMessageArrived(RuriLib.LogLevel.Info, "Started the Master Worker");
    }
    else
      this.RaiseMessageArrived(RuriLib.LogLevel.Error, "Cannot start the Background Worker (busy)");
  }

  public void Stop()
  {
    if (this.Master.IsBusy)
      this.Master.CancelAsync();
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, "Sent cancellation request to Master Worker");
    this.Master.Status = WorkerStatus.Stopping;
    this.OnPropertyChanged("Busy");
    this.OnPropertyChanged("ControlsEnabled");
    this.RaiseWorkerStatusChanged();
  }

  public void ForceStop()
  {
    this.AbortAllBots();
    this.Master.Abort();
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, "Hard Aborted the Master Worker");
    this.Timer.Stop();
    this.Master.Status = WorkerStatus.Idle;
    this.OnPropertyChanged("Busy");
    this.OnPropertyChanged("ControlsEnabled");
    this.RaiseWorkerStatusChanged();
    this.StartingPoint += this.TestedCount;
  }

  private void Run(object sender, DoWorkEventArgs e)
  {
    if (this.CustomAction != null)
      this.Config = new Config(new ConfigSettings(), "");
    if (this.Config == null)
      throw new Exception("No Config loaded!");
    if (this.Wordlist == null)
      throw new Exception("No Wordlist loaded!");
    if (!this.Wordlist.Temporary)
      this.DataPool = new DataPool(File.ReadLines(this.Wordlist.Path));
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Loaded {this.DataPool.Size} lines");
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Using Proxies: {this.UseProxies}");
    if (this.DataPool.Size == 0)
      throw new Exception("No data to process!");
    if (this.StartingPoint > this.DataPool.Size)
      throw new Exception("Illegal Starting Point!");
    if (this.CustomAction == null && this.Config.BlocksAmount == 0)
      throw new Exception("The Config has zero blocks!");
    this.NoProxyWarningSent = false;
    this.CustomInputsInitialized = false;
    this.RetryCount = 0;
    this.FailedList.Clear();
    this.GlobalVariables = new VariableList();
    this.GlobalCookies = new CookieDictionary();
    this.RaiseDispatchAction((Action) (() =>
    {
      this.HitsList.Clear();
      this.CustomList.Clear();
      this.ToCheckList.Clear();
    }));
    if (this.UseProxies)
    {
      this.LoadProxies();
      if (this.TotalProxiesCount == 0)
        throw new Exception("Zero proxies available!");
    }
    if (this.Config.Settings.CustomInputs.Count > 0)
    {
      this.RaiseAskCustomInputs();
      while (!this.CustomInputsInitialized)
        Thread.Sleep(100);
    }
    this.RaiseDispatchAction((Action) (() => this.Bots.Clear()));
    for (int id = 1; id <= this.BotsNumber; ++id)
    {
      this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Creating bot {id}");
      RunnerBotViewModel bot = new RunnerBotViewModel(id);
      if (this.Settings.General.BotsDisplayMode == BotsDisplayMode.None)
        bot.Status = "Bots Display is Disabled in Settings";
      bot.Worker.DoWork += new DoWorkEventHandler(this.RunBot);
      this.RaiseDispatchAction((Action) (() => this.Bots.Add(bot)));
    }
    foreach (string data in this.DataPool.List.Skip<string>(this.StartingPoint - 1))
    {
      if (this.Master.CancellationPending)
      {
        this.AbortAllBots();
        return;
      }
      CData cdata = new CData(data, this.Env.GetWordlistType(this.Wordlist.Type));
      if (!cdata.IsValid || !cdata.RespectsRules(this.Config.Settings.DataRules.ToList<DataRule>()))
      {
        this.FailedList.Add(new ValidData(data, "", ProxyType.Http, BotStatus.FAIL, "", "", "", (List<LogEntry>) null));
      }
      else
      {
        this.UpdateTimer();
        this.UpdateStats();
        this.UpdateCPM();
        if (this.BotsNumber != this.Bots.Count)
        {
          this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Bots Number was changed from {this.Bots.Count} to {this.BotsNumber}");
          if (this.BotsNumber > this.Bots.Count)
          {
            for (int id = this.Bots.Count + 1; id <= this.BotsNumber; ++id)
            {
              this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Creating bot {id}");
              try
              {
                RunnerBotViewModel bot = new RunnerBotViewModel(id);
                if (this.Settings.General.BotsDisplayMode == BotsDisplayMode.None)
                  bot.Status = "Bots Display is Disabled in Settings";
                bot.Worker.DoWork += new DoWorkEventHandler(this.RunBot);
                this.RaiseDispatchAction((Action) (() => this.Bots.Add(bot)));
              }
              catch (Exception ex)
              {
                this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"Error while creating bot {id}: " + ex.Message);
              }
            }
          }
          else
          {
            for (int b = this.Bots.Count - 1; b >= this.BotsNumber; b--)
            {
              this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Removing bot {b}");
              try
              {
                RunnerBotViewModel bot = this.Bots[b];
                if (bot.IsDriverOpen)
                  bot.Driver.Quit();
                bot.Worker.CancelAsync();
                bot.Worker.Abort();
                this.RaiseDispatchAction((Action) (() => this.Bots.RemoveAt(b)));
              }
              catch (Exception ex)
              {
                this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"Error while creating bot {b}: " + ex.Message);
              }
            }
          }
        }
        this.RaiseMessageArrived(RuriLib.LogLevel.Info, "Trying to assign data " + data);
        bool flag = false;
        while (!flag)
        {
          if (this.Settings.General.MaxHits != 0 && this.HitCount >= this.Settings.General.MaxHits)
          {
            this.AbortAllBots();
            return;
          }
          if (this.Master.CancellationPending)
          {
            this.AbortAllBots();
            return;
          }
          if (this.Timer.IsRunning && (int) this.Timer.Elapsed.TotalSeconds != 0)
          {
            if ((int) this.Timer.Elapsed.TotalSeconds % 120 == 0)
              this.RaiseSaveProgress();
            if (this.Settings.Proxies.ReloadInterval > 0 && (int) this.Timer.Elapsed.TotalSeconds % (this.Settings.Proxies.ReloadInterval * 60) == 0)
              this.LoadProxies();
          }
          if (this.Config.Settings.MaxCPM > 0 && this.cpm >= this.Config.Settings.MaxCPM)
          {
            this.UpdateCPM();
          }
          else
          {
            foreach (RunnerBotViewModel bot in (Collection<RunnerBotViewModel>) this.Bots)
            {
              if (!bot.Worker.IsBusy)
              {
                this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Assigned data {data} to bot {(object) bot.Id}");
                bot.Worker.RunWorkerAsync((object) data);
                flag = true;
                break;
              }
            }
          }
          if (!flag)
          {
            this.UpdateTimer();
            this.UpdateStats();
            Thread.Sleep(200);
          }
        }
      }
    }
    while (this.Bots.Select<RunnerBotViewModel, AbortableBackgroundWorker>((Func<RunnerBotViewModel, AbortableBackgroundWorker>) (b => b.Worker)).Any<AbortableBackgroundWorker>((Func<AbortableBackgroundWorker, bool>) (w => w.IsBusy)) && !this.Master.CancellationPending)
    {
      this.RaiseMessageArrived(RuriLib.LogLevel.Info, "All data assigned, waiting for completion");
      this.UpdateStats();
      this.UpdateCPM();
      Thread.Sleep(200);
    }
  }

  private void RunCompleted(object sender, RunWorkerCompletedEventArgs e)
  {
    if (e.Error != null)
      this.RaiseMessageArrived(RuriLib.LogLevel.Error, "The Master Worker has encountered an error: " + e.Error.Message, true);
    this.Master.Status = WorkerStatus.Idle;
    this.OnPropertyChanged("Busy");
    this.OnPropertyChanged("ControlsEnabled");
    this.RaiseWorkerStatusChanged();
    this.Timer.Stop();
    this.AbortAllBots();
    this.StartingPoint += this.TestedCount;
  }

  private void RunBot(object sender, DoWorkEventArgs e)
  {
    AbortableBackgroundWorker senderABW = (AbortableBackgroundWorker) sender;
    RunnerBotViewModel runnerBotViewModel = this.Bots.First<RunnerBotViewModel>((Func<RunnerBotViewModel, bool>) (b => b.Id == senderABW.Id));
    runnerBotViewModel.Status = "INITIALIZING...";
    string data1 = (string) e.Argument;
    runnerBotViewModel.Data = data1;
    CData data2 = new CData(data1, this.Env.GetWordlistType(this.Wordlist.Type));
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"[{runnerBotViewModel.Id}] started with data " + runnerBotViewModel.Data);
    try
    {
      while (!senderABW.CancellationPending && !this.ShouldStop())
      {
        CProxy proxy = (CProxy) null;
        if (this.UseProxies)
        {
          proxy = this.ProxyPool.GetProxy(this.Settings.Proxies.ConcurrentUse, this.Config.Settings.MaxProxyUses, this.Settings.Proxies.NeverBan);
          if (proxy == null)
          {
            if (this.Settings.Proxies.Reload)
            {
              if (!this.IsReloadingProxies)
              {
                this.IsReloadingProxies = true;
                this.LoadProxies();
                this.IsReloadingProxies = false;
              }
              else
              {
                Thread.Sleep(100);
                continue;
              }
            }
            else if (!this.ShouldStop())
            {
              if (!this.NoProxyWarningSent)
              {
                this.RaiseMessageArrived(RuriLib.LogLevel.Error, "No more proxies and no Unban All option selected OR the config has a max proxy use! Aborting", true, 2);
                this.NoProxyWarningSent = true;
              }
              this.Master.CancelAsync();
              return;
            }
          }
          runnerBotViewModel.Proxy = proxy.Proxy;
        }
        string str1 = proxy == null ? "NONE" : $"{proxy.Proxy} ({proxy.Type})";
        BotData data3 = new BotData(this.Settings, this.Config.Settings, data2, proxy, this.UseProxies, this.Random, runnerBotViewModel.Id, false);
        data3.Driver = runnerBotViewModel.Driver;
        data3.BrowserOpen = runnerBotViewModel.IsDriverOpen;
        List<LogEntry> log = new List<LogEntry>();
        foreach (KeyValuePair<string, string> customInput in this.CustomInputs)
          data3.Variables.Set(new CVar(customInput.Key, customInput.Value));
        data3.GlobalVariables = this.GlobalVariables;
        try
        {
          foreach (KeyValuePair<string, string> globalCookie in (Dictionary<string, string>) this.GlobalCookies)
            data3.Cookies.Add(globalCookie.Key, globalCookie.Value);
        }
        catch
        {
        }
        if (data3.UseProxies && data3.Proxy != null && data3.Proxy.Clearance != "" && !this.Settings.Proxies.AlwaysGetClearance)
        {
          data3.Cookies["cf_clearance"] = data3.Proxy.Clearance;
          data3.Cookies["__cfduid"] = data3.Proxy.Cfduid;
        }
        log.Add(new LogEntry($"===== LOG FOR BOT #{runnerBotViewModel.Id} WITH DATA {data3.Data.Data} AND PROXY {str1} ====={Environment.NewLine}", Colors.White));
        if (this.CustomAction != null)
        {
          this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"[{runnerBotViewModel.Id}] Executing custom action");
          try
          {
            this.CustomAction(data3);
          }
          catch (Exception ex)
          {
            this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"[{runnerBotViewModel.Id}] CUSTOM ACTION EXCEPTION: {ex.ToString()}");
            throw;
          }
        }
        else
        {
          LoliScript loliScript = new LoliScript(this.Config.Script);
          loliScript.Reset();
          if (!this.Settings.General.EnableBotLog)
            log.Add(new LogEntry("The Bot Logging is disabled in General Settings", Colors.Tomato));
          if (this.Config.Settings.AlwaysOpen)
            SBlockBrowserAction.OpenBrowser(data3);
          while (!senderABW.CancellationPending && !this.ShouldStop())
          {
            if (this.Settings.General.BotsDisplayMode == BotsDisplayMode.Everything)
              runnerBotViewModel.Status = $"<<< PROCESSING BLOCK: {loliScript.NextBlock} >>>";
            try
            {
              loliScript.TakeStep(data3);
            }
            catch (BlockProcessingException ex)
            {
              if (this.Settings.General.BotsDisplayMode == BotsDisplayMode.Everything)
                runnerBotViewModel.Status = $"<<< ERROR IN BLOCK: {loliScript.NextBlock} >>>";
              this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"[{runnerBotViewModel.Id}][{runnerBotViewModel.Data}][{str1}] ERROR in block {loliScript.NextBlock} | Exception: {ex.Message}");
              Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
              if (this.Settings.General.BotsDisplayMode == BotsDisplayMode.Everything)
                runnerBotViewModel.Status = "<<< SCRIPT ERROR >>>";
              this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"[{runnerBotViewModel.Id}][{runnerBotViewModel.Data}][{str1}] ERROR in the script | Exception: {ex.Message}");
              Thread.Sleep(1000);
            }
            if (data3.LogBuffer.Count > 0)
            {
              log.AddRange((IEnumerable<LogEntry>) data3.LogBuffer);
              log.Add(new LogEntry("", Colors.White));
            }
            if (!loliScript.CanProceed)
              goto label_53;
          }
          this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"[{runnerBotViewModel.Id}] Cancellation pending, aborting");
          return;
        }
label_53:
        log.Add(new LogEntry($"===== BOT TERMINATED WITH RESULT: {data3.StatusString} =====", Colors.White));
        if (this.Settings.General.BotsDisplayMode != BotsDisplayMode.None)
          runnerBotViewModel.Status = $"<<< FINISHED WITH RESULT: {data3.StatusString} >>>";
        this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"[{runnerBotViewModel.Id}][{runnerBotViewModel.Data}][{str1}] Ended with result {data3.StatusString}");
        if (this.Config.Settings.AlwaysQuit)
        {
          try
          {
            data3.Driver.Quit();
            data3.BrowserOpen = false;
          }
          catch
          {
          }
        }
        runnerBotViewModel.Driver = data3.Driver;
        runnerBotViewModel.IsDriverOpen = data3.BrowserOpen;
        if (this.UseProxies)
        {
          proxy.Status = Status.AVAILABLE;
          ++proxy.Uses;
          --proxy.Hooked;
        }
        this.Balance = data3.Balance;
        try
        {
          this.GlobalCookies = new CookieDictionary();
          foreach (KeyValuePair<string, string> globalCookie in data3.GlobalCookies)
            this.GlobalCookies.Add(globalCookie.Key, globalCookie.Value);
        }
        catch
        {
        }
        ValidData validData = (ValidData) null;
        string customStatus = data3.Status.ToString();
        VariableList capturedData = new VariableList(data3.Variables.All.Where<CVar>((Func<CVar, bool>) (v => v.IsCapture && !v.Hidden)).ToList<CVar>());
        switch (data3.Status)
        {
          case BotStatus.NONE:
            validData = new ValidData(data3.Data.Data, data3.Proxy == null ? "" : data3.Proxy.Proxy, data3.Proxy == null ? ProxyType.Http : data3.Proxy.Type, data3.Status, "TOCHK", capturedData.ToCaptureString(), this.Settings.General.SaveLastSource ? data3.ResponseSource : "", log);
            this.RaiseDispatchAction((Action) (() => this.ToCheckList.Add(validData)));
            if (this.UseProxies && !this.Settings.Proxies.NeverBan && this.Config.Settings.BanProxyAfterGoodStatus)
            {
              proxy.Status = Status.BANNED;
              proxy.LastUsed = DateTime.Now;
              ++this.RetryCount;
              break;
            }
            break;
          case BotStatus.ERROR:
            ++this.RetryCount;
            if (this.UseProxies)
            {
              proxy.Status = Status.BAD;
              continue;
            }
            continue;
          case BotStatus.SUCCESS:
            validData = new ValidData(data3.Data.Data, data3.Proxy == null ? "" : data3.Proxy.Proxy, data3.Proxy == null ? ProxyType.Http : data3.Proxy.Type, data3.Status, "HIT", capturedData.ToCaptureString(), this.Settings.General.SaveLastSource ? data3.ResponseSource : "", log);
            this.RaiseDispatchAction((Action) (() => this.HitsList.Add(validData)));
            if (this.UseProxies && !this.Settings.Proxies.NeverBan && this.Config.Settings.BanProxyAfterGoodStatus)
            {
              proxy.Status = Status.BANNED;
              proxy.LastUsed = DateTime.Now;
              ++this.RetryCount;
              break;
            }
            break;
          case BotStatus.FAIL:
            this.FailedList.Add(new ValidData("", "", ProxyType.Http, data3.Status, "", "", "", (List<LogEntry>) null));
            break;
          case BotStatus.BAN:
            if (this.UseProxies && !this.Settings.Proxies.NeverBan)
            {
              proxy.Status = Status.BANNED;
              proxy.LastUsed = DateTime.Now;
              ++this.RetryCount;
            }
            if (data2.Retries < this.Settings.Proxies.BanLoopEvasion || this.Settings.Proxies.BanLoopEvasion == 0)
            {
              ++data2.Retries;
              continue;
            }
            this.RaiseMessageArrived(RuriLib.LogLevel.Warning, $"[{runnerBotViewModel.Id}][{runnerBotViewModel.Data}] Maximum retries exceeded");
            data3.Status = BotStatus.NONE;
            customStatus = data3.Status.ToString();
            goto case BotStatus.NONE;
          case BotStatus.RETRY:
            ++this.RetryCount;
            continue;
          case BotStatus.CUSTOM:
            customStatus = data3.CustomStatus;
            validData = new ValidData(data3.Data.Data, data3.Proxy == null ? "" : data3.Proxy.Proxy, data3.Proxy == null ? ProxyType.Http : data3.Proxy.Type, data3.Status, customStatus, capturedData.ToCaptureString(), this.Settings.General.SaveLastSource ? data3.ResponseSource : "", log);
            this.RaiseDispatchAction((Action) (() => this.CustomList.Add(validData)));
            if (this.UseProxies && !this.Settings.Proxies.NeverBan && this.Config.Settings.BanProxyAfterGoodStatus)
            {
              proxy.Status = Status.BANNED;
              proxy.LastUsed = DateTime.Now;
              ++this.RetryCount;
              break;
            }
            break;
        }
        if (validData != null)
          this.RaiseFoundHit(new Hit(data3.Data.Data, capturedData, proxy == null ? "" : proxy.Proxy, customStatus, this.ConfigName, this.WordlistName));
        if (this.Settings.General.WebhookEnabled && (data3.Status == BotStatus.SUCCESS || data3.Status == BotStatus.CUSTOM))
        {
          HttpRequest httpRequest = new HttpRequest();
          try
          {
            string str2 = JsonConvert.SerializeObject((object) new WebhookFormat(data1, customStatus, capturedData.ToCaptureString(), DateTime.Now, this.Config.Settings.Name, this.Config.Settings.Author, this.Settings.General.WebhookUser));
            httpRequest.PostAsync(this.Settings.General.WebhookURL, str2, "application/json");
          }
          catch
          {
            this.RaiseMessageArrived(RuriLib.LogLevel.Error, "Could not register the hit to webhook " + this.Settings.General.WebhookURL);
          }
        }
        if (this.Settings.General.WaitTime <= 0)
          return;
        Thread.Sleep(this.Settings.General.WaitTime);
        return;
      }
      this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"[{runnerBotViewModel.Id}] Cancellation pending, aborting");
    }
    catch (Exception ex)
    {
      this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"{$"[{runnerBotViewModel.Id}] Check exception on data "}{data1} - {ex.Message}");
    }
  }

  private bool ShouldStop()
  {
    return this.Master.CancellationPending || this.Master.Status != WorkerStatus.Running;
  }

  public void UpdateTimer()
  {
    this.OnPropertyChanged("TimerDays");
    this.OnPropertyChanged("TimerHours");
    this.OnPropertyChanged("TimerMinutes");
    this.OnPropertyChanged("TimerSeconds");
    this.OnPropertyChanged("TimeLeft");
  }

  public void UpdateStats()
  {
    this.OnPropertyChanged("HitsList");
    this.OnPropertyChanged("FreeList");
    this.OnPropertyChanged("ToCheckList");
    this.OnPropertyChanged("TestedCount");
    this.OnPropertyChanged("ProgressCount");
    this.OnPropertyChanged("FailCount");
    this.OnPropertyChanged("HitCount");
    this.OnPropertyChanged("CustomCount");
    this.OnPropertyChanged("ToCheckCount");
    this.OnPropertyChanged("Balance");
    this.OnPropertyChanged("Progress");
    this.OnPropertyChanged("TotalProxiesCount");
    this.OnPropertyChanged("AvailableProxiesCount");
    this.OnPropertyChanged("AliveProxiesCount");
    this.OnPropertyChanged("BannedProxiesCount");
    this.OnPropertyChanged("BadProxiesCount");
  }

  public void UpdateCPM() => this.OnPropertyChanged("CPM");

  public event Action<IRunnerMessaging, RuriLib.LogLevel, string, bool, int> MessageArrived;

  public event Action<IRunnerMessaging> WorkerStatusChanged;

  public event Action<IRunnerMessaging, Hit> FoundHit;

  public event Action<IRunnerMessaging> ReloadProxies;

  public event Action<IRunnerMessaging, Action> DispatchAction;

  public event Action<IRunnerMessaging> SaveProgress;

  public event Action<IRunnerMessaging> AskCustomInputs;

  private void RaiseMessageArrived(RuriLib.LogLevel level, string message, bool prompt = false, int timeout = 0)
  {
    Action<IRunnerMessaging, RuriLib.LogLevel, string, bool, int> messageArrived = this.MessageArrived;
    if (messageArrived == null)
      return;
    messageArrived((IRunnerMessaging) this, level, message, prompt, timeout);
  }

  private void RaiseWorkerStatusChanged()
  {
    this.OnPropertyChanged("WorkerStatus");
    Action<IRunnerMessaging> workerStatusChanged = this.WorkerStatusChanged;
    if (workerStatusChanged == null)
      return;
    workerStatusChanged((IRunnerMessaging) this);
  }

  private void RaiseFoundHit(Hit hit)
  {
    Action<IRunnerMessaging, Hit> foundHit = this.FoundHit;
    if (foundHit == null)
      return;
    foundHit((IRunnerMessaging) this, hit);
  }

  private void RaiseReloadingProxies()
  {
    Action<IRunnerMessaging> reloadProxies = this.ReloadProxies;
    if (reloadProxies == null)
      return;
    reloadProxies((IRunnerMessaging) this);
  }

  private void RaiseDispatchAction(Action action)
  {
    Action<IRunnerMessaging, Action> dispatchAction = this.DispatchAction;
    if (dispatchAction == null)
      return;
    dispatchAction((IRunnerMessaging) this, action);
  }

  private void RaiseSaveProgress()
  {
    Action<IRunnerMessaging> saveProgress = this.SaveProgress;
    if (saveProgress == null)
      return;
    saveProgress((IRunnerMessaging) this);
  }

  private void RaiseAskCustomInputs()
  {
    Action<IRunnerMessaging> askCustomInputs = this.AskCustomInputs;
    if (askCustomInputs == null)
      return;
    askCustomInputs((IRunnerMessaging) this);
  }

  private void LoadProxies()
  {
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Loading proxies from {this.Settings.Proxies.ReloadSource}");
    switch (this.Settings.Proxies.ReloadSource)
    {
      case ProxyReloadSource.Manager:
        this.RaiseReloadingProxies();
        Thread.Sleep(100);
        break;
      case ProxyReloadSource.File:
        try
        {
          this.ProxyPool = new ProxyPool(RunnerViewModel.GetProxiesFromFile(this.Settings.Proxies.ReloadPath, this.Settings.Proxies.ReloadType), this.Settings.Proxies.ShuffleOnStart);
          break;
        }
        catch (Exception ex)
        {
          this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"Could not read the proxies from file {this.Settings.Proxies.ReloadPath} - {ex.Message}", true);
          break;
        }
      case ProxyReloadSource.Remote:
        List<CProxy> proxies = new List<CProxy>();
        Parallel.ForEach<RemoteProxySource>(this.Settings.Proxies.RemoteProxySources.Where<RemoteProxySource>((Func<RemoteProxySource, bool>) (s => s.Active)), (Action<RemoteProxySource>) (s =>
        {
          try
          {
            proxies.AddRange((IEnumerable<CProxy>) RunnerViewModel.GetProxiesFromRemoteSource(s.Url, s.Type, s.Pattern, s.Output));
          }
          catch (Exception ex)
          {
            this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"Could not contact the reload API {s.Url} for {s.Type} proxies - {ex.Message}", true, 5);
          }
        }));
        this.ProxyPool = new ProxyPool(proxies, this.Settings.Proxies.ShuffleOnStart);
        break;
    }
    this.RaiseMessageArrived(RuriLib.LogLevel.Info, $"Loaded {this.TotalProxiesCount} proxies");
  }

  public static List<CProxy> GetProxiesFromRemoteSource(
    string url,
    ProxyType type,
    string pattern,
    string output)
  {
    List<CProxy> fromRemoteSource = new List<CProxy>();
    HttpRequest httpRequest = new HttpRequest();
    httpRequest.ConnectTimeout = 5000;
    httpRequest.ReadWriteTimeout = 5000;
    Random random = new Random();
    httpRequest.UserAgent = $"{$"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/53{(object) random.Next(0, 9)}7.3{(object) random.Next(0, 9)} (KHTML, like Gecko) Chrome/41.0.2{(object) random.Next(0, 9)}" + (object) random.Next(0, 9)}{(object) random.Next(0, 9)}.0 Safari/53{(object) random.Next(0, 9)}.3{(object) random.Next(0, 9)}";
    string input = httpRequest.Get(url).ToString();
    try
    {
      foreach (Match match in Regex.Matches(input, pattern))
      {
        string proxy = output;
        for (int groupnum = 0; groupnum < match.Groups.Count; ++groupnum)
          proxy = proxy.Replace($"[{(object) groupnum}]", match.Groups[groupnum].Value);
        fromRemoteSource.Add(new CProxy(proxy, type));
      }
    }
    catch
    {
    }
    return fromRemoteSource;
  }

  public static async Task<RemoteProxySourceResult> GetProxiesFromRemoteSourceAsync(
    string url,
    ProxyType type,
    string pattern,
    string output)
  {
    List<CProxy> proxies = new List<CProxy>();
    Random random = new Random();
    try
    {
      foreach (Match match in Regex.Matches((await new HttpRequest()
      {
        UserAgent = $"{$"Mozilla / 5.0(Windows NT 6.1) AppleWebKit / 53{(object) random.Next(0, 9)}7.3{(object) random.Next(0, 9)}(KHTML, like Gecko) Chrome / 41.0.2{(object) random.Next(0, 9)}" + (object) random.Next(0, 9)}{(object) random.Next(0, 9)}.0 Safari / 53{(object) random.Next(0, 9)}.3{(object) random.Next(0, 9)}"
      }.GetAsync(url)).ToString(), pattern))
      {
        string proxy = output;
        for (int groupnum = 0; groupnum < match.Groups.Count; ++groupnum)
          proxy = proxy.Replace($"[{(object) groupnum}]", match.Groups[groupnum].Value);
        proxies.Add(new CProxy(proxy, type));
      }
    }
    catch (Exception ex)
    {
      return new RemoteProxySourceResult()
      {
        Successful = false,
        Error = ex.Message,
        Url = url,
        Proxies = new List<CProxy>()
      };
    }
    return new RemoteProxySourceResult()
    {
      Successful = true,
      Error = "",
      Url = url,
      Proxies = proxies
    };
  }

  public static List<CProxy> GetProxiesFromFile(string fileName, ProxyType type)
  {
    string[] source = File.ReadAllLines(fileName);
    List<CProxy> proxiesFromFile = new List<CProxy>();
    proxiesFromFile.AddRange(((IEnumerable<string>) source).Select<string, CProxy>((Func<string, CProxy>) (l => new CProxy(l, type))));
    return proxiesFromFile;
  }

  private void AbortAllBots()
  {
    foreach (RunnerBotViewModel bot in (Collection<RunnerBotViewModel>) this.Bots)
    {
      try
      {
        bot.Worker.CancelAsync();
      }
      catch (Exception ex)
      {
        this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"Error while deleting bot {bot.Id}: " + ex.Message);
      }
    }
    this.QuitAllDrivers();
  }

  private void QuitAllDrivers()
  {
    foreach (RunnerBotViewModel bot in (Collection<RunnerBotViewModel>) this.Bots)
    {
      try
      {
        if (bot.IsDriverOpen)
          bot.Driver.Quit();
      }
      catch (Exception ex)
      {
        this.RaiseMessageArrived(RuriLib.LogLevel.Error, $"Error while quitting driver {bot.Id}: " + ex.Message);
      }
    }
  }

  public int Clamp(int a, int min, int max)
  {
    if (a < min)
      return min;
    return a > max ? max : a;
  }
}
