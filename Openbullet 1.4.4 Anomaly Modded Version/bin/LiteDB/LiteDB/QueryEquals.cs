// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryEquals
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryEquals : Query
{
  private BsonValue _value;

  public QueryEquals(string field, BsonValue value)
    : base(field)
  {
    this._value = value;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    IndexNode node = indexer.Find(index, this._value, false, 1);
    if (node != null)
    {
      yield return node;
      if (!index.Unique)
      {
        while (!node.Next[0].IsEmpty && (node = indexer.GetNode(node.Next[0])).Key.CompareTo(this._value) == 0 && !node.IsHeadTail(index))
          yield return node;
      }
    }
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this.Expression.Execute(doc).Any<BsonValue>((Func<BsonValue, bool>) (x => x.CompareTo(this._value) == 0));
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Seek" : (object) ""))}({this.Expression?.ToString() ?? this.Field} = {this._value})";
  }
}
