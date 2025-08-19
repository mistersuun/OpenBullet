// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteEngine
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB.Shell;
using LiteDB_V6;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB;

public class LiteEngine : IDisposable
{
  private const int MAX_SORT_PAGES = 5000;
  private static List<ICommand> _commands = new List<ICommand>();
  private Logger _log;
  private LockService _locker;
  private IDiskService _disk;
  private CacheService _cache;
  private PageService _pager;
  private TransactionService _trans;
  private IndexService _indexer;
  private DataService _data;
  private CollectionService _collections;
  private AesEncryption _crypto;
  private int _cacheSize;
  private TimeSpan _timeout;
  private BsonReader _bsonReader;
  private BsonWriter _bsonWriter = new BsonWriter();

  public BsonValue Min(string collection, string field)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    using (this._locker.Read())
    {
      CollectionPage collectionPage = this.GetCollectionPage(collection, false);
      if (collectionPage == null)
        return BsonValue.MinValue;
      CollectionIndex index = collectionPage.GetIndex(field);
      if (index == null)
        return BsonValue.MinValue;
      IndexNode node = this._indexer.GetNode(this._indexer.GetNode(index.HeadNode).Next[0]);
      return node.IsHeadTail(index) ? BsonValue.MinValue : node.Key;
    }
  }

  public BsonValue Max(string collection, string field)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    using (this._locker.Read())
    {
      CollectionPage collectionPage = this.GetCollectionPage(collection, false);
      if (collectionPage == null)
        return BsonValue.MaxValue;
      CollectionIndex index = collectionPage.GetIndex(field);
      if (index == null)
        return BsonValue.MaxValue;
      IndexNode node = this._indexer.GetNode(this._indexer.GetNode(index.TailNode).Prev[0]);
      return node.IsHeadTail(index) ? BsonValue.MaxValue : node.Key;
    }
  }

  public long Count(string collection, Query query = null)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    using (this._locker.Read())
    {
      CollectionPage collectionPage = this.GetCollectionPage(collection, false);
      if (collectionPage == null)
        return 0;
      if (query == null)
        return collectionPage.DocumentCount;
      IEnumerable<IndexNode> source = query.Run(collectionPage, this._indexer);
      return query.UseFilter ? source.Select<IndexNode, BsonDocument>((Func<IndexNode, BsonDocument>) (x => this._bsonReader.Deserialize(this._data.Read(x.DataBlock)).AsDocument)).Where<BsonDocument>((Func<BsonDocument, bool>) (x => query.FilterDocument(x))).Distinct<BsonDocument>().LongCount<BsonDocument>() : source.Select<IndexNode, PageAddress>((Func<IndexNode, PageAddress>) (x => x.DataBlock)).Distinct<PageAddress>().LongCount<PageAddress>();
    }
  }

  public bool Exists(string collection, Query query)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    using (this._locker.Read())
    {
      CollectionPage collectionPage = this.GetCollectionPage(collection, false);
      if (collectionPage == null)
        return false;
      IEnumerable<IndexNode> source = query.Run(collectionPage, this._indexer);
      return query.UseFilter ? source.Select<IndexNode, BsonDocument>((Func<IndexNode, BsonDocument>) (x => this._bsonReader.Deserialize(this._data.Read(x.DataBlock)).AsDocument)).Where<BsonDocument>((Func<BsonDocument, bool>) (x => query.FilterDocument(x))).Any<BsonDocument>() : source.FirstOrDefault<IndexNode>() != null;
    }
  }

  public IEnumerable<string> GetCollectionNames()
  {
    using (this._locker.Read())
      return this._pager.GetPage<HeaderPage>(0U).CollectionPages.Keys.AsEnumerable<string>();
  }

  public bool DropCollection(string collection)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    return this.Transaction<bool>(collection, false, (Func<CollectionPage, bool>) (col =>
    {
      if (col == null)
        return false;
      this._log.Write((byte) 4, "drop collection {0}", (object) collection);
      this._collections.Drop(col);
      return true;
    }));
  }

  public bool RenameCollection(string collection, string newName)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (newName.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (newName));
    return this.Transaction<bool>(collection, false, (Func<CollectionPage, bool>) (col =>
    {
      if (col == null)
        return false;
      this._log.Write((byte) 4, "rename collection '{0}' -> '{1}'", (object) collection, (object) newName);
      col.CollectionName = !this.GetCollectionNames().Contains<string>(newName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? newName : throw LiteException.AlreadyExistsCollectionName(newName);
      this._pager.SetDirty((BasePage) col);
      HeaderPage page = this._pager.GetPage<HeaderPage>(0U);
      page.CollectionPages.Remove(collection);
      page.CollectionPages.Add(newName, col.PageID);
      this._pager.SetDirty((BasePage) page);
      return true;
    }));
  }

  public bool Delete(string collection, BsonValue id)
  {
    return this.Delete(collection, Query.EQ("_id", id)) == 1;
  }

  public int Delete(string collection, Query query)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    return this.Transaction<int>(collection, false, (Func<CollectionPage, int>) (col =>
    {
      if (col == null)
        return 0;
      this._log.Write((byte) 4, "delete documents in '{0}'", (object) collection);
      IEnumerable<IndexNode> indexNodes = query.Run(col, this._indexer);
      this._log.Write((byte) 16 /*0x10*/, "{0} :: {1}", (object) collection, (object) query);
      int num = 0;
      foreach (IndexNode node in indexNodes)
      {
        this._trans.CheckPoint();
        if (!query.UseFilter || query.FilterDocument(this._bsonReader.Deserialize(this._data.Read(node.DataBlock)).AsDocument))
        {
          this._log.Write((byte) 4, "delete document :: _id = {0}", node.Key.RawValue);
          foreach (IndexNode indexNode in this._indexer.GetNodeList(node, true).ToArray<IndexNode>())
            this._indexer.Delete(col.Indexes[(int) indexNode.Slot], indexNode.Position);
          this._data.Delete(col, node.DataBlock);
          ++num;
        }
      }
      return num;
    }));
  }

  public IEnumerable<BsonDocument> Find(string collection, Query query, int skip = 0, int limit = 2147483647 /*0x7FFFFFFF*/)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    this._log.Write((byte) 4, "query documents in '{0}' => {1}", (object) collection, (object) query);
    QueryCursor cursor = new QueryCursor(query, skip, limit);
    bool flag;
    try
    {
      using (this._locker.Read())
      {
        CollectionPage collectionPage = this.GetCollectionPage(collection, false);
        if (collectionPage == null)
        {
          flag = false;
          goto label_27;
        }
        cursor.Initialize(query.Run(collectionPage, this._indexer).GetEnumerator());
        this._log.Write((byte) 16 /*0x10*/, "{0} :: {1}", (object) collection, (object) query);
        cursor.Fetch(this._trans, this._data, this._bsonReader);
      }
      foreach (BsonDocument document in cursor.Documents)
        yield return document;
      while (cursor.HasMore)
      {
        using (LockControl lockControl = this._locker.Read())
        {
          if (lockControl.Changed)
          {
            CollectionPage collectionPage = this.GetCollectionPage(collection, false);
            if (collectionPage == null)
            {
              flag = false;
              goto label_27;
            }
            cursor.ReQuery(query.Run(collectionPage, this._indexer).GetEnumerator());
          }
          cursor.Fetch(this._trans, this._data, this._bsonReader);
        }
        foreach (BsonDocument document in cursor.Documents)
          yield return document;
      }
      goto label_29;
label_27:
      goto label_30;
    }
    finally
    {
      cursor?.Dispose();
    }
label_29:
    cursor = (QueryCursor) null;
    yield break;
label_30:
    return flag;
  }

  public BsonDocument FindOne(string collection, Query query)
  {
    return this.Find(collection, query).FirstOrDefault<BsonDocument>();
  }

  public BsonDocument FindById(string collection, BsonValue id)
  {
    return !(id == (BsonValue) null) && !id.IsNull ? this.Find(collection, Query.EQ("_id", id)).FirstOrDefault<BsonDocument>() : throw new ArgumentNullException(nameof (id));
  }

  public IEnumerable<BsonDocument> FindAll(string collection) => this.Find(collection, Query.All());

  public IEnumerable<BsonDocument> Find(
    string collection,
    Query query,
    string[] includes,
    int skip = 0,
    int limit = 2147483647 /*0x7FFFFFFF*/)
  {
    if (includes == null)
      throw new ArgumentNullException(nameof (includes));
    foreach (BsonDocument doc1 in this.Find(collection, query, skip, limit))
    {
      foreach (string include in includes)
      {
        foreach (BsonDocument doc2 in new BsonExpression(include.StartsWith("$") ? include : "$." + include).Execute(doc1, false).Where<BsonValue>((Func<BsonValue, bool>) (x => x.IsDocument)).Select<BsonValue, BsonDocument>((Func<BsonValue, BsonDocument>) (x => x.AsDocument)).ToList<BsonDocument>())
        {
          BsonValue id = doc2["$id"];
          BsonValue collection1 = doc2["$ref"];
          if (!id.IsNull && collection1.IsString)
          {
            BsonDocument byId = this.FindById((string) collection1, id);
            if ((BsonValue) byId != (BsonValue) null)
            {
              doc2.Remove("$id");
              doc2.Remove("$ref");
              byId.CopyTo(doc2);
            }
            else
              doc2.Destroy();
          }
        }
      }
      yield return doc1;
    }
  }

  public List<BsonDocument> FindSort(
    string collection,
    Query query,
    string orderBy,
    int order = 1,
    int skip = 0,
    int limit = 2147483647 /*0x7FFFFFFF*/)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (query == null)
      throw new ArgumentNullException(nameof (query));
    this._log.Write((byte) 4, "query-sort documents in '{0}' => {1}", (object) collection, (object) query);
    BsonExpression bsonExpression = new BsonExpression(orderBy);
    using (this._locker.Read())
    {
      BsonValue other = order == 1 ? BsonValue.MaxValue : BsonValue.MinValue;
      int num1 = limit == int.MaxValue ? int.MaxValue : skip + limit;
      int num2 = 0;
      using (LiteEngine liteEngine = new LiteEngine((IDiskService) new TempDiskService()))
      {
        CollectionPage collectionPage = this.GetCollectionPage(collection, false);
        if (collectionPage == null)
          return new List<BsonDocument>();
        CollectionPage col = liteEngine._collections.Add("tmp");
        CollectionIndex index = liteEngine._indexer.CreateIndex(col);
        IndexNode node1 = liteEngine._indexer.GetNode(index.HeadNode);
        IndexNode node2 = liteEngine._indexer.GetNode(index.TailNode);
        foreach (IndexNode indexNode1 in query.Run(collectionPage, this._indexer))
        {
          BsonDocument asDocument = this._bsonReader.Deserialize(this._data.Read(indexNode1.DataBlock)).AsDocument;
          if (!query.UseFilter || query.FilterDocument(asDocument))
          {
            BsonValue key = bsonExpression.Execute(asDocument).First<BsonValue>();
            int num3 = key.CompareTo(other);
            if (order == 1 && num3 < 1 || order == -1 && num3 > -1)
            {
              IndexNode indexNode2 = liteEngine._indexer.AddNode(index, key, (IndexNode) null);
              indexNode2.DataBlock = indexNode1.DataBlock;
              indexNode2.CacheDocument = asDocument;
              ++num2;
              if (num2 > num1)
              {
                PageAddress nodeAddress = order == 1 ? node2.Prev[0] : node1.Next[0];
                liteEngine._indexer.Delete(index, nodeAddress);
                PageAddress address = order == 1 ? node2.Prev[0] : node1.Next[0];
                other = liteEngine._indexer.GetNode(address).Key;
                --num2;
              }
              if (liteEngine._cache.DirtyUsed > 5000)
              {
                liteEngine._trans.PersistDirtyPages();
                liteEngine._trans.CheckPoint();
              }
            }
          }
        }
        List<BsonDocument> sort = new List<BsonDocument>();
        foreach (IndexNode indexNode in skip < limit ? liteEngine._indexer.FindAll(index, order).Skip<IndexNode>(skip).Take<IndexNode>(limit) : liteEngine._indexer.FindAll(index, -order).Take<IndexNode>(limit).Reverse<IndexNode>())
        {
          BsonDocument bsonDocument = indexNode.CacheDocument;
          if ((BsonValue) bsonDocument == (BsonValue) null)
            bsonDocument = this._bsonReader.Deserialize(this._data.Read(indexNode.DataBlock)).AsDocument;
          sort.Add(bsonDocument);
        }
        return sort;
      }
    }
  }

  public bool EnsureIndex(string collection, string field, bool unique = false)
  {
    return this.EnsureIndex(collection, field, (string) null, unique);
  }

  public bool EnsureIndex(string collection, string field, string expression, bool unique = false)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (!CollectionIndex.IndexPattern.IsMatch(field))
      throw new ArgumentException("Invalid field format pattern: " + CollectionIndex.IndexPattern.ToString(), nameof (field));
    if (field == "_id")
      return false;
    if (expression != null && expression.Length > 200)
      throw new ArgumentException("expression is limited in 200 characters", nameof (expression));
    return this.Transaction<bool>(collection, true, (Func<CollectionPage, bool>) (col =>
    {
      if (col.GetIndex(field) != null)
        return false;
      CollectionIndex index = this._indexer.CreateIndex(col);
      index.Field = field;
      index.Expression = expression ?? "$." + field;
      index.Unique = unique;
      this._log.Write((byte) 4, "create index on '{0}' :: {1} unique: {2}", (object) collection, (object) index.Expression, (object) unique);
      foreach (IndexNode last in new QueryAll("_id", 1).Run(col, this._indexer))
      {
        BsonDocument asDocument = this._bsonReader.Deserialize(this._data.Read(last.DataBlock)).AsDocument;
        foreach (BsonValue key in new BsonExpression(index.Expression).Execute(asDocument))
          this._indexer.AddNode(index, key, last).DataBlock = last.DataBlock;
        this._trans.CheckPoint();
      }
      return true;
    }));
  }

  public bool DropIndex(string collection, string field)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (field.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (field));
    if (field == "_id")
      throw LiteException.IndexDropId();
    return this.Transaction<bool>(collection, false, (Func<CollectionPage, bool>) (col =>
    {
      if (col == null)
        return false;
      CollectionIndex index = col.GetIndex(field);
      if (index == null)
        return false;
      this._log.Write((byte) 4, "drop index on '{0}' :: '{1}'", (object) collection, (object) field);
      this._indexer.DropIndex(index);
      index.Clear();
      this._pager.SetDirty((BasePage) col);
      return true;
    }));
  }

  public IEnumerable<IndexInfo> GetIndexes(string collection)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    using (this._locker.Read())
    {
      CollectionPage collectionPage = this.GetCollectionPage(collection, false);
      if (collectionPage == null)
        yield break;
      foreach (CollectionIndex index in collectionPage.GetIndexes(true))
        yield return new IndexInfo(index);
    }
  }

  public BsonDocument Info()
  {
    using (this._locker.Read())
    {
      HeaderPage page = this._pager.GetPage<HeaderPage>(0U);
      BsonArray bsonArray = new BsonArray();
      foreach (string key in page.CollectionPages.Keys)
      {
        CollectionPage collectionPage = this.GetCollectionPage(key, false);
        BsonDocument bsonDocument = new BsonDocument()
        {
          {
            "name",
            (BsonValue) collectionPage.CollectionName
          },
          {
            "pageID",
            (BsonValue) (double) collectionPage.PageID
          },
          {
            "count",
            (BsonValue) collectionPage.DocumentCount
          },
          {
            "sequence",
            (BsonValue) collectionPage.Sequence
          },
          {
            "indexes",
            (BsonValue) new BsonArray(((IEnumerable<CollectionIndex>) collectionPage.Indexes).Where<CollectionIndex>((Func<CollectionIndex, bool>) (x => !x.IsEmpty)).Select<CollectionIndex, BsonDocument>((Func<CollectionIndex, BsonDocument>) (i => new BsonDocument()
            {
              {
                "slot",
                (BsonValue) i.Slot
              },
              {
                "field",
                (BsonValue) i.Field
              },
              {
                "expression",
                (BsonValue) i.Expression
              },
              {
                "unique",
                (BsonValue) i.Unique
              }
            })))
          }
        };
        bsonArray.Add((BsonValue) bsonDocument);
      }
      return new BsonDocument()
      {
        {
          "userVersion",
          (BsonValue) (int) page.UserVersion
        },
        {
          "encrypted",
          (BsonValue) ((IEnumerable<byte>) page.Password).Any<byte>((Func<byte, bool>) (x => x > (byte) 0))
        },
        {
          "changeID",
          (BsonValue) (int) page.ChangeID
        },
        {
          "lastPageID",
          (BsonValue) (int) page.LastPageID
        },
        {
          "fileSize",
          (BsonValue) BasePage.GetSizeOfPages(page.LastPageID + 1U)
        },
        {
          "collections",
          (BsonValue) bsonArray
        }
      };
    }
  }

  public BsonValue Insert(string collection, BsonDocument doc, BsonType autoId = BsonType.ObjectId)
  {
    if ((BsonValue) doc == (BsonValue) null)
      throw new ArgumentNullException(nameof (doc));
    this.Insert(collection, (IEnumerable<BsonDocument>) new BsonDocument[1]
    {
      doc
    }, autoId);
    return doc["_id"];
  }

  public int Insert(string collection, IEnumerable<BsonDocument> docs, BsonType autoId = BsonType.ObjectId)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (docs == null)
      throw new ArgumentNullException(nameof (docs));
    return this.Transaction<int>(collection, true, (Func<CollectionPage, int>) (col =>
    {
      int num = 0;
      foreach (BsonDocument doc in docs)
      {
        this.InsertDocument(col, doc, autoId);
        this._trans.CheckPoint();
        ++num;
      }
      return num;
    }));
  }

  public int InsertBulk(
    string collection,
    IEnumerable<BsonDocument> docs,
    int batchSize = 5000,
    BsonType autoId = BsonType.ObjectId)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (docs == null)
      throw new ArgumentNullException(nameof (docs));
    if (batchSize < 100 || batchSize > 100000)
      throw new ArgumentException("batchSize must be a value between 100 and 100000");
    int num = 0;
    foreach (IEnumerable<BsonDocument> docs1 in docs.Batch<BsonDocument>(batchSize))
      num += this.Insert(collection, docs1, autoId);
    return num;
  }

  private void InsertDocument(CollectionPage col, BsonDocument doc, BsonType autoId)
  {
    if (col.Sequence == 0L && col.DocumentCount > 0L)
    {
      BsonValue bsonValue = this.Max(col.CollectionName, "_id");
      col.Sequence = bsonValue.IsInt32 || bsonValue.IsInt64 || bsonValue.IsDouble || bsonValue.IsDecimal ? Convert.ToInt64(bsonValue.RawValue) : Convert.ToInt64(col.DocumentCount);
    }
    ++col.Sequence;
    this._pager.SetDirty((BasePage) col);
    BsonValue key1;
    if (!doc.RawValue.TryGetValue("_id", out key1))
    {
      BsonDocument bsonDocument = doc;
      BsonValue bsonValue;
      switch (autoId)
      {
        case BsonType.Int32:
          bsonValue = new BsonValue((int) col.Sequence);
          break;
        case BsonType.Int64:
          bsonValue = new BsonValue(col.Sequence);
          break;
        case BsonType.ObjectId:
          bsonValue = new BsonValue(ObjectId.NewObjectId());
          break;
        case BsonType.Guid:
          bsonValue = new BsonValue(Guid.NewGuid());
          break;
        case BsonType.DateTime:
          bsonValue = new BsonValue(DateTime.Now);
          break;
        default:
          bsonValue = BsonValue.Null;
          break;
      }
      key1 = bsonValue;
      bsonDocument["_id"] = bsonValue;
    }
    else if (autoId == BsonType.Int32 || autoId == BsonType.Int64)
    {
      long asInt64 = key1.AsInt64;
      col.Sequence = asInt64 >= col.Sequence ? asInt64 : col.Sequence - 1L;
    }
    if (key1.IsNull || key1.IsMinValue || key1.IsMaxValue)
      throw LiteException.InvalidDataType("_id", key1);
    this._log.Write((byte) 4, "insert document on '{0}' :: _id = {1}", (object) col.CollectionName, key1.RawValue);
    byte[] data = this._bsonWriter.Serialize(doc);
    DataBlock dataBlock = this._data.Insert(col, data);
    IndexNode last = this._indexer.AddNode(col.PK, key1, (IndexNode) null);
    last.DataBlock = dataBlock.Position;
    foreach (CollectionIndex index in col.GetIndexes(false))
    {
      foreach (BsonValue key2 in new BsonExpression(index.Expression).Execute(doc))
        this._indexer.AddNode(index, key2, last).DataBlock = dataBlock.Position;
    }
  }

  public IList<BsonValue> Run(string command)
  {
    if (LiteEngine._commands.Count == 0)
      LiteEngine.RegisterCommands();
    StringScanner s = new StringScanner(command);
    foreach (ICommand command1 in LiteEngine._commands)
    {
      if (command1.IsCommand(s))
        return (IList<BsonValue>) command1.Execute(s, this).ToList<BsonValue>();
    }
    throw LiteException.InvalidCommand(command);
  }

  private static void RegisterCommands()
  {
    lock (LiteEngine._commands)
    {
      Type type = typeof (ICommand);
      IEnumerable<Type> types = ((IEnumerable<Type>) typeof (LiteEngine).GetTypeInfo().Assembly.GetTypes()).Where<Type>((Func<Type, bool>) (p => type.IsAssignableFrom(p) && p.GetTypeInfo().IsClass));
      LiteEngine._commands.Clear();
      foreach (Type type1 in types)
        LiteEngine._commands.Add(Activator.CreateInstance(type1) as ICommand);
    }
  }

  public long Shrink(string password = null, IDiskService tempDisk = null)
  {
    long fileLength = this._disk.FileLength;
    using (IDiskService disk = tempDisk ?? (IDiskService) new StreamDiskService((Stream) new MemoryStream()))
    {
      using (this._locker.Write())
      {
        using (LiteEngine engine = new LiteEngine(disk, password))
        {
          foreach (string collectionName in this.GetCollectionNames())
          {
            foreach (IndexInfo indexInfo in this.GetIndexes(collectionName).Where<IndexInfo>((Func<IndexInfo, bool>) (x => x.Field != "_id")))
              engine.EnsureIndex(collectionName, indexInfo.Field, indexInfo.Unique);
            IEnumerable<BsonDocument> docs = this.Find(collectionName, Query.All());
            engine.InsertBulk(collectionName, docs);
            long seq = this._collections.Get(collectionName).Sequence;
            engine.Transaction<bool>(collectionName, true, (Func<CollectionPage, bool>) (col =>
            {
              col.Sequence = seq;
              engine._pager.SetDirty((BasePage) col);
              return true;
            }));
          }
          engine.UserVersion = this.UserVersion;
          this._disk.SetLength(disk.FileLength);
          HeaderPage headerPage = BasePage.ReadPage(disk.ReadPage(0U)) as HeaderPage;
          for (uint pageID = 0; pageID <= headerPage.LastPageID; ++pageID)
          {
            if (pageID != 1U)
            {
              byte[] buffer = disk.ReadPage(pageID);
              this._disk.WritePage(pageID, buffer);
            }
          }
          if (this._crypto != null)
            this._crypto.Dispose();
          this._crypto = password == null ? (AesEncryption) null : new AesEncryption(password, headerPage.Salt);
          this.InitializeServices();
          return fileLength - disk.FileLength;
        }
      }
    }
  }

  public bool Update(string collection, BsonDocument doc)
  {
    if ((BsonValue) doc == (BsonValue) null)
      throw new ArgumentNullException(nameof (doc));
    return this.Update(collection, (IEnumerable<BsonDocument>) new BsonDocument[1]
    {
      doc
    }) == 1;
  }

  public int Update(string collection, IEnumerable<BsonDocument> docs)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (docs == null)
      throw new ArgumentNullException(nameof (docs));
    return this.Transaction<int>(collection, false, (Func<CollectionPage, int>) (col =>
    {
      if (col == null)
        return 0;
      int num = 0;
      foreach (BsonDocument doc in docs)
      {
        if (this.UpdateDocument(col, doc))
        {
          this._trans.CheckPoint();
          ++num;
        }
      }
      return num;
    }));
  }

  private bool UpdateDocument(CollectionPage col, BsonDocument doc)
  {
    BsonValue bsonValue = doc["_id"];
    if (bsonValue.IsNull || bsonValue.IsMinValue || bsonValue.IsMaxValue)
      throw LiteException.InvalidDataType("_id", bsonValue);
    this._log.Write((byte) 4, "update document on '{0}' :: _id = {1}", (object) col.CollectionName, bsonValue.RawValue);
    IndexNode indexNode1 = this._indexer.Find(col.PK, bsonValue, false, 1);
    if (indexNode1 == null)
      return false;
    byte[] data = this._bsonWriter.Serialize(doc);
    DataBlock dataBlock = this._data.Update(col, indexNode1.DataBlock, data);
    IndexNode[] allNodes = this._indexer.GetNodeList(indexNode1, false).ToArray<IndexNode>();
    foreach (CollectionIndex index1 in col.GetIndexes(false))
    {
      CollectionIndex index = index1;
      BsonValue[] keys = new BsonExpression(index.Expression).Execute(doc).ToArray<BsonValue>();
      IndexNode[] array1 = ((IEnumerable<IndexNode>) allNodes).Where<IndexNode>((Func<IndexNode, bool>) (x => (int) x.Slot == index.Slot && !((IEnumerable<BsonValue>) keys).Any<BsonValue>((Func<BsonValue, bool>) (k => k == x.Key)))).ToArray<IndexNode>();
      BsonValue[] array2 = ((IEnumerable<BsonValue>) keys).Where<BsonValue>((Func<BsonValue, bool>) (x => !((IEnumerable<IndexNode>) allNodes).Any<IndexNode>((Func<IndexNode, bool>) (k => (int) k.Slot == index.Slot && k.Key == x)))).ToArray<BsonValue>();
      foreach (IndexNode indexNode2 in array1)
        this._indexer.Delete(index, indexNode2.Position);
      foreach (BsonValue key in array2)
        this._indexer.AddNode(index, key, indexNode1).DataBlock = dataBlock.Position;
    }
    return true;
  }

  public bool Upsert(string collection, BsonDocument doc, BsonType autoId = BsonType.ObjectId)
  {
    if ((BsonValue) doc == (BsonValue) null)
      throw new ArgumentNullException(nameof (doc));
    return this.Upsert(collection, (IEnumerable<BsonDocument>) new BsonDocument[1]
    {
      doc
    }, autoId) == 1;
  }

  public int Upsert(string collection, IEnumerable<BsonDocument> docs, BsonType autoId = BsonType.ObjectId)
  {
    if (collection.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (collection));
    if (docs == null)
      throw new ArgumentNullException(nameof (docs));
    return this.Transaction<int>(collection, true, (Func<CollectionPage, int>) (col =>
    {
      int num = 0;
      foreach (BsonDocument doc in docs)
      {
        if (doc["_id"] == BsonValue.Null || !this.UpdateDocument(col, doc))
        {
          this.InsertDocument(col, doc, autoId);
          ++num;
        }
        this._trans.CheckPoint();
      }
      return num;
    }));
  }

  public ushort UserVersion
  {
    get
    {
      using (this._locker.Read())
        return this._pager.GetPage<HeaderPage>(0U).UserVersion;
    }
    set
    {
      this.Transaction<bool>((string) null, false, (Func<CollectionPage, bool>) (col =>
      {
        HeaderPage page = this._pager.GetPage<HeaderPage>(0U);
        page.UserVersion = value;
        this._pager.SetDirty((BasePage) page);
        return true;
      }));
    }
  }

  public Logger Log => this._log;

  public int CacheSize => this._cacheSize;

  public int CacheUsed => this._cache.CleanUsed;

  public TimeSpan Timeout => this._timeout;

  public LockService Locker => this._locker;

  public LiteEngine(string filename, bool journal = true)
    : this((IDiskService) new FileDiskService(filename, journal))
  {
  }

  public LiteEngine(string filename, string password, bool journal = true)
    : this((IDiskService) new FileDiskService(filename, new FileOptions()
    {
      Journal = journal
    }), password)
  {
  }

  public LiteEngine(Stream stream, string password = null)
    : this((IDiskService) new StreamDiskService(stream), password)
  {
  }

  public LiteEngine(
    IDiskService disk,
    string password = null,
    TimeSpan? timeout = null,
    int cacheSize = 5000,
    Logger log = null,
    bool utcDate = false)
  {
    if (disk == null)
      throw new ArgumentNullException(nameof (disk));
    this._timeout = timeout ?? TimeSpan.FromMinutes(1.0);
    this._cacheSize = cacheSize;
    this._disk = disk;
    this._log = log ?? new Logger();
    this._bsonReader = new BsonReader(utcDate);
    try
    {
      this._disk.Initialize(this._log, password);
      int position = this._disk.Lock(LockState.Read, this._timeout);
      byte[] buffer = this._disk.ReadPage(0U);
      this._disk.Unlock(LockState.Read, position);
      HeaderPage headerPage = BasePage.ReadPage(buffer) as HeaderPage;
      if ((password == null ? new byte[20] : AesEncryption.HashSHA1(password)).BinaryCompareTo(headerPage.Password) != 0)
        throw LiteException.DatabaseWrongPassword();
      if (password != null)
        this._crypto = new AesEncryption(password, headerPage.Salt);
      this.InitializeServices();
      if (!headerPage.Recovery)
        return;
      this._trans.Recovery();
    }
    catch (Exception ex)
    {
      this.Dispose();
      throw;
    }
  }

  private void InitializeServices()
  {
    this._cache = new CacheService(this._disk, this._log);
    this._locker = new LockService(this._disk, this._cache, this._timeout, this._log);
    this._pager = new PageService(this._disk, this._crypto, this._cache, this._log);
    this._indexer = new IndexService(this._pager, this._log);
    this._data = new DataService(this._pager, this._log);
    this._trans = new TransactionService(this._disk, this._crypto, this._pager, this._locker, this._cache, this._cacheSize, this._log);
    this._collections = new CollectionService(this._pager, this._indexer, this._data, this._trans, this._log);
  }

  private CollectionPage GetCollectionPage(string name, bool addIfNotExits)
  {
    if (name == null)
      return (CollectionPage) null;
    CollectionPage collectionPage = this._collections.Get(name);
    if (collectionPage == null & addIfNotExits)
    {
      this._log.Write((byte) 4, "create new collection '{0}'", (object) name);
      collectionPage = this._collections.Add(name);
    }
    return collectionPage;
  }

  private T Transaction<T>(string collection, bool addIfNotExists, Func<CollectionPage, T> action)
  {
    using (this._locker.Write())
    {
      try
      {
        CollectionPage collectionPage = this.GetCollectionPage(collection, addIfNotExists);
        T obj = action(collectionPage);
        this._trans.PersistDirtyPages();
        return obj;
      }
      catch (Exception ex)
      {
        this._log.Write((byte) 1, ex.Message);
        this._cache.DiscardDirtyPages();
        throw;
      }
    }
  }

  public void Dispose()
  {
    this._disk.Dispose();
    if (this._crypto == null)
      return;
    this._crypto.Dispose();
  }

  public static void CreateDatabase(Stream stream, string password = null, long initialSize = 0)
  {
    long num = initialSize == 0L ? 0L : (initialSize - 8192L /*0x2000*/) / 4096L /*0x1000*/;
    if (num < 0L)
      num = 0L;
    HeaderPage headerPage = new HeaderPage()
    {
      LastPageID = initialSize == 0L ? 1U : (uint) num + 1U,
      FreeEmptyPageID = initialSize == 0L ? uint.MaxValue : 2U
    };
    if (password != null)
    {
      headerPage.Password = AesEncryption.HashSHA1(password);
      headerPage.Salt = AesEncryption.Salt();
    }
    stream.Seek(0L, SeekOrigin.Begin);
    byte[] buffer = headerPage.WritePage();
    stream.Write(buffer, 0, 4096 /*0x1000*/);
    stream.Write(new byte[4096 /*0x1000*/], 0, 4096 /*0x1000*/);
    AesEncryption aesEncryption = password != null ? new AesEncryption(password, headerPage.Salt) : (AesEncryption) null;
    if (num > 0L)
    {
      stream.SetLength(initialSize);
      uint pageID = 1;
      while ((long) ++pageID < num + 2L)
      {
        EmptyPage emptyPage = new EmptyPage(pageID);
        emptyPage.PrevPageID = pageID == 2U ? 0U : pageID - 1U;
        emptyPage.NextPageID = (long) pageID == num + 1L ? uint.MaxValue : pageID + 1U;
        byte[] numArray = emptyPage.WritePage();
        if (password != null)
          numArray = aesEncryption.Encrypt(numArray);
        stream.Write(numArray, 0, 4096 /*0x1000*/);
      }
    }
    aesEncryption?.Dispose();
  }

  public static bool Upgrade(string filename, string password = null, bool backup = true, int batchSize = 5000)
  {
    if (!File.Exists(filename))
      return false;
    string tempFile = FileHelper.GetTempFile(filename);
    using (FileStream fileStream = new FileStream(filename, System.IO.FileMode.Open, FileAccess.Read))
    {
      using (IDbReader dbReader = (IDbReader) new DbReader())
      {
        if (!dbReader.Initialize((Stream) fileStream, password))
          return false;
        using (LiteEngine liteEngine = new LiteEngine(tempFile, false))
        {
          foreach (string collection in dbReader.GetCollections())
          {
            foreach (string uniqueIndex in dbReader.GetUniqueIndexes(collection))
              liteEngine.EnsureIndex(collection, uniqueIndex, true);
            IEnumerable<BsonDocument> documents = dbReader.GetDocuments(collection);
            liteEngine.InsertBulk(collection, documents, batchSize);
          }
        }
      }
    }
    if (backup)
      File.Move(filename, FileHelper.GetTempFile(filename, "-bkp"));
    else
      File.Delete(filename);
    File.Move(tempFile, filename);
    return true;
  }
}
