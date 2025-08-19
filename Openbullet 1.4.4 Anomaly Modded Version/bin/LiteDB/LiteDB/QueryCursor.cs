// Decompiled with JetBrains decompiler
// Type: LiteDB.QueryCursor
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

internal class QueryCursor : IDisposable
{
  private int _position;
  private int _skip;
  private int _limit;
  private Query _query;
  private IEnumerator<IndexNode> _nodes;

  public List<BsonDocument> Documents { get; private set; }

  public bool HasMore { get; private set; }

  public QueryCursor(Query query, int skip, int limit)
  {
    this._query = query;
    this._skip = skip;
    this._limit = limit;
    this._position = skip;
    this._nodes = (IEnumerator<IndexNode>) null;
    this.HasMore = true;
    this.Documents = new List<BsonDocument>();
  }

  public void Initialize(IEnumerator<IndexNode> nodes) => this._nodes = nodes;

  public void ReQuery(IEnumerator<IndexNode> nodes)
  {
    this._nodes = nodes;
    this._skip = this._position;
  }

  public void Fetch(TransactionService trans, DataService data, BsonReader bsonReader)
  {
    this.Documents.Clear();
    while (!trans.CheckPoint())
    {
      this.HasMore = this._nodes.MoveNext();
      if (!this.HasMore)
        break;
      if (this._query.UseIndex && !this._query.UseFilter)
      {
        if (--this._skip < 0)
        {
          if (--this._limit <= -1)
          {
            this.HasMore = false;
            break;
          }
        }
        else
          continue;
      }
      IndexNode current = this._nodes.Current;
      byte[] bson = data.Read(current.DataBlock);
      BsonDocument asDocument = bsonReader.Deserialize(bson).AsDocument;
      if (this._query.UseFilter)
      {
        if (this._query.FilterDocument(asDocument) && --this._skip < 0)
        {
          if (--this._limit <= -1)
          {
            this.HasMore = false;
            break;
          }
        }
        else
          continue;
      }
      ++this._position;
      if (this._limit == 0)
        this.HasMore = false;
      this.Documents.Add(asDocument);
    }
  }

  public void Dispose()
  {
    if (this._nodes == null)
      return;
    this._nodes.Dispose();
  }
}
