// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.DataPage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System.Collections.Generic;

#nullable disable
namespace LiteDB_V6;

internal class DataPage : BasePage
{
  public override PageType PageType => PageType.Data;

  public Dictionary<ushort, DataBlock> DataBlocks { get; set; }

  public DataPage(uint pageID)
    : base(pageID)
  {
    this.DataBlocks = new Dictionary<ushort, DataBlock>();
  }

  protected override void ReadContent(ByteReader reader)
  {
    this.DataBlocks = new Dictionary<ushort, DataBlock>(this.ItemCount);
    for (int index1 = 0; index1 < this.ItemCount; ++index1)
    {
      DataBlock dataBlock = new DataBlock();
      dataBlock.Page = this;
      dataBlock.Position = new PageAddress(this.PageID, reader.ReadUInt16());
      dataBlock.ExtendPageID = reader.ReadUInt32();
      for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
        dataBlock.IndexRef[index2] = reader.ReadPageAddress();
      ushort count = reader.ReadUInt16();
      dataBlock.Data = reader.ReadBytes((int) count);
      this.DataBlocks.Add(dataBlock.Position.Index, dataBlock);
    }
  }
}
