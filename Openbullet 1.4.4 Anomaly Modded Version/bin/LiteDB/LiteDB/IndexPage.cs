// Decompiled with JetBrains decompiler
// Type: LiteDB.IndexPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class IndexPage(uint pageID) : BasePage(pageID)
{
  public const int INDEX_RESERVED_BYTES = 100;
  private Dictionary<ushort, IndexNode> _nodes = new Dictionary<ushort, IndexNode>();

  public override PageType PageType => PageType.Index;

  public IndexNode GetNode(ushort index) => this._nodes[index];

  public void AddNode(IndexNode node)
  {
    ushort num = this._nodes.NextIndex<IndexNode>();
    node.Position = new PageAddress(this.PageID, num);
    ++this.ItemCount;
    this.FreeBytes -= node.Length;
    this._nodes.Add(num, node);
  }

  public void DeleteNode(IndexNode node)
  {
    --this.ItemCount;
    this.FreeBytes += node.Length;
    this._nodes.Remove(node.Position.Index);
  }

  public int NodesCount => this._nodes.Count;

  protected override void ReadContent(ByteReader reader)
  {
    this._nodes = new Dictionary<ushort, IndexNode>(this.ItemCount);
    for (int index1 = 0; index1 < this.ItemCount; ++index1)
    {
      ushort index2 = reader.ReadUInt16();
      IndexNode indexNode = new IndexNode(reader.ReadByte())
      {
        Page = this,
        Position = new PageAddress(this.PageID, index2),
        Slot = reader.ReadByte(),
        PrevNode = reader.ReadPageAddress(),
        NextNode = reader.ReadPageAddress(),
        KeyLength = reader.ReadUInt16()
      };
      indexNode.Key = reader.ReadBsonValue(indexNode.KeyLength);
      indexNode.DataBlock = reader.ReadPageAddress();
      for (int index3 = 0; index3 < indexNode.Prev.Length; ++index3)
      {
        indexNode.Prev[index3] = reader.ReadPageAddress();
        indexNode.Next[index3] = reader.ReadPageAddress();
      }
      this._nodes.Add(indexNode.Position.Index, indexNode);
    }
  }

  protected override void WriteContent(ByteWriter writer)
  {
    foreach (IndexNode indexNode in this._nodes.Values)
    {
      writer.Write(indexNode.Position.Index);
      writer.Write((byte) indexNode.Prev.Length);
      writer.Write(indexNode.Slot);
      writer.Write(indexNode.PrevNode);
      writer.Write(indexNode.NextNode);
      writer.Write(indexNode.KeyLength);
      writer.WriteBsonValue(indexNode.Key, indexNode.KeyLength);
      writer.Write(indexNode.DataBlock);
      for (int index = 0; index < indexNode.Prev.Length; ++index)
      {
        writer.Write(indexNode.Prev[index]);
        writer.Write(indexNode.Next[index]);
      }
    }
  }
}
