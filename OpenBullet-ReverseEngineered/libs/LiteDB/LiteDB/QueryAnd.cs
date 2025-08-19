// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryAnd
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryAnd : Query
{
  private Query _left;
  private Query _right;

  public QueryAnd(Query left, Query right)
    : base((string) null)
  {
    this._left = left;
    this._right = right;
  }

  internal override bool UseFilter
  {
    get => this._left.UseFilter || this._right.UseFilter;
    set
    {
      this._left.UseFilter = value;
      this._right.UseFilter = value;
    }
  }

  internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
  {
    IEnumerable<IndexNode> first = this._left.Run(col, indexer);
    IEnumerable<IndexNode> second = this._right.Run(col, indexer);
    if (this._left.UseIndex)
    {
      this.UseIndex = true;
      this._right.UseFilter = true;
      return first;
    }
    if (this._right.UseIndex)
    {
      this.UseIndex = true;
      this._left.UseFilter = true;
      return second;
    }
    this.UseIndex = false;
    this.UseFilter = true;
    return first.Intersect<IndexNode>(second, (IEqualityComparer<IndexNode>) new IndexNodeComparer());
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    throw new NotSupportedException();
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this._left.FilterDocument(doc) && this._right.FilterDocument(doc);
  }

  public override string ToString() => $"({this._left} and {this._right})";
}
