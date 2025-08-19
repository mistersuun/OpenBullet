// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteQueryable`1
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

public class LiteQueryable<T>
{
  private int _limit = int.MaxValue;
  private int _skip;
  private LiteCollection<T> _collection;
  private List<Action<T, int>> _foreach;
  private Query _query;

  internal LiteQueryable(LiteCollection<T> collection)
  {
    this._collection = collection;
    this._foreach = new List<Action<T, int>>();
    this._query = (Query) null;
  }

  public LiteQueryable<T> Include<K>(Expression<Func<T, K>> dbref)
  {
    this._collection = this._collection.Include<K>(dbref);
    return this;
  }

  public LiteQueryable<T> Include(string path)
  {
    this._collection = this._collection.Include(path);
    return this;
  }

  public LiteQueryable<T> ForEach(Action<T> action)
  {
    this._foreach.Add((Action<T, int>) ((a, i) => action(a)));
    return this;
  }

  public LiteQueryable<T> ForEach(Action<T, int> action)
  {
    this._foreach.Add(action);
    return this;
  }

  public LiteQueryable<T> Where(Query query)
  {
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    this._query = this._query == null ? query : Query.And(this._query, query);
    return this;
  }

  public LiteQueryable<T> Where(Expression<Func<T, bool>> predicate)
  {
    return this.Where(this._collection.Visitor.Visit(predicate));
  }

  public LiteQueryable<T> Where(bool condition, Query query)
  {
    return !condition ? this : this.Where(query);
  }

  public LiteQueryable<T> Where(bool condition, Expression<Func<T, bool>> predicate)
  {
    return !condition ? this : this.Where(predicate);
  }

  public LiteQueryable<T> Skip(int skip)
  {
    this._skip = skip;
    return this;
  }

  public LiteQueryable<T> Limit(int limit)
  {
    this._limit = limit;
    return this;
  }

  public T Single() => this.ToEnumerable().Single<T>();

  public T SingleOrDefault() => this.ToEnumerable().SingleOrDefault<T>();

  public T First() => this.ToEnumerable().First<T>();

  public T FirstOrDefault() => this.ToEnumerable().FirstOrDefault<T>();

  public T SingleById(BsonValue id)
  {
    this._query = Query.EQ("_id", id);
    return this.ToEnumerable().Single<T>();
  }

  public IEnumerable<T> ToEnumerable()
  {
    foreach (T obj in this._collection.Find(this._query ?? Query.All(), this._skip, this._limit))
    {
      int num = 0;
      foreach (Action<T, int> action in this._foreach)
        action(obj, num++);
      yield return obj;
    }
  }

  public List<T> ToList() => this.ToEnumerable().ToList<T>();

  public T[] ToArray() => this.ToEnumerable().ToArray<T>();

  public int Count()
  {
    return this._query != null ? this._collection.Count(this._query) : this._collection.Count();
  }

  public bool Exists() => this._collection.Exists(this._query ?? Query.All());
}
