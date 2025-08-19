// Decompiled with JetBrains decompiler
// Type: LiteDB.PageService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class PageService
{
  private CacheService _cache;
  private IDiskService _disk;
  private AesEncryption _crypto;
  private Logger _log;

  public PageService(IDiskService disk, AesEncryption crypto, CacheService cache, Logger log)
  {
    this._disk = disk;
    this._crypto = crypto;
    this._cache = cache;
    this._log = log;
  }

  public T GetPage<T>(uint pageID) where T : BasePage
  {
    lock (this._disk)
    {
      BasePage page = this._cache.GetPage(pageID);
      if (page == null)
      {
        byte[] numArray = this._disk.ReadPage(pageID);
        if (this._crypto != null && pageID > 0U)
          numArray = this._crypto.Decrypt(numArray);
        page = BasePage.ReadPage(numArray);
        this._cache.AddPage(page);
      }
      return (T) page;
    }
  }

  public void SetDirty(BasePage page) => this._cache.SetDirty(page);

  public IEnumerable<T> GetSeqPages<T>(uint firstPageID) where T : BasePage
  {
    uint pageID = firstPageID;
    while (pageID != uint.MaxValue)
    {
      T page = this.GetPage<T>(pageID);
      pageID = page.NextPageID;
      yield return page;
    }
  }

  public T NewPage<T>(BasePage prevPage = null) where T : BasePage
  {
    HeaderPage page1 = this.GetPage<HeaderPage>(0U);
    byte[] numArray = new byte[0];
    uint pageID;
    if (page1.FreeEmptyPageID != uint.MaxValue)
    {
      BasePage page2 = this.GetPage<BasePage>(page1.FreeEmptyPageID);
      this.AddOrRemoveToFreeList(false, page2, (BasePage) page1, ref page1.FreeEmptyPageID);
      pageID = page2.PageID;
      if (page2.DiskData.Length != 0)
        numArray = page2.DiskData;
    }
    else
    {
      pageID = ++page1.LastPageID;
      this.SetDirty((BasePage) page1);
    }
    T instance = BasePage.CreateInstance<T>(pageID);
    instance.DiskData = numArray;
    this.SetDirty((BasePage) instance);
    if (prevPage != null)
    {
      instance.PrevPageID = prevPage.PageID;
      prevPage.NextPageID = instance.PageID;
      this.SetDirty(prevPage);
    }
    return instance;
  }

  public void DeletePage(uint pageID, bool addSequence = false)
  {
    BasePage[] basePageArray;
    if (!addSequence)
      basePageArray = new BasePage[1]
      {
        this.GetPage<BasePage>(pageID)
      };
    else
      basePageArray = this.GetSeqPages<BasePage>(pageID).ToArray<BasePage>();
    HeaderPage page1 = this.GetPage<HeaderPage>(0U);
    foreach (BasePage basePage in basePageArray)
    {
      EmptyPage page2 = new EmptyPage(basePage.PageID);
      this.SetDirty((BasePage) page2);
      this.AddOrRemoveToFreeList(true, (BasePage) page2, (BasePage) page1, ref page1.FreeEmptyPageID);
    }
  }

  public T GetFreePage<T>(uint startPageID, int size) where T : BasePage
  {
    if (startPageID != uint.MaxValue)
    {
      T page = this.GetPage<T>(startPageID);
      if (page.FreeBytes >= size)
        return page;
    }
    return this.NewPage<T>();
  }

  public void AddOrRemoveToFreeList(
    bool add,
    BasePage page,
    BasePage startPage,
    ref uint fieldPageID)
  {
    if (add)
    {
      if (page.PrevPageID == uint.MaxValue && page.NextPageID == uint.MaxValue)
        this.AddToFreeList(page, startPage, ref fieldPageID);
      else
        this.MoveToFreeList(page, startPage, ref fieldPageID);
    }
    else
    {
      if (page.PrevPageID == uint.MaxValue && page.NextPageID == uint.MaxValue)
        return;
      this.RemoveToFreeList(page, startPage, ref fieldPageID);
    }
  }

  private void AddToFreeList(BasePage page, BasePage startPage, ref uint fieldPageID)
  {
    int freeBytes = page.FreeBytes;
    uint pageID = fieldPageID;
    BasePage page1 = (BasePage) null;
    for (; pageID != uint.MaxValue; pageID = page1.NextPageID)
    {
      page1 = this.GetPage<BasePage>(pageID);
      if (freeBytes >= page1.FreeBytes)
      {
        page.PrevPageID = page1.PrevPageID;
        page.NextPageID = page1.PageID;
        page1.PrevPageID = page.PageID;
        this.SetDirty(page1);
        this.SetDirty(page);
        if (page.PrevPageID == 0U)
        {
          fieldPageID = page.PageID;
          this.SetDirty(startPage);
          return;
        }
        BasePage page2 = this.GetPage<BasePage>(page.PrevPageID);
        page2.NextPageID = page.PageID;
        this.SetDirty(page2);
        return;
      }
    }
    if (page1 == null)
    {
      page.PrevPageID = 0U;
      fieldPageID = page.PageID;
      this.SetDirty(startPage);
    }
    else
    {
      page.PrevPageID = page1.PageID;
      page1.NextPageID = page.PageID;
      this.SetDirty(page1);
    }
    this.SetDirty(page);
  }

  private void RemoveToFreeList(BasePage page, BasePage startPage, ref uint fieldPageID)
  {
    if (page.PrevPageID == 0U)
    {
      fieldPageID = page.NextPageID;
      this.SetDirty(startPage);
    }
    else
    {
      BasePage page1 = this.GetPage<BasePage>(page.PrevPageID);
      page1.NextPageID = page.NextPageID;
      this.SetDirty(page1);
    }
    if (page.NextPageID != uint.MaxValue)
    {
      BasePage page2 = this.GetPage<BasePage>(page.NextPageID);
      page2.PrevPageID = page.PrevPageID;
      this.SetDirty(page2);
    }
    page.PrevPageID = page.NextPageID = uint.MaxValue;
    this.SetDirty(page);
  }

  private void MoveToFreeList(BasePage page, BasePage startPage, ref uint fieldPageID)
  {
    this.RemoveToFreeList(page, startPage, ref fieldPageID);
    this.AddToFreeList(page, startPage, ref fieldPageID);
  }
}
