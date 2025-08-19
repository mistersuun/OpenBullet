// Decompiled with JetBrains decompiler
// Type: LiteDB.DataPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class DataPage(uint pageID) : BasePage(pageID)
{
  public const int DATA_RESERVED_BYTES = 2035;
  private Dictionary<ushort, DataBlock> _dataBlocks = new Dictionary<ushort, DataBlock>();

  public override PageType PageType => PageType.Data;

  public DataBlock GetBlock(ushort index) => this._dataBlocks[index];

  public void AddBlock(DataBlock block)
  {
    ushort num = this._dataBlocks.NextIndex<DataBlock>();
    block.Position = new PageAddress(this.PageID, num);
    ++this.ItemCount;
    this.FreeBytes -= block.Length;
    this._dataBlocks.Add(num, block);
  }

  public void UpdateBlockData(DataBlock block, byte[] data)
  {
    this.FreeBytes = this.FreeBytes + block.Data.Length - data.Length;
    block.Data = data;
  }

  public void DeleteBlock(DataBlock block)
  {
    --this.ItemCount;
    this.FreeBytes += block.Length;
    this._dataBlocks.Remove(block.Position.Index);
  }

  public int BlocksCount => this._dataBlocks.Count;

  protected override void ReadContent(ByteReader reader)
  {
    this._dataBlocks = new Dictionary<ushort, DataBlock>(this.ItemCount);
    for (int index = 0; index < this.ItemCount; ++index)
    {
      DataBlock dataBlock = new DataBlock();
      dataBlock.Page = this;
      dataBlock.Position = new PageAddress(this.PageID, reader.ReadUInt16());
      dataBlock.ExtendPageID = reader.ReadUInt32();
      ushort count = reader.ReadUInt16();
      dataBlock.Data = reader.ReadBytes((int) count);
      this._dataBlocks.Add(dataBlock.Position.Index, dataBlock);
    }
  }

  protected override void WriteContent(ByteWriter writer)
  {
    foreach (DataBlock dataBlock in this._dataBlocks.Values)
    {
      writer.Write(dataBlock.Position.Index);
      writer.Write(dataBlock.ExtendPageID);
      writer.Write((ushort) dataBlock.Data.Length);
      writer.Write(dataBlock.Data);
    }
  }
}
