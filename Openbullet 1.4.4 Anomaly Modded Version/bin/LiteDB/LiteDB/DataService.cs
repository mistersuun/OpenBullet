// Decompiled with JetBrains decompiler
// Type: LiteDB.DataService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;

#nullable disable
namespace LiteDB;

internal class DataService
{
  private PageService _pager;
  private Logger _log;

  public DataService(PageService pager, Logger log)
  {
    this._pager = pager;
    this._log = log;
  }

  public DataBlock Insert(CollectionPage col, byte[] data)
  {
    bool flag = data.Length + 8 > 4071;
    DataPage freePage = this._pager.GetFreePage<DataPage>(col.FreeDataPageID, flag ? 8 : data.Length + 8);
    DataBlock block = new DataBlock() { Page = freePage };
    if (flag)
    {
      ExtendPage page = this._pager.NewPage<ExtendPage>();
      block.ExtendPageID = page.PageID;
      this.StoreExtendData(page, data);
    }
    else
      block.Data = data;
    freePage.AddBlock(block);
    this._pager.SetDirty((BasePage) freePage);
    this._pager.AddOrRemoveToFreeList(freePage.FreeBytes > 2035, (BasePage) freePage, (BasePage) col, ref col.FreeDataPageID);
    ++col.DocumentCount;
    this._pager.SetDirty((BasePage) col);
    return block;
  }

  public DataBlock Update(CollectionPage col, PageAddress blockAddress, byte[] data)
  {
    DataPage page1 = this._pager.GetPage<DataPage>(blockAddress.PageID);
    DataBlock block = page1.GetBlock(blockAddress.Index);
    if (page1.FreeBytes + block.Data.Length - data.Length <= 0)
    {
      page1.UpdateBlockData(block, new byte[0]);
      ExtendPage page2;
      if (block.ExtendPageID == uint.MaxValue)
      {
        page2 = this._pager.NewPage<ExtendPage>();
        block.ExtendPageID = page2.PageID;
      }
      else
        page2 = this._pager.GetPage<ExtendPage>(block.ExtendPageID);
      this.StoreExtendData(page2, data);
    }
    else
    {
      page1.UpdateBlockData(block, data);
      if (block.ExtendPageID != uint.MaxValue)
      {
        this._pager.DeletePage(block.ExtendPageID, true);
        block.ExtendPageID = uint.MaxValue;
      }
    }
    this._pager.SetDirty((BasePage) page1);
    this._pager.AddOrRemoveToFreeList(page1.FreeBytes > 2035, (BasePage) page1, (BasePage) col, ref col.FreeDataPageID);
    return block;
  }

  public byte[] Read(PageAddress blockAddress)
  {
    DataBlock block = this.GetBlock(blockAddress);
    return block.ExtendPageID != uint.MaxValue ? this.ReadExtendData(block.ExtendPageID) : block.Data;
  }

  public DataBlock GetBlock(PageAddress blockAddress)
  {
    return this._pager.GetPage<DataPage>(blockAddress.PageID).GetBlock(blockAddress.Index);
  }

  public byte[] ReadExtendData(uint extendPageID)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      foreach (ExtendPage seqPage in this._pager.GetSeqPages<ExtendPage>(extendPageID))
        memoryStream.Write(seqPage.GetData(), 0, seqPage.ItemCount);
      return memoryStream.ToArray();
    }
  }

  public DataBlock Delete(CollectionPage col, PageAddress blockAddress)
  {
    DataPage page = this._pager.GetPage<DataPage>(blockAddress.PageID);
    DataBlock block = page.GetBlock(blockAddress.Index);
    if (block.ExtendPageID != uint.MaxValue)
      this._pager.DeletePage(block.ExtendPageID, true);
    page.DeleteBlock(block);
    this._pager.SetDirty((BasePage) page);
    if (page.BlocksCount == 0)
    {
      this._pager.AddOrRemoveToFreeList(false, (BasePage) page, (BasePage) col, ref col.FreeDataPageID);
      this._pager.DeletePage(page.PageID);
    }
    else
      this._pager.AddOrRemoveToFreeList(page.FreeBytes > 2035, (BasePage) page, (BasePage) col, ref col.FreeDataPageID);
    --col.DocumentCount;
    this._pager.SetDirty((BasePage) col);
    return block;
  }

  public void StoreExtendData(ExtendPage page, byte[] data)
  {
    int offset = 0;
    int length1 = data.Length;
    while (length1 > 0)
    {
      int length2 = Math.Min(length1, 4071);
      page.SetData(data, offset, length2);
      length1 -= length2;
      offset += length2;
      this._pager.SetDirty((BasePage) page);
      if (length1 > 0)
        page = page.NextPageID != uint.MaxValue ? this._pager.GetPage<ExtendPage>(page.NextPageID) : this._pager.NewPage<ExtendPage>((BasePage) page);
    }
    if (page.NextPageID == uint.MaxValue)
      return;
    this._pager.DeletePage(page.NextPageID, true);
    page.NextPageID = uint.MaxValue;
    this._pager.SetDirty((BasePage) page);
  }
}
