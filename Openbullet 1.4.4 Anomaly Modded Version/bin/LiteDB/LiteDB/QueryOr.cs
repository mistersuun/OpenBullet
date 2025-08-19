// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryOr
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryOr : Query
{
  private Query _left;
  private Query _right;

  public QueryOr(Query left, Query right)
    : base((string) null)
  {
    this._left = left;
    this._right = right;
  }

  internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
  {
    IEnumerable<IndexNode> first = this._left.Run(col, indexer);
    IEnumerable<IndexNode> indexNodes = this._right.Run(col, indexer);
    this.UseIndex = this._left.UseIndex && this._right.UseIndex;
    this.UseFilter = this._left.UseFilter || this._right.UseFilter;
    IEnumerable<IndexNode> second = indexNodes;
    IndexNodeComparer comparer = new IndexNodeComparer();
    return first.Union<IndexNode>(second, (IEqualityComparer<IndexNode>) comparer);
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    throw new NotSupportedException();
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this._left.FilterDocument(doc) || this._right.FilterDocument(doc);
  }

  public override string ToString() => $"({this._left} or {this._right})";
}
