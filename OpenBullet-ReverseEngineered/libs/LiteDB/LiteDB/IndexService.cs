// Decompiled with JetBrains decompiler
// Type: LiteDB.IndexService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class IndexService
{
  public const int MAX_INDEX_LENGTH = 512 /*0x0200*/;
  private PageService _pager;
  private Logger _log;
  private Random _rand = new Random();

  public IndexService(PageService pager, Logger log)
  {
    this._pager = pager;
    this._log = log;
  }

  public CollectionIndex CreateIndex(CollectionPage col)
  {
    CollectionIndex freeIndex = col.GetFreeIndex();
    IndexPage page = this._pager.NewPage<IndexPage>();
    IndexNode node = new IndexNode((byte) 32 /*0x20*/)
    {
      Key = BsonValue.MinValue,
      KeyLength = (ushort) BsonValue.MinValue.GetBytesCount(false),
      Slot = (byte) freeIndex.Slot,
      Page = page
    };
    page.AddNode(node);
    this._pager.SetDirty((BasePage) freeIndex.Page);
    this._pager.AddOrRemoveToFreeList(true, (BasePage) page, (BasePage) freeIndex.Page, ref freeIndex.FreeIndexPageID);
    freeIndex.HeadNode = node.Position;
    IndexNode indexNode = this.AddNode(freeIndex, BsonValue.MaxValue, (byte) 32 /*0x20*/, (IndexNode) null);
    freeIndex.TailNode = indexNode.Position;
    return freeIndex;
  }

  public IndexNode AddNode(CollectionIndex index, BsonValue key, IndexNode last)
  {
    byte level = this.FlipCoin();
    if ((int) level > (int) index.MaxLevel)
    {
      index.MaxLevel = level;
      this._pager.SetDirty((BasePage) index.Page);
    }
    return this.AddNode(index, key, level, last);
  }

  private IndexNode AddNode(CollectionIndex index, BsonValue key, byte level, IndexNode last)
  {
    int bytesCount = key.GetBytesCount(false);
    if (bytesCount > 512 /*0x0200*/)
      throw LiteException.IndexKeyTooLong();
    IndexNode node1 = new IndexNode(level)
    {
      Key = key,
      KeyLength = (ushort) bytesCount,
      Slot = (byte) index.Slot
    };
    IndexPage freePage = this._pager.GetFreePage<IndexPage>(index.FreeIndexPageID, node1.Length);
    node1.Page = freePage;
    freePage.AddNode(node1);
    IndexNode indexNode1 = this.GetNode(index.HeadNode);
    IndexNode indexNode2 = (IndexNode) null;
    PageAddress pageAddress;
    for (int index1 = (int) index.MaxLevel - 1; index1 >= 0; --index1)
    {
      IndexNode indexNode3;
      if (indexNode2 != null)
      {
        pageAddress = indexNode2.Position;
        if (pageAddress.Equals((object) indexNode1.Next[index1]))
        {
          indexNode3 = indexNode2;
          goto label_7;
        }
      }
      indexNode3 = this.GetNode(indexNode1.Next[index1]);
label_7:
      indexNode2 = indexNode3;
      for (; !indexNode1.Next[index1].IsEmpty; indexNode1 = indexNode2)
      {
        IndexNode indexNode4;
        if (indexNode2 != null)
        {
          pageAddress = indexNode2.Position;
          if (pageAddress.Equals((object) indexNode1.Next[index1]))
          {
            indexNode4 = indexNode2;
            goto label_12;
          }
        }
        indexNode4 = this.GetNode(indexNode1.Next[index1]);
label_12:
        indexNode2 = indexNode4;
        int num = indexNode2.Key.CompareTo(key);
        if (num == 0 && index.Unique)
          throw LiteException.IndexDuplicateKey(index.Field, key);
        if (num == 1)
          break;
      }
      if (index1 <= (int) level - 1)
      {
        this._pager.SetDirty((BasePage) indexNode1.Page);
        node1.Next[index1] = indexNode1.Next[index1];
        node1.Prev[index1] = indexNode1.Position;
        indexNode1.Next[index1] = node1.Position;
        IndexNode node2 = this.GetNode(node1.Next[index1]);
        if (node2 != null)
        {
          node2.Prev[index1] = node1.Position;
          this._pager.SetDirty((BasePage) node2.Page);
        }
      }
    }
    this._pager.AddOrRemoveToFreeList(freePage.FreeBytes > 100, (BasePage) freePage, (BasePage) index.Page, ref index.FreeIndexPageID);
    if (last != null)
    {
      pageAddress = last.NextNode;
      if (!pageAddress.IsEmpty)
      {
        IndexNode node3 = this.GetNode(last.NextNode);
        node3.PrevNode = node1.Position;
        last.NextNode = node1.Position;
        node1.PrevNode = last.Position;
        node1.NextNode = node3.Position;
        this._pager.SetDirty((BasePage) node3.Page);
      }
      else
      {
        last.NextNode = node1.Position;
        node1.PrevNode = last.Position;
      }
      this._pager.SetDirty((BasePage) last.Page);
    }
    return node1;
  }

  public IEnumerable<IndexNode> GetNodeList(IndexNode node, bool includeInitial)
  {
    PageAddress next = node.NextNode;
    PageAddress prev = node.PrevNode;
    if (includeInitial)
      yield return node;
    while (!next.IsEmpty)
    {
      IndexNode node1 = this.GetNode(next);
      next = node1.NextNode;
      yield return node1;
    }
    while (!prev.IsEmpty)
    {
      IndexNode node2 = this.GetNode(prev);
      prev = node2.PrevNode;
      yield return node2;
    }
  }

  public void Delete(CollectionIndex index, PageAddress nodeAddress)
  {
    IndexNode node1 = this.GetNode(nodeAddress);
    IndexPage page = node1.Page;
    this._pager.SetDirty((BasePage) page);
    for (int index1 = node1.Prev.Length - 1; index1 >= 0; --index1)
    {
      IndexNode node2 = this.GetNode(node1.Prev[index1]);
      IndexNode node3 = this.GetNode(node1.Next[index1]);
      if (node2 != null)
      {
        node2.Next[index1] = node1.Next[index1];
        this._pager.SetDirty((BasePage) node2.Page);
      }
      if (node3 != null)
      {
        node3.Prev[index1] = node1.Prev[index1];
        this._pager.SetDirty((BasePage) node3.Page);
      }
    }
    page.DeleteNode(node1);
    if (page.NodesCount == 0)
    {
      this._pager.AddOrRemoveToFreeList(false, (BasePage) page, (BasePage) index.Page, ref index.FreeIndexPageID);
      this._pager.DeletePage(page.PageID);
    }
    else
      this._pager.AddOrRemoveToFreeList(page.FreeBytes > 100, (BasePage) node1.Page, (BasePage) index.Page, ref index.FreeIndexPageID);
    IndexNode node4 = this.GetNode(node1.PrevNode);
    IndexNode node5 = this.GetNode(node1.NextNode);
    if (node4 != null)
    {
      node4.NextNode = node1.NextNode;
      this._pager.SetDirty((BasePage) node4.Page);
    }
    if (node5 == null)
      return;
    node5.PrevNode = node1.PrevNode;
    this._pager.SetDirty((BasePage) node5.Page);
  }

  public void DropIndex(CollectionIndex index)
  {
    HashSet<uint> uintSet = new HashSet<uint>();
    foreach (IndexNode indexNode in this.FindAll(index, 1))
    {
      uintSet.Add(indexNode.Position.PageID);
      IndexNode node1 = this.GetNode(indexNode.PrevNode);
      IndexNode node2 = this.GetNode(indexNode.NextNode);
      if (node1 != null)
      {
        node1.NextNode = indexNode.NextNode;
        this._pager.SetDirty((BasePage) node1.Page);
      }
      if (node2 != null)
      {
        node2.PrevNode = indexNode.PrevNode;
        this._pager.SetDirty((BasePage) node2.Page);
      }
    }
    foreach (uint pageID in uintSet)
      this._pager.DeletePage(pageID);
  }

  public IndexNode GetNode(PageAddress address)
  {
    return address.IsEmpty ? (IndexNode) null : this._pager.GetPage<IndexPage>(address.PageID).GetNode(address.Index);
  }

  public byte FlipCoin()
  {
    byte num = 1;
    for (int index = this._rand.Next(); (index & 1) == 1; index >>= 1)
    {
      ++num;
      if (num == (byte) 32 /*0x20*/)
        break;
    }
    return num;
  }

  public IEnumerable<IndexNode> FindAll(CollectionIndex index, int order)
  {
    IndexNode cur = this.GetNode(order == 1 ? index.HeadNode : index.TailNode);
    while (!cur.NextPrev(0, order).IsEmpty)
    {
      cur = this.GetNode(cur.NextPrev(0, order));
      if (cur.IsHeadTail(index))
        break;
      yield return cur;
    }
  }

  public IndexNode Find(CollectionIndex index, BsonValue value, bool sibling, int order)
  {
    IndexNode node1 = this.GetNode(order == 1 ? index.HeadNode : index.TailNode);
    for (int index1 = (int) index.MaxLevel - 1; index1 >= 0; --index1)
    {
      for (; !node1.NextPrev(index1, order).IsEmpty; node1 = this.GetNode(node1.NextPrev(index1, order)))
      {
        IndexNode node2 = this.GetNode(node1.NextPrev(index1, order));
        int num = node2.Key.CompareTo(value);
        if (num != order || index1 <= 0 && sibling)
        {
          if (((num != order ? 0 : (index1 == 0 ? 1 : 0)) & (sibling ? 1 : 0)) != 0)
            return !node2.IsHeadTail(index) ? node2 : (IndexNode) null;
          if (num == 0)
            return index.Unique ? node2 : this.FindBoundary(index, node2, value, order * -1, index1);
        }
        else
          break;
      }
    }
    return (IndexNode) null;
  }

  private IndexNode FindBoundary(
    CollectionIndex index,
    IndexNode cur,
    BsonValue value,
    int order,
    int level)
  {
    IndexNode boundary = cur;
    while (cur.Key.CompareTo(value) == 0)
    {
      boundary = cur;
      cur = this.GetNode(cur.NextPrev(0, order));
      if (cur.IsHeadTail(index))
        break;
    }
    return boundary;
  }
}
