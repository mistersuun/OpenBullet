// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.Key
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.Functions.Conditions;
using RuriLib.ViewModels;

#nullable disable
namespace RuriLib.Models;

public class Key : ViewModelBase
{
  private string leftTerm = "<SOURCE>";
  private Comparer comparer = Comparer.Contains;
  private string rightTerm = "";

  public string LeftTerm
  {
    get => this.leftTerm;
    set
    {
      this.leftTerm = value;
      this.OnPropertyChanged(nameof (LeftTerm));
    }
  }

  public Comparer Comparer
  {
    get => this.comparer;
    set
    {
      this.comparer = value;
      this.OnPropertyChanged(nameof (Comparer));
    }
  }

  public string RightTerm
  {
    get => this.rightTerm;
    set
    {
      this.rightTerm = value;
      this.OnPropertyChanged(nameof (RightTerm));
    }
  }

  public bool CheckKey(BotData data)
  {
    try
    {
      return Condition.ReplaceAndVerify(this.LeftTerm, this.Comparer, this.RightTerm, data);
    }
    catch
    {
      return false;
    }
  }
}
