// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.PageService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB_V6;

internal class PageService
{
  private FileDiskService _disk;
  private Dictionary<uint, BasePage> _cache = new Dictionary<uint, BasePage>();

  public PageService(FileDiskService disk) => this._disk = disk;

  public T GetPage<T>(uint pageID) where T : BasePage
  {
    BasePage page1;
    if (this._cache.TryGetValue(pageID, out page1))
      return (T) page1;
    if (this._cache.Count > 5000)
      this._cache.Clear();
    BasePage page2 = BasePage.ReadPage(this._disk.ReadPage(pageID));
    this._cache[pageID] = page2;
    return (T) page2;
  }

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
}
