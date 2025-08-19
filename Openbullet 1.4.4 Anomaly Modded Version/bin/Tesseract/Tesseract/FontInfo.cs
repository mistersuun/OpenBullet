// Decompiled with JetBrains decompiler
// Type: Tesseract.FontInfo
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

#nullable disable
namespace Tesseract;

public class FontInfo
{
  internal FontInfo(
    string name,
    int id,
    bool isItalic,
    bool isBold,
    bool isFixedPitch,
    bool isSerif,
    bool isFraktur = false)
  {
    this.Name = name;
    this.Id = id;
    this.IsItalic = isItalic;
    this.IsBold = isBold;
    this.IsFixedPitch = isFixedPitch;
    this.IsSerif = isSerif;
    this.IsFraktur = isFraktur;
  }

  public string Name { get; private set; }

  public int Id { get; private set; }

  public bool IsItalic { get; private set; }

  public bool IsBold { get; private set; }

  public bool IsFixedPitch { get; private set; }

  public bool IsSerif { get; private set; }

  public bool IsFraktur { get; private set; }
}
