// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.DataRule
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Newtonsoft.Json;
using RuriLib.ViewModels;

#nullable disable
namespace RuriLib.Models;

public class DataRule : ViewModelBase
{
  private string sliceName = "";
  private RuleType ruleType;
  private string ruleString = "Lowercase";

  public string SliceName
  {
    get => this.sliceName;
    set
    {
      this.sliceName = value;
      this.OnPropertyChanged(nameof (SliceName));
    }
  }

  public RuleType RuleType
  {
    get => this.ruleType;
    set
    {
      this.ruleType = value;
      this.OnPropertyChanged(nameof (RuleType));
    }
  }

  public string RuleString
  {
    get => this.ruleString;
    set
    {
      this.ruleString = value;
      this.OnPropertyChanged(nameof (RuleString));
    }
  }

  public int Id { get; set; }

  [JsonIgnore]
  public bool TypeInitialized { get; set; }

  [JsonIgnore]
  public bool StringInitialized { get; set; }

  public DataRule(int id) => this.Id = id;
}
