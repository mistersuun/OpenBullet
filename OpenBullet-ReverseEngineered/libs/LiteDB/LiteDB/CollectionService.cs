// Decompiled with JetBrains decompiler
// Type: LiteDB.CollectionService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class CollectionService
{
  private PageService _pager;
  private IndexService _indexer;
  private DataService _data;
  private TransactionService _trans;
  private Logger _log;

  public CollectionService(
    PageService pager,
    IndexService indexer,
    DataService data,
    TransactionService trans,
    Logger log)
  {
    this._pager = pager;
    this._indexer = indexer;
    this._data = data;
    this._trans = trans;
    this._log = log;
  }

  public CollectionPage Get(string name)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentNullException(nameof (name));
    uint pageID;
    return this._pager.GetPage<HeaderPage>(0U).CollectionPages.TryGetValue(name, out pageID) ? this._pager.GetPage<CollectionPage>(pageID) : (CollectionPage) null;
  }

  public CollectionPage Add(string name)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentNullException(nameof (name));
    if (!CollectionPage.NamePattern.IsMatch(name))
      throw LiteException.InvalidFormat(name);
    this._log.Write((byte) 4, "creating new collection '{0}'", (object) name);
    HeaderPage page = this._pager.GetPage<HeaderPage>(0U);
    if (page.CollectionPages.Sum<KeyValuePair<string, uint>>((Func<KeyValuePair<string, uint>, int>) (x => x.Key.Length + 8)) + name.Length + 8 >= 3000)
      throw LiteException.CollectionLimitExceeded(3000);
    CollectionPage col = this._pager.NewPage<CollectionPage>();
    page.CollectionPages.Add(name, col.PageID);
    col.CollectionName = name;
    this._pager.SetDirty((BasePage) page);
    CollectionIndex index = this._indexer.CreateIndex(col);
    index.Field = "_id";
    index.Expression = "$._id";
    index.Unique = true;
    return col;
  }

  public IEnumerable<CollectionPage> GetAll()
  {
    foreach (uint pageID in this._pager.GetPage<HeaderPage>(0U).CollectionPages.Values)
      yield return this._pager.GetPage<CollectionPage>(pageID);
  }

  public void Drop(CollectionPage col)
  {
    HashSet<uint> uintSet = new HashSet<uint>();
    foreach (CollectionIndex index in col.GetIndexes(true))
    {
      foreach (IndexNode indexNode in this._indexer.FindAll(index, 1))
      {
        if (index.Slot == 0)
        {
          uintSet.Add(indexNode.DataBlock.PageID);
          DataBlock block = this._data.GetBlock(indexNode.DataBlock);
          if (block.ExtendPageID != uint.MaxValue)
            this._pager.DeletePage(block.ExtendPageID, true);
        }
        this._trans.CheckPoint();
        uintSet.Add(indexNode.Position.PageID);
      }
      uintSet.Add(index.HeadNode.PageID);
      uintSet.Add(index.TailNode.PageID);
    }
    foreach (uint pageID in uintSet)
    {
      this._pager.DeletePage(pageID);
      this._trans.CheckPoint();
    }
    HeaderPage page = this._pager.GetPage<HeaderPage>(0U);
    page.CollectionPages.Remove(col.CollectionName);
    this._pager.SetDirty((BasePage) page);
    this._pager.DeletePage(col.PageID);
  }
}
