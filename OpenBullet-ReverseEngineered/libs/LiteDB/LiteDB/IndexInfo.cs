// Decompiled with JetBrains decompiler
// Type: LiteDB.IndexInfo
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

public class IndexInfo
{
  internal IndexInfo(CollectionIndex index)
  {
    this.Slot = index.Slot;
    this.Field = index.Field;
    this.Expression = index.Expression;
    this.Unique = index.Unique;
    this.MaxLevel = index.MaxLevel;
  }

  public int Slot { get; private set; }

  public string Field { get; private set; }

  public string Expression { get; private set; }

  public bool Unique { get; private set; }

  public byte MaxLevel { get; private set; }
}
