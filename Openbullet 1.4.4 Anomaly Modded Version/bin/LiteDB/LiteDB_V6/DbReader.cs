// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.DbReader
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB_V6;

internal class DbReader : IDbReader, IDisposable
{
  private PageService _pager;
  private CollectionService _collections;
  private FileDiskService _disk;
  private IndexService _indexer;
  private DataService _data;

  public bool Initialize(Stream stream, string password)
  {
    if (stream.ReadByte(52L) != (byte) 6)
      return false;
    this._disk = new FileDiskService(stream, password);
    this._pager = new PageService(this._disk);
    this._indexer = new IndexService(this._pager);
    this._data = new DataService(this._pager);
    this._collections = new CollectionService(this._pager);
    return true;
  }

  public IEnumerable<string> GetCollections()
  {
    return (IEnumerable<string>) this._pager.GetPage<HeaderPage>(0U).CollectionPages.Keys;
  }

  public IEnumerable<string> GetUniqueIndexes(string collection)
  {
    foreach (CollectionIndex collectionIndex in ((IEnumerable<CollectionIndex>) this._collections.Get(collection).Indexes).Where<CollectionIndex>((Func<CollectionIndex, bool>) (x => x.Field != "_id" && x.Unique)))
      yield return collectionIndex.Field;
  }

  public IEnumerable<BsonDocument> GetDocuments(string collection)
  {
    foreach (IndexNode indexNode in this._indexer.FindAll(this._collections.Get(collection).PK))
      yield return new BsonReader(false).Deserialize(this._data.Read(indexNode.DataBlock));
  }

  public void Dispose()
  {
    if (this._disk == null)
      return;
    this._disk.Dispose();
  }
}
