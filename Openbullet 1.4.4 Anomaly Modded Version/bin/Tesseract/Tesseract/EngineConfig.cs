// Decompiled with JetBrains decompiler
// Type: Tesseract.EngineConfig
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract;

public class EngineConfig
{
  private string _dataPath;
  private string _language;

  public string DataPath
  {
    get => this._dataPath;
    set => this._dataPath = value;
  }

  public string Language
  {
    get => this._language;
    set => this._language = value;
  }
}
