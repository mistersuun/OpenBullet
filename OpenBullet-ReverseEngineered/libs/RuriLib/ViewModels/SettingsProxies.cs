// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.SettingsProxies
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

#nullable disable
namespace RuriLib.ViewModels;

public class SettingsProxies : ViewModelBase
{
  private bool concurrentUse;
  private bool neverBan;
  private int banLoopEvasion = 100;
  private bool shuffleOnStart;
  private bool reload = true;
  private ProxyReloadSource reloadSource;
  private string reloadPath = "";
  private ProxyType reloadType;
  private int reloadInterval;
  private bool alwaysGetClearance;
  private string[] globalBanKeys = new string[0];
  private string[] globalRetryKeys = new string[0];

  public bool ConcurrentUse
  {
    get => this.concurrentUse;
    set
    {
      this.concurrentUse = value;
      this.OnPropertyChanged(nameof (ConcurrentUse));
    }
  }

  public bool NeverBan
  {
    get => this.neverBan;
    set
    {
      this.neverBan = value;
      this.OnPropertyChanged(nameof (NeverBan));
    }
  }

  public int BanLoopEvasion
  {
    get => this.banLoopEvasion;
    set
    {
      this.banLoopEvasion = value;
      this.OnPropertyChanged(nameof (BanLoopEvasion));
    }
  }

  public bool ShuffleOnStart
  {
    get => this.shuffleOnStart;
    set
    {
      this.shuffleOnStart = value;
      this.OnPropertyChanged(nameof (ShuffleOnStart));
    }
  }

  public bool Reload
  {
    get => this.reload;
    set
    {
      this.reload = value;
      this.OnPropertyChanged(nameof (Reload));
    }
  }

  public ProxyReloadSource ReloadSource
  {
    get => this.reloadSource;
    set
    {
      this.reloadSource = value;
      this.OnPropertyChanged(nameof (ReloadSource));
    }
  }

  public string ReloadPath
  {
    get => this.reloadPath;
    set
    {
      this.reloadPath = value;
      this.OnPropertyChanged(nameof (ReloadPath));
    }
  }

  public ProxyType ReloadType
  {
    get => this.reloadType;
    set
    {
      this.reloadType = value;
      this.OnPropertyChanged(nameof (ReloadType));
    }
  }

  public int ReloadInterval
  {
    get => this.reloadInterval;
    set
    {
      this.reloadInterval = value;
      this.OnPropertyChanged(nameof (ReloadInterval));
    }
  }

  public bool AlwaysGetClearance
  {
    get => this.alwaysGetClearance;
    set
    {
      this.alwaysGetClearance = value;
      this.OnPropertyChanged(nameof (AlwaysGetClearance));
    }
  }

  public string[] GlobalBanKeys
  {
    get => this.globalBanKeys;
    set
    {
      this.globalBanKeys = value;
      this.OnPropertyChanged(nameof (GlobalBanKeys));
    }
  }

  public string[] GlobalRetryKeys
  {
    get => this.globalRetryKeys;
    set
    {
      this.globalRetryKeys = value;
      this.OnPropertyChanged(nameof (GlobalRetryKeys));
    }
  }

  public ObservableCollection<RemoteProxySource> RemoteProxySources { get; set; } = new ObservableCollection<RemoteProxySource>();

  public void RemoveRemoteProxySourceById(int id)
  {
    this.RemoteProxySources.Remove(this.GetRemoteProxySourceById(id));
  }

  public RemoteProxySource GetRemoteProxySourceById(int id)
  {
    return this.RemoteProxySources.FirstOrDefault<RemoteProxySource>((Func<RemoteProxySource, bool>) (s => s.Id == id));
  }

  public void Reset()
  {
    SettingsProxies settingsProxies = new SettingsProxies();
    foreach (PropertyInfo propertyInfo in (IEnumerable<PropertyInfo>) new List<PropertyInfo>((IEnumerable<PropertyInfo>) typeof (SettingsProxies).GetProperties()))
      propertyInfo.SetValue((object) this, propertyInfo.GetValue((object) settingsProxies, (object[]) null));
  }
}
