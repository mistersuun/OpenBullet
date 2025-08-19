// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryNot
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryNot : Query
{
  private Query _query;
  private int _order;

  public QueryNot(Query query, int order)
    : base("_id")
  {
    this._query = query;
    this._order = order;
  }

  internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
  {
    IEnumerable<IndexNode> second = this._query.Run(col, indexer);
    this.UseIndex = this._query.UseIndex;
    this.UseFilter = this._query.UseFilter;
    return this._query.UseIndex ? new QueryAll("_id", this._order).Run(col, indexer).Except<IndexNode>(second, (IEqualityComparer<IndexNode>) new IndexNodeComparer()) : second;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    throw new NotSupportedException();
  }

  internal override bool FilterDocument(BsonDocument doc) => !this._query.FilterDocument(doc);

  public override string ToString() => $"!({this._query})";
}
