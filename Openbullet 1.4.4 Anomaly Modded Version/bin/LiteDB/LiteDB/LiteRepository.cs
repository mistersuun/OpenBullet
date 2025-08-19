// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteRepository
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

public class LiteRepository : IDisposable
{
  private LiteDatabase _db;
  private readonly bool _disposeDatabase;

  public LiteDatabase Database => this._db;

  public LiteEngine Engine => this._db.Engine;

  public LiteRepository(LiteDatabase database, bool disposeDatabase = false)
  {
    this._db = database;
    this._disposeDatabase = disposeDatabase;
  }

  public LiteRepository(string connectionString, BsonMapper mapper = null)
    : this(new LiteDatabase(connectionString, mapper), true)
  {
  }

  public LiteRepository(ConnectionString connectionString, BsonMapper mapper = null)
    : this(new LiteDatabase(connectionString, mapper), true)
  {
  }

  public LiteRepository(Stream stream, BsonMapper mapper = null, string password = null)
    : this(new LiteDatabase(stream, mapper, password), true)
  {
  }

  public LiteStorage FileStorage => this._db.FileStorage;

  public BsonValue Insert<T>(T entity, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Insert(entity);
  }

  public int Insert<T>(IEnumerable<T> entities, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Insert(entities);
  }

  public bool Update<T>(T entity, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Update(entity);
  }

  public int Update<T>(IEnumerable<T> entities, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Update(entities);
  }

  public bool Upsert<T>(T entity, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Upsert(entity);
  }

  public int Upsert<T>(IEnumerable<T> entities, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Upsert(entities);
  }

  public bool Delete<T>(BsonValue id, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Delete(id);
  }

  public int Delete<T>(LiteDB.Query query, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Delete(query);
  }

  public int Delete<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
  {
    return this._db.GetCollection<T>(collectionName).Delete(predicate);
  }

  public LiteQueryable<T> Query<T>(string collectionName = null)
  {
    return new LiteQueryable<T>(this._db.GetCollection<T>(collectionName));
  }

  public T SingleById<T>(BsonValue id, string collectionName = null)
  {
    return this.Query<T>(collectionName).SingleById(id);
  }

  public List<T> Fetch<T>(LiteDB.Query query = null, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(query ?? LiteDB.Query.All()).ToList();
  }

  public List<T> Fetch<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(predicate).ToList();
  }

  public T First<T>(LiteDB.Query query = null, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(query ?? LiteDB.Query.All()).First();
  }

  public T First<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(predicate).First();
  }

  public T FirstOrDefault<T>(LiteDB.Query query = null, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(query ?? LiteDB.Query.All()).FirstOrDefault();
  }

  public T FirstOrDefault<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(predicate).FirstOrDefault();
  }

  public T Single<T>(LiteDB.Query query = null, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(query ?? LiteDB.Query.All()).Single();
  }

  public T Single<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(predicate).Single();
  }

  public T SingleOrDefault<T>(LiteDB.Query query = null, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(query ?? LiteDB.Query.All()).SingleOrDefault();
  }

  public T SingleOrDefault<T>(Expression<Func<T, bool>> predicate, string collectionName = null)
  {
    return this.Query<T>(collectionName).Where(predicate).SingleOrDefault();
  }

  public void Dispose()
  {
    if (this._disposeDatabase)
      this._db?.Dispose();
    this._db = (LiteDatabase) null;
  }
}
