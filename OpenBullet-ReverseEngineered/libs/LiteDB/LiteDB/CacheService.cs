// Decompiled with JetBrains decompiler
// Type: LiteDB.CacheService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class CacheService
{
  private Dictionary<uint, BasePage> _clean = new Dictionary<uint, BasePage>();
  private Dictionary<uint, BasePage> _dirty = new Dictionary<uint, BasePage>();
  private IDiskService _disk;
  private Logger _log;

  public CacheService(IDiskService disk, Logger log)
  {
    this._disk = disk;
    this._log = log;
  }

  public BasePage GetPage(uint pageID)
  {
    return this._dirty.GetOrDefault<uint, BasePage>(pageID) ?? this._clean.GetOrDefault<uint, BasePage>(pageID);
  }

  public void AddPage(BasePage page)
  {
    this._clean[page.PageID] = !page.IsDirty ? page : throw new NotSupportedException("Page can't be dirty");
  }

  public void SetDirty(BasePage page)
  {
    this._clean.Remove(page.PageID);
    page.IsDirty = true;
    this._dirty[page.PageID] = page;
  }

  public ICollection<BasePage> GetDirtyPages() => (ICollection<BasePage>) this._dirty.Values;

  public int CleanUsed => this._clean.Count;

  public int DirtyUsed => this._dirty.Count;

  public void DiscardDirtyPages()
  {
    this._log.Write((byte) 64 /*0x40*/, "clearing dirty pages from cache");
    this._dirty.Clear();
  }

  public void MarkDirtyAsClean()
  {
    foreach (KeyValuePair<uint, BasePage> keyValuePair in this._dirty)
    {
      keyValuePair.Value.IsDirty = false;
      this._clean[keyValuePair.Key] = keyValuePair.Value;
    }
    this._dirty.Clear();
  }

  public void ClearPages()
  {
    lock (this._clean)
    {
      this._log.Write((byte) 64 /*0x40*/, "cleaning cache");
      this._clean.Clear();
    }
  }
}
