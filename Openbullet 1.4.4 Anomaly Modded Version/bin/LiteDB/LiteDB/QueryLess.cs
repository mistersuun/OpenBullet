// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryLess
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace LiteDB;

internal class QueryLess : Query
{
  private BsonValue _value;
  private bool _equals;

  public BsonValue Value => this._value;

  public bool IsEquals => this._equals;

  public QueryLess(string field, BsonValue value, bool equals)
    : base(field)
  {
    this._value = value;
    this._equals = equals;
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    foreach (IndexNode indexNode in indexer.FindAll(index, 1))
    {
      if (indexNode.Key.Type == this._value.Type || indexNode.Key.IsNumber && this._value.IsNumber)
      {
        int num = indexNode.Key.CompareTo(this._value);
        if (num != 1 && (this._equals || num != 0))
        {
          if (indexNode.IsHeadTail(index))
            break;
          yield return indexNode;
        }
        else
          break;
      }
    }
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this.Expression.Execute(doc).Where<BsonValue>((Func<BsonValue, bool>) (x =>
    {
      if (x.Type == this._value.Type)
        return true;
      return x.IsNumber && this._value.IsNumber;
    })).Any<BsonValue>((Func<BsonValue, bool>) (x => x.CompareTo(this._value) <= (this._equals ? 0 : -1)));
  }

  public override string ToString()
  {
    return $"{(this.UseFilter ? (object) "Filter" : (this.UseIndex ? (object) "Seek" : (object) ""))}({this.Expression?.ToString() ?? this.Field} <{(this._equals ? (object) "=" : (object) "")} {this._value})";
  }
}
