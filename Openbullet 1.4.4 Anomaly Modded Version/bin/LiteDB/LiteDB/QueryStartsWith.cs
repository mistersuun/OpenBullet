// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryStartsWith
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryStartsWith : Query
{
  private BsonValue _value;

  public QueryStartsWith(string field, BsonValue value)
    : base(field)
  {
    this._value = value;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    IndexNode node = indexer.Find(index, this._value, true, 1);
    for (string str = this._value.AsString; node != null && node.Key.AsString.StartsWith(str); node = indexer.GetNode(node.Next[0]))
    {
      if (!node.DataBlock.IsEmpty)
        yield return node;
    }
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this.Expression.Execute(doc, false).Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsString)).Any<BsonValue>((Func<BsonValue, bool>) (x => x.AsString.StartsWith((string) this._value)));
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Seek" : (object) ""))}({this.Expression?.ToString() ?? this.Field} startsWith {this._value})";
  }
}
