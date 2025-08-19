// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.ConfigViewModel
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

#nullable disable
namespace RuriLib.ViewModels;

public class ConfigViewModel : ViewModelBase
{
  private string category = "Default";
  private string path = "";

  public Config Config { get; set; }

  public string Category
  {
    get => this.category;
    set
    {
      this.category = value;
      this.OnPropertyChanged(nameof (Category));
    }
  }

  public string Path
  {
    get => this.path;
    set
    {
      this.path = value;
      this.OnPropertyChanged(nameof (Path));
    }
  }

  public bool Remote { get; set; }

  public string Name => this.Config.Settings.Name;

  public ConfigViewModel(string path, string category, Config config, bool remote = false)
  {
    this.Path = path;
    this.Category = category;
    this.Config = config;
    this.Remote = remote;
  }
}
