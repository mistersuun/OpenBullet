// Decompiled with JetBrains decompiler
// Type: RuriLib.ViewModels.CustomInput
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

#nullable disable
namespace RuriLib.ViewModels;

public class CustomInput : ViewModelBase
{
  private string description = "";
  private string variableName = "";

  public string Description
  {
    get => this.description;
    set
    {
      this.description = value;
      this.OnPropertyChanged(nameof (Description));
    }
  }

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public int Id { get; set; }

  public CustomInput(int id) => this.Id = id;
}
