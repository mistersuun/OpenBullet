// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryLinq`1
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

internal class QueryLinq<T> : Query
{
  private System.Linq.Expressions.Expression _expr;
  private BsonMapper _mapper;
  private Func<T, bool> _where;

  public QueryLinq(System.Linq.Expressions.Expression expr, ParameterExpression p, BsonMapper mapper)
    : base((string) null)
  {
    this._where = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(expr, p).Compile();
    this._mapper = mapper;
    this._expr = expr;
  }

  internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
  {
    this.UseIndex = false;
    this.UseFilter = true;
    return Query.All().Run(col, indexer);
  }

  internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
  {
    throw new NotSupportedException();
  }

  internal override bool FilterDocument(BsonDocument doc)
  {
    return this._where(this._mapper.ToObject<T>(doc));
  }

  public override string ToString() => $"Linq({this._expr})";
}
