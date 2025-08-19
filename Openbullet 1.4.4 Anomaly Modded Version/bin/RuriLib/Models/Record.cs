// Decompiled with JetBrains decompiler
// Type: RuriLib.Models.Record
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;

#nullable disable
namespace RuriLib.Models;

public class Record
{
  public Guid Id { get; set; }

  public string ConfigName { get; set; }

  public string WordlistLocation { get; set; }

  public int Checkpoint { get; set; }

  public Record()
  {
  }

  public Record(string configName, string wordlistLocation, int checkpoint)
  {
    this.ConfigName = configName;
    this.WordlistLocation = wordlistLocation;
    this.Checkpoint = checkpoint;
  }
}
