// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.WordlistType
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System.Collections.Generic;

#nullable disable
namespace RuriLib.Models;

public class WordlistType
{
  public bool Verify { get; set; }

  public string Regex { get; set; } = "^.*$";

  public string Name { get; set; } = "Default";

  public string Separator { get; set; } = ":";

  public List<string> Slices { get; set; } = new List<string>();
}
