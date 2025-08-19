// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryBetween
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryBetween : Query
{
  private BsonValue _start;
  private BsonValue _end;
  private bool _startEquals;
  private bool _endEquals;

  public QueryBetween(
    string field,
    BsonValue start,
    BsonValue end,
    bool startEquals,
    bool endEquals)
    : base(field)
  {
    this._start = start;
    this._startEquals = startEquals;
    this._end = end;
    this._endEquals = endEquals;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    int order = this._start.CompareTo(this._end) <= 0 ? 1 : -1;
    IndexNode node;
    for (node = indexer.Find(index, this._start, true, order); node != null && node.Key.CompareTo(this._start) == 0; node = indexer.GetNode(node.NextPrev(0, order)))
    {
      if (this._startEquals)
        yield return node;
    }
    for (; node != null; node = indexer.GetNode(node.NextPrev(0, order)))
    {
      int diff = node.Key.CompareTo(this._end);
      if (this._endEquals && diff == 0)
      {
        yield return node;
      }
      else
      {
        if (diff != -order)
          break;
        yield return node;
      }
    }
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this.Expression.Execute(doc, false).Any<BsonValue>((Func<BsonValue, bool>) (x =>
    {
      if ((this._startEquals ? (x.CompareTo(this._start) >= 0 ? 1 : 0) : (x.CompareTo(this._start) > 0 ? 1 : 0)) == 0)
        return false;
      return !this._endEquals ? x.CompareTo(this._end) < 0 : x.CompareTo(this._end) <= 0;
    }));
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "IndexSeek" : (object) ""))}({this.Expression?.ToString() ?? this.Field} between {(this._startEquals ? (object) "[" : (object) "(")}{this._start} and {this._end}{(this._endEquals ? (object) "]" : (object) ")")})";
  }
}
