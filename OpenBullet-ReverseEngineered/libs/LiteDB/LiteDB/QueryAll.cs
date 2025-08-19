// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryAll
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class QueryAll : Query
{
  private int _order;

  public QueryAll(string field, int order)
    : base(field)
  {
    this._order = order;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    return indexer.FindAll(index, this._order);
  }

  internal override bool FilterDocument(BsonDocument doc) => true;

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Scan" : (object) ""))}({this.Expression?.ToString() ?? this.Field})";
  }
}
