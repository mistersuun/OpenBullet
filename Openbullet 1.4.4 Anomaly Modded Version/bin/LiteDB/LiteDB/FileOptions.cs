// Decompiled with JetBrains decompiler
// Type: LiteDB.FileOptions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

public class FileOptions
{
  public bool Journal { get; set; }

  public long InitialSize { get; set; }

  public long LimitSize { get; set; }

  public FileMode FileMode { get; set; }

  public bool Flush { get; set; }

  public FileOptions()
  {
    this.Journal = true;
    this.InitialSize = 0L;
    this.LimitSize = long.MaxValue;
    this.FileMode = FileMode.Shared;
    this.Flush = false;
  }
}
