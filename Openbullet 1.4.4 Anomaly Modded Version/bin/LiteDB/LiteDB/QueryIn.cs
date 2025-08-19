// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryIn
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryIn : Query
{
  private IEnumerable<BsonValue> _values;

  public QueryIn(string field, IEnumerable<BsonValue> values)
    : base(field)
  {
    this._values = values ?? Enumerable.Empty<BsonValue>();
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    QueryIn queryIn = this;
    foreach (BsonValue bsonValue in queryIn._values.Distinct<BsonValue>())
    {
      foreach (IndexNode indexNode in Query.EQ(queryIn.Field, bsonValue).ExecuteIndex(indexer, index))
        yield return indexNode;
    }
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    foreach (BsonValue bsonValue in this.Expression.Execute(doc))
    {
      foreach (BsonValue other in this._values.Distinct<BsonValue>())
      {
        if (bsonValue.CompareTo(other) == 0)
          return true;
      }
    }
    return false;
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Seek" : (object) ""))}({this.Expression?.ToString() ?? this.Field} in {string.Join(",", this._values.Select<BsonValue, string>((Func<BsonValue, string>) (a => !(a != (BsonValue) null) ? "Null" : a.ToString())).ToArray<string>())})";
  }
}
