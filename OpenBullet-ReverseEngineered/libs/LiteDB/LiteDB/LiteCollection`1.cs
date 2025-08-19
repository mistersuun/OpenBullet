// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteCollection`1
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace LiteDB;

public sealed class LiteCollection<T>
{
  private string _name;
  private LazyLoad<LiteEngine> _engine;
  private BsonMapper _mapper;
  private Logger _log;
  private List<string> _includes;
  private QueryVisitor<T> _visitor;
  private MemberMapper _id;
  private BsonType _autoId = BsonType.Null;

  public int Count() => (int) this._engine.Value.Count(this._name);

  public int Count(Query query)
  {
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    return (int) this._engine.Value.Count(this._name, query);
  }

  public int Count(Expression<Func<T, bool>> predicate)
  {
    return predicate != null ? this.Count(this._visitor.Visit(predicate)) : throw new ArgumentNullException(nameof (predicate));
  }

  public long LongCount() => this._engine.Value.Count(this._name);

  public long LongCount(Query query)
  {
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    return this._engine.Value.Count(this._name, query);
  }

  public long LongCount(Expression<Func<T, bool>> predicate)
  {
    return predicate != null ? this.LongCount(this._visitor.Visit(predicate)) : throw new ArgumentNullException(nameof (predicate));
  }

  public bool Exists(Query query)
  {
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    return this._engine.Value.Exists(this._name, query);
  }

  public bool Exists(Expression<Func<T, bool>> predicate)
  {
    return predicate != null ? this.Exists(this._visitor.Visit(predicate)) : throw new ArgumentNullException(nameof (predicate));
  }

  public BsonValue Min(string field)
  {
    if (string.IsNullOrEmpty(field))
      throw new ArgumentNullException(nameof (field));
    return this._engine.Value.Min(this._name, field);
  }

  public BsonValue Min() => this.Min("_id");

  public BsonValue Min<K>(Expression<Func<T, K>> property)
  {
    return property != null ? this.Min(this._visitor.GetField((Expression) property)) : throw new ArgumentNullException(nameof (property));
  }

  public BsonValue Max(string field)
  {
    if (string.IsNullOrEmpty(field))
      throw new ArgumentNullException(nameof (field));
    return this._engine.Value.Max(this._name, field);
  }

  public BsonValue Max() => this.Max("_id");

  public BsonValue Max<K>(Expression<Func<T, K>> property)
  {
    return property != null ? this.Max(this._visitor.GetField((Expression) property)) : throw new ArgumentNullException(nameof (property));
  }

  public int Delete(Query query)
  {
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    return this._engine.Value.Delete(this._name, query);
  }

  public int Delete(Expression<Func<T, bool>> predicate)
  {
    return this.Delete(this._visitor.Visit(predicate));
  }

  public bool Delete(BsonValue id)
  {
    if (id == (BsonValue) null || id.IsNull)
      throw new ArgumentNullException(nameof (id));
    return this.Delete(Query.EQ("_id", id)) > 0;
  }

  public IEnumerable<T> Find(Query query, int skip = 0, int limit = 2147483647 /*0x7FFFFFFF*/)
  {
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    foreach (BsonDocument doc in this._engine.Value.Find(this._name, query, this._includes.ToArray(), skip, limit))
      yield return this._mapper.ToObject<T>(doc);
  }

  public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 2147483647 /*0x7FFFFFFF*/)
  {
    if (predicate == null)
      throw new ArgumentNullException(nameof (predicate));
    return this.Find(this._visitor.Visit(predicate), skip, limit);
  }

  public T FindById(BsonValue id)
  {
    return !(id == (BsonValue) null) && !id.IsNull ? this.Find(Query.EQ("_id", id)).SingleOrDefault<T>() : throw new ArgumentNullException(nameof (id));
  }

  public T FindOne(Query query) => this.Find(query).FirstOrDefault<T>();

  public T FindOne(Expression<Func<T, bool>> predicate) => this.Find(predicate).FirstOrDefault<T>();

  public IEnumerable<T> FindAll() => this.Find(Query.All());

  public LiteCollection<T> Include<K>(Expression<Func<T, K>> path)
  {
    return path != null ? this.Include(this._visitor.GetPath((Expression) path)) : throw new ArgumentNullException(nameof (path));
  }

  public LiteCollection<T> Include(string path)
  {
    return !string.IsNullOrEmpty(path) ? this.Include(new string[1]
    {
      path
    }) : throw new ArgumentNullException(nameof (path));
  }

  public LiteCollection<T> Include(string[] paths)
  {
    if (paths == null)
      throw new ArgumentNullException(nameof (paths));
    LiteCollection<T> liteCollection = new LiteCollection<T>(this._name, this._engine, this._mapper, this._log);
    liteCollection._includes.AddRange((IEnumerable<string>) this._includes);
    liteCollection._includes.AddRange(((IEnumerable<string>) paths).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))));
    return liteCollection;
  }

  public LiteCollection<T> IncludeAll(int maxDepth = -1)
  {
    return this.Include(this.GetRecursivePaths(typeof (T), maxDepth, 0));
  }

  private string[] GetRecursivePaths(
    Type pathType,
    int maxDepth,
    int currentDepth,
    string basePath = null)
  {
    ++currentDepth;
    List<string> stringList = new List<string>();
    if (maxDepth < 0 || currentDepth <= maxDepth)
    {
      IEnumerable<MemberMapper> memberMappers = this._mapper.GetEntityMapper(pathType).Members.Where<MemberMapper>((Func<MemberMapper, bool>) (x => x.IsDbRef));
      basePath = string.IsNullOrEmpty(basePath) ? "$" : basePath;
      foreach (MemberMapper memberMapper in memberMappers)
      {
        string basePath1 = memberMapper.IsList ? $"{basePath}.{memberMapper.FieldName}[*]" : $"{basePath}.{memberMapper.FieldName}";
        stringList.Add(basePath1);
        stringList.AddRange((IEnumerable<string>) this.GetRecursivePaths(memberMapper.UnderlyingType, maxDepth, currentDepth, basePath1));
      }
    }
    return stringList.ToArray();
  }

  public bool EnsureIndex(string field, bool unique = false)
  {
    return this.EnsureIndex(field, (string) null, unique);
  }

  public bool EnsureIndex(string field, string expression, bool unique = false)
  {
    if (string.IsNullOrEmpty(field))
      throw new ArgumentNullException(nameof (field));
    return this._engine.Value.EnsureIndex(this._name, field, expression, unique);
  }

  public bool EnsureIndex<K>(Expression<Func<T, K>> property, bool unique = false)
  {
    return this.EnsureIndex<K>(property, (string) null, unique);
  }

  public bool EnsureIndex<K>(Expression<Func<T, K>> property, string expression, bool unique = false)
  {
    return this.EnsureIndex(this._visitor.GetField((Expression) property), expression ?? this._visitor.GetPath((Expression) property), unique);
  }

  public IEnumerable<IndexInfo> GetIndexes() => this._engine.Value.GetIndexes(this._name);

  public bool DropIndex(string field) => this._engine.Value.DropIndex(this._name, field);

  public BsonValue Insert(T document)
  {
    BsonDocument doc = (object) document != null ? this._mapper.ToDocument<T>(document) : throw new ArgumentNullException(nameof (document));
    int num = this.RemoveDocId(doc) ? 1 : 0;
    BsonValue bsonValue = this._engine.Value.Insert(this._name, doc, this._autoId);
    if (num != 0 && this._id != null)
      this._id.Setter((object) document, bsonValue.RawValue);
    return bsonValue;
  }

  public void Insert(BsonValue id, T document)
  {
    if ((object) document == null)
      throw new ArgumentNullException(nameof (document));
    if (id == (BsonValue) null || id.IsNull)
      throw new ArgumentNullException(nameof (id));
    BsonDocument document1 = this._mapper.ToDocument<T>(document);
    document1["_id"] = id;
    this._engine.Value.Insert(this._name, document1);
  }

  public int Insert(IEnumerable<T> docs)
  {
    if (docs == null)
      throw new ArgumentNullException(nameof (docs));
    return this._engine.Value.Insert(this._name, this.GetBsonDocs(docs), this._autoId);
  }

  public int InsertBulk(IEnumerable<T> docs, int batchSize = 5000)
  {
    if (docs == null)
      throw new ArgumentNullException(nameof (docs));
    return this._engine.Value.InsertBulk(this._name, this.GetBsonDocs(docs), batchSize, this._autoId);
  }

  private IEnumerable<BsonDocument> GetBsonDocs(IEnumerable<T> documents)
  {
    foreach (T document1 in documents)
    {
      T document = document1;
      BsonDocument doc = this._mapper.ToDocument<T>(document);
      bool removed = this.RemoveDocId(doc);
      yield return doc;
      if (removed && this._id != null)
        this._id.Setter((object) document, doc["_id"].RawValue);
      doc = (BsonDocument) null;
      document = default (T);
    }
  }

  private bool RemoveDocId(BsonDocument doc)
  {
    BsonValue bsonValue;
    if (!doc.TryGetValue("_id", out bsonValue) || (this._autoId != BsonType.ObjectId || !bsonValue.IsNull && !(bsonValue.AsObjectId == ObjectId.Empty)) && (this._autoId != BsonType.Guid || !(bsonValue.AsGuid == Guid.Empty)) && (this._autoId != BsonType.DateTime || !(bsonValue.AsDateTime == DateTime.MinValue)) && (this._autoId != BsonType.Int32 || bsonValue.AsInt32 != 0) && (this._autoId != BsonType.Int64 || bsonValue.AsInt64 != 0L))
      return false;
    doc.Remove("_id");
    return true;
  }

  public string Name => this._name;

  internal QueryVisitor<T> Visitor => this._visitor;

  public LiteCollection(string name, LazyLoad<LiteEngine> engine, BsonMapper mapper, Logger log)
  {
    this._name = name ?? mapper.ResolveCollectionName(typeof (T));
    this._engine = engine;
    this._mapper = mapper;
    this._log = log;
    this._visitor = new QueryVisitor<T>(mapper);
    this._includes = new List<string>();
    if (typeof (T) != typeof (BsonDocument))
    {
      this._id = mapper.GetEntityMapper(typeof (T)).Id;
      if (this._id == null || !this._id.AutoId)
        return;
      this._autoId = this._id.DataType == typeof (ObjectId) ? BsonType.ObjectId : (this._id.DataType == typeof (Guid) ? BsonType.Guid : (this._id.DataType == typeof (DateTime) ? BsonType.DateTime : (this._id.DataType == typeof (int) ? BsonType.Int32 : (this._id.DataType == typeof (long) ? BsonType.Int64 : BsonType.Null))));
    }
    else
      this._autoId = BsonType.ObjectId;
  }

  public bool Update(T document)
  {
    if ((object) document == null)
      throw new ArgumentNullException(nameof (document));
    return this._engine.Value.Update(this._name, (IEnumerable<BsonDocument>) new BsonDocument[1]
    {
      this._mapper.ToDocument<T>(document)
    }) > 0;
  }

  public bool Update(BsonValue id, T document)
  {
    if ((object) document == null)
      throw new ArgumentNullException(nameof (document));
    if (id == (BsonValue) null || id.IsNull)
      throw new ArgumentNullException(nameof (id));
    BsonDocument document1 = this._mapper.ToDocument<T>(document);
    document1["_id"] = id;
    return this._engine.Value.Update(this._name, (IEnumerable<BsonDocument>) new BsonDocument[1]
    {
      document1
    }) > 0;
  }

  public int Update(IEnumerable<T> documents)
  {
    if (documents == null)
      throw new ArgumentNullException(nameof (documents));
    return this._engine.Value.Update(this._name, documents.Select<T, BsonDocument>((Func<T, BsonDocument>) (x => this._mapper.ToDocument<T>(x))));
  }

  public bool Upsert(T document)
  {
    if ((object) document == null)
      throw new ArgumentNullException(nameof (document));
    return this.Upsert((IEnumerable<T>) new T[1]{ document }) == 1;
  }

  public int Upsert(IEnumerable<T> documents)
  {
    if (documents == null)
      throw new ArgumentNullException(nameof (documents));
    return this._engine.Value.Upsert(this._name, this.GetBsonDocs(documents), this._autoId);
  }

  public bool Upsert(BsonValue id, T document)
  {
    if ((object) document == null)
      throw new ArgumentNullException(nameof (document));
    if (id == (BsonValue) null || id.IsNull)
      throw new ArgumentNullException(nameof (id));
    BsonDocument document1 = this._mapper.ToDocument<T>(document);
    document1["_id"] = id;
    return this._engine.Value.Upsert(this._name, document1);
  }
}
