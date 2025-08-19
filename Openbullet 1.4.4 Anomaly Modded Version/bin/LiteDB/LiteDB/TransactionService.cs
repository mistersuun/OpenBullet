// Decompiled with JetBrains decompiler
// Type: LiteDB.TransactionService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class TransactionService
{
  private IDiskService _disk;
  private AesEncryption _crypto;
  private LockService _locker;
  private PageService _pager;
  private CacheService _cache;
  private Logger _log;
  private int _cacheSize;

  internal TransactionService(
    IDiskService disk,
    AesEncryption crypto,
    PageService pager,
    LockService locker,
    CacheService cache,
    int cacheSize,
    Logger log)
  {
    this._disk = disk;
    this._crypto = crypto;
    this._cache = cache;
    this._locker = locker;
    this._pager = pager;
    this._cacheSize = cacheSize;
    this._log = log;
  }

  public bool CheckPoint()
  {
    if (this._cache.CleanUsed <= this._cacheSize)
      return false;
    this._log.Write((byte) 64 /*0x40*/, "cache size reached {0} pages, will clear now", (object) this._cache.CleanUsed);
    this._cache.ClearPages();
    return true;
  }

  public void PersistDirtyPages()
  {
    HeaderPage page = this._pager.GetPage<HeaderPage>(0U);
    page.ChangeID = page.ChangeID == ushort.MaxValue ? (ushort) 0 : (ushort) ((uint) page.ChangeID + 1U);
    this._pager.SetDirty((BasePage) page);
    this._log.Write((byte) 128 /*0x80*/, "begin disk operations - changeID: {0}", (object) page.ChangeID);
    if (this._disk.IsJournalEnabled)
    {
      this._disk.WriteJournal((ICollection<byte[]>) this._cache.GetDirtyPages().OrderByDescending<BasePage, uint>((Func<BasePage, uint>) (x => x.PageID)).Select<BasePage, byte[]>((Func<BasePage, byte[]>) (x => x.DiskData)).Where<byte[]>((Func<byte[], bool>) (x => x.Length != 0)).ToList<byte[]>(), page.LastPageID);
      page.Recovery = true;
    }
    else
      this._disk.SetLength(BasePage.GetSizeOfPages(page.LastPageID + 1U));
    foreach (BasePage dirtyPage in (IEnumerable<BasePage>) this._cache.GetDirtyPages())
    {
      byte[] buffer = this._crypto == null || dirtyPage.PageID == 0U ? dirtyPage.WritePage() : this._crypto.Encrypt(dirtyPage.WritePage());
      this._disk.WritePage(dirtyPage.PageID, buffer);
    }
    if (this._disk.IsJournalEnabled)
    {
      page.Recovery = false;
      this._log.Write((byte) 128 /*0x80*/, "re-write header page now with recovery = false");
      this._disk.WritePage(0U, page.WritePage());
    }
    this._cache.MarkDirtyAsClean();
    this._disk.Flush();
    this._disk.ClearJournal(page.LastPageID);
  }

  public void Recovery()
  {
    this._log.Write((byte) 2, "initializing recovery mode");
    using (this._locker.Write())
    {
      HeaderPage headerPage = BasePage.ReadPage(this._disk.ReadPage(0U)) as HeaderPage;
      if (!headerPage.Recovery)
        return;
      foreach (byte[] bytes in this._disk.ReadJournal(headerPage.LastPageID))
      {
        uint uint32 = BitConverter.ToUInt32(bytes, 0);
        this._log.Write((byte) 2, "recover page #{0:0000}", (object) uint32);
        this._disk.WritePage(uint32, this._crypto == null || uint32 == 0U ? bytes : this._crypto.Encrypt(bytes));
      }
      this._disk.ClearJournal(headerPage.LastPageID);
    }
  }
}
