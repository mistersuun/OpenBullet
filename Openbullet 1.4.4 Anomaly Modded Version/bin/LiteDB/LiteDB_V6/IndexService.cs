// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.IndexService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System.Collections.Generic;

#nullable disable
namespace LiteDB_V6;

internal class IndexService
{
  private PageService _pager;

  public IndexService(PageService pager) => this._pager = pager;

  public IndexNode GetNode(PageAddress address)
  {
    return address.IsEmpty ? (IndexNode) null : this._pager.GetPage<IndexPage>(address.PageID).Nodes[address.Index];
  }

  public IEnumerable<IndexNode> FindAll(CollectionIndex index)
  {
    IndexNode cur = this.GetNode(index.HeadNode);
    while (!cur.Next[0].IsEmpty)
    {
      cur = this.GetNode(cur.Next[0]);
      if (cur.IsHeadTail(index))
        break;
      yield return cur;
    }
  }
}
