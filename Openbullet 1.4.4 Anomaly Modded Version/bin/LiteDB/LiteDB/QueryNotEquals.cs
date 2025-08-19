// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryNotEquals
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryNotEquals : Query
{
  private BsonValue _value;

  public QueryNotEquals(string field, BsonValue value)
    : base(field)
  {
    this._value = value;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    return indexer.FindAll(index, 1).Where<IndexNode>((Func<IndexNode, bool>) (x => x.Key.CompareTo(this._value) != 0));
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this.Expression.Execute(doc).Any<BsonValue>((Func<BsonValue, bool>) (x => x.CompareTo(this._value) != 0));
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Scan" : (object) ""))}({this.Expression?.ToString() ?? this.Field} != {this._value})";
  }
}
