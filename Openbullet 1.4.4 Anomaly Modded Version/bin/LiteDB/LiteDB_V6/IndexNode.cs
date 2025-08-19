// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.IndexNode
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;

#nullable disable
namespace LiteDB_V6;

internal class IndexNode
{
  public PageAddress Position { get; set; }

  public PageAddress[] Prev { get; set; }

  public PageAddress[] Next { get; set; }

  public ushort KeyLength { get; set; }

  public BsonValue Key { get; set; }

  public PageAddress DataBlock { get; set; }

  public IndexPage Page { get; set; }

  public bool IsHeadTail(CollectionIndex index)
  {
    return this.Position.Equals((object) index.HeadNode) || this.Position.Equals((object) index.TailNode);
  }

  public IndexNode(byte level)
  {
    this.Position = PageAddress.Empty;
    this.DataBlock = PageAddress.Empty;
    this.Prev = new PageAddress[(int) level];
    this.Next = new PageAddress[(int) level];
    for (int index = 0; index < (int) level; ++index)
    {
      this.Prev[index] = PageAddress.Empty;
      this.Next[index] = PageAddress.Empty;
    }
  }
}
