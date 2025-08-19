// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryWhere
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryWhere : Query
{
  private Func<BsonValue, bool> _func;
  private int _order;

  public QueryWhere(string field, Func<BsonValue, bool> func, int order)
    : base(field)
  {
    this._func = func;
    this._order = order;
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this.Expression.Execute(doc).Any<BsonValue>((Func<BsonValue, bool>) (x => this._func(x)));
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    return indexer.FindAll(index, this._order).Where<IndexNode>((Func<IndexNode, bool>) (i => this._func(i.Key)));
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Scan" : (object) ""))}({this._func.ToString()}[{this.Expression?.ToString() ?? this.Field}])";
  }
}
