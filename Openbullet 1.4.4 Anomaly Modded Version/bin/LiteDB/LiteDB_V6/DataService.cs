// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.DataService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System.IO;

#nullable disable
namespace LiteDB_V6;

internal class DataService
{
  private PageService _pager;

  public DataService(PageService pager) => this._pager = pager;

  public byte[] Read(PageAddress blockAddress)
  {
    DataBlock block = this.GetBlock(blockAddress);
    return block.ExtendPageID != uint.MaxValue ? this.ReadExtendData(block.ExtendPageID) : block.Data;
  }

  public DataBlock GetBlock(PageAddress blockAddress)
  {
    return this._pager.GetPage<DataPage>(blockAddress.PageID).DataBlocks[blockAddress.Index];
  }

  public byte[] ReadExtendData(uint extendPageID)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      foreach (ExtendPage seqPage in this._pager.GetSeqPages<ExtendPage>(extendPageID))
        memoryStream.Write(seqPage.Data, 0, seqPage.Data.Length);
      return memoryStream.ToArray();
    }
  }
}
