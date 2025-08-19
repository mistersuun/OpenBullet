// Decompiled with JetBrains decompiler
// Type: LiteDB.IndexNode
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

internal class IndexNode
{
  public const int INDEX_NODE_FIXED_SIZE = 25;
  public const int MAX_LEVEL_LENGTH = 32 /*0x20*/;

  public PageAddress Position { get; set; }

  public byte Slot { get; set; }

  public PageAddress PrevNode { get; set; }

  public PageAddress NextNode { get; set; }

  public PageAddress[] Prev { get; set; }

  public PageAddress[] Next { get; set; }

  public ushort KeyLength { get; set; }

  public BsonValue Key { get; set; }

  public PageAddress DataBlock { get; set; }

  public IndexPage Page { get; set; }

  public PageAddress NextPrev(int index, int order)
  {
    return order != 1 ? this.Prev[index] : this.Next[index];
  }

  public bool IsHeadTail(CollectionIndex index)
  {
    return this.Position.Equals((object) index.HeadNode) || this.Position.Equals((object) index.TailNode);
  }

  public int Length => 25 + this.Prev.Length * 6 * 2 + (int) this.KeyLength;

  public BsonDocument CacheDocument { get; set; }

  public IndexNode(byte level)
  {
    this.Position = PageAddress.Empty;
    this.PrevNode = PageAddress.Empty;
    this.NextNode = PageAddress.Empty;
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
