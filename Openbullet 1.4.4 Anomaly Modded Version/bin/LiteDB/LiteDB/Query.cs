// Decompiled with JetBrains decompiler
// Type: LiteDB.Query
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

public abstract class Query
{
  public const int Ascending = 1;
  public const int Descending = -1;

  public string Field { get; private set; }

  internal BsonExpression Expression { get; set; }

  internal virtual bool UseIndex { get; set; }

  internal virtual bool UseFilter { get; set; }

  internal Query(string field) => this.Field = field;

  public static Query All(int order = 1) => (Query) new QueryAll("_id", order);

  public static Query All(string field, int order = 1)
  {
    return !field.IsNullOrWhiteSpace() ? (Query) new QueryAll(field, order) : throw new ArgumentNullException(nameof (field));
  }

  public static Query EQ(string field, BsonValue value)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    return (Query) new QueryEquals(field1, bsonValue);
  }

  public static Query LT(string field, BsonValue value)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    return (Query) new QueryLess(field1, bsonValue, false);
  }

  public static Query LTE(string field, BsonValue value)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    return (Query) new QueryLess(field1, bsonValue, true);
  }

  public static Query GT(string field, BsonValue value)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    return (Query) new QueryGreater(field1, bsonValue, false);
  }

  public static Query GTE(string field, BsonValue value)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    return (Query) new QueryGreater(field1, bsonValue, true);
  }

  public static Query Between(
    string field,
    BsonValue start,
    BsonValue end,
    bool startEquals = true,
    bool endEquals = true)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue start1 = start;
    if ((object) start1 == null)
      start1 = BsonValue.Null;
    BsonValue end1 = end;
    if ((object) end1 == null)
      end1 = BsonValue.Null;
    int num1 = startEquals ? 1 : 0;
    int num2 = endEquals ? 1 : 0;
    return (Query) new QueryBetween(field1, start1, end1, num1 != 0, num2 != 0);
  }

  public static Query StartsWith(string field, string value)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    return !value.IsNullOrWhiteSpace() ? (Query) new QueryStartsWith(field, (BsonValue) value) : throw new ArgumentNullException(nameof (value));
  }

  public static Query Contains(string field, string value)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    return !value.IsNullOrWhiteSpace() ? (Query) new QueryContains(field, (BsonValue) value) : throw new ArgumentNullException(nameof (value));
  }

  public static Query Not(string field, BsonValue value)
  {
    string field1 = !field.IsNullOrWhiteSpace() ? field : throw new ArgumentNullException(nameof (field));
    BsonValue bsonValue = value;
    if ((object) bsonValue == null)
      bsonValue = BsonValue.Null;
    return (Query) new QueryNotEquals(field1, bsonValue);
  }

  public static Query Not(Query query, int order = 1)
  {
    return query != null ? (Query) new QueryNot(query, order) : throw new ArgumentNullException(nameof (query));
  }

  public static Query In(string field, BsonArray value)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    if ((BsonValue) value == (BsonValue) null)
      throw new ArgumentNullException(nameof (value));
    return (Query) new QueryIn(field, (IEnumerable<BsonValue>) value.RawValue);
  }

  public static Query In(string field, params BsonValue[] values)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    return values != null ? (Query) new QueryIn(field, (IEnumerable<BsonValue>) values) : throw new ArgumentNullException(nameof (values));
  }

  public static Query In(string field, IEnumerable<BsonValue> values)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    return values != null ? (Query) new QueryIn(field, values) : throw new ArgumentNullException(nameof (values));
  }

  public static Query Where(string field, Func<BsonValue, bool> predicate, int order = 1)
  {
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    if (predicate == null)
      throw new ArgumentNullException(nameof (predicate));
    return (Query) new QueryWhere(field, predicate, order);
  }

  public static Query And(Query left, Query right)
  {
    if (left == null)
      throw new ArgumentNullException(nameof (left));
    if (right == null)
      throw new ArgumentNullException(nameof (right));
    if (!(left is QueryGreater) || !(right is QueryLess) || !(left.Field == right.Field))
      return (Query) new QueryAnd(left, right);
    QueryGreater queryGreater = left as QueryGreater;
    QueryLess queryLess = right as QueryLess;
    return Query.Between(queryGreater.Field, queryGreater.Value, queryLess.Value, queryGreater.IsEquals, queryLess.IsEquals);
  }

  public static Query And(params Query[] queries)
  {
    Query left = queries != null && queries.Length >= 2 ? queries[0] : throw new ArgumentException("At least two Query should be passed");
    for (int index = 1; index < queries.Length; ++index)
      left = Query.And(left, queries[index]);
    return left;
  }

  public static Query Or(Query left, Query right)
  {
    if (left == null)
      throw new ArgumentNullException(nameof (left));
    return right != null ? (Query) new QueryOr(left, right) : throw new ArgumentNullException(nameof (right));
  }

  public static Query Or(params Query[] queries)
  {
    Query left = queries != null && queries.Length >= 2 ? queries[0] : throw new ArgumentException("At least two Query should be passed");
    for (int index = 1; index < queries.Length; ++index)
      left = Query.Or(left, queries[index]);
    return left;
  }

  internal virtual IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
  {
    CollectionIndex index = col.GetIndex(this.Field);
    if (index == null)
    {
      this.UseFilter = true;
      this.Expression = new BsonExpression(this.Field.StartsWith("$") || this.Field.IndexOf("(") > 0 ? this.Field : "$." + this.Field);
      return indexer.FindAll(col.PK, 1);
    }
    this.UseIndex = true;
    this.Expression = new BsonExpression(index.Expression);
    return this.ExecuteIndex(indexer, index).DistinctBy<IndexNode, PageAddress>((Func<IndexNode, PageAddress>) (x => x.DataBlock), (IEqualityComparer<PageAddress>) null);
  }

  internal abstract IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index);

  internal abstract bool FilterDocument(BsonDocument doc);
}
