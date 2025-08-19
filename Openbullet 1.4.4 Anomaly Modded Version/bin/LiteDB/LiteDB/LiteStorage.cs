// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteStorage
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB;

public class LiteStorage
{
  internal const string FILES = "_files";
  internal const string CHUNKS = "_chunks";
  private LiteEngine _engine;

  public LiteStorage(LiteEngine engine) => this._engine = engine;

  public LiteFileStream OpenWrite(string id, string filename, BsonDocument metadata = null)
  {
    LiteFileInfo liteFileInfo = this.FindById(id);
    if (liteFileInfo == null)
    {
      liteFileInfo = new LiteFileInfo(this._engine, id, filename ?? id);
      this._engine.Insert("_files", liteFileInfo.AsDocument);
    }
    if ((BsonValue) metadata != (BsonValue) null)
      liteFileInfo.Metadata = metadata;
    return liteFileInfo.OpenWrite();
  }

  public LiteFileInfo Upload(string id, string filename, Stream stream)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    LiteFileInfo liteFileInfo = this.FindById(id);
    if (liteFileInfo == null)
    {
      liteFileInfo = new LiteFileInfo(this._engine, id, filename ?? id);
      this._engine.Insert("_files", liteFileInfo.AsDocument);
    }
    using (LiteFileStream destination = liteFileInfo.OpenWrite())
      stream.CopyTo((Stream) destination);
    return liteFileInfo;
  }

  public LiteFileInfo Upload(string id, string filename)
  {
    if (filename.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (filename));
    using (FileStream fileStream = File.OpenRead(filename))
      return this.Upload(id, Path.GetFileName(filename), (Stream) fileStream);
  }

  public bool SetMetadata(string id, BsonDocument metadata)
  {
    LiteFileInfo byId = this.FindById(id);
    if (byId == null)
      return false;
    byId.Metadata = metadata ?? new BsonDocument();
    this._engine.Update("_files", byId.AsDocument);
    return true;
  }

  public LiteFileStream OpenRead(string id)
  {
    return (this.FindById(id) ?? throw LiteException.FileNotFound(id)).OpenRead();
  }

  public LiteFileInfo Download(string id, Stream stream)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    LiteFileInfo byId = this.FindById(id);
    if (byId == null)
      throw LiteException.FileNotFound(id);
    byId.CopyTo(stream);
    return byId;
  }

  public LiteFileInfo FindById(string id)
  {
    BsonDocument doc = !id.IsNullOrWhiteSpace() ? this._engine.Find("_files", Query.EQ("_id", (BsonValue) id)).FirstOrDefault<BsonDocument>() : throw new ArgumentNullException(nameof (id));
    return (BsonValue) doc == (BsonValue) null ? (LiteFileInfo) null : new LiteFileInfo(this._engine, doc);
  }

  public IEnumerable<LiteFileInfo> Find(string startsWith)
  {
    foreach (BsonDocument doc in this._engine.Find("_files", startsWith.IsNullOrWhiteSpace() ? Query.All() : Query.StartsWith("_id", startsWith)))
      yield return new LiteFileInfo(this._engine, doc);
  }

  public bool Exists(string id)
  {
    return !id.IsNullOrWhiteSpace() ? this._engine.Exists("_files", Query.EQ("_id", (BsonValue) id)) : throw new ArgumentNullException(nameof (id));
  }

  public IEnumerable<LiteFileInfo> FindAll() => this.Find((string) null);

  public bool Delete(string id)
  {
    bool flag = !id.IsNullOrWhiteSpace() ? this._engine.Delete("_files", (BsonValue) id) : throw new ArgumentNullException(nameof (id));
    if (!flag)
      return false;
    int num = 0;
    while (flag)
      flag = this._engine.Delete("_chunks", (BsonValue) LiteFileStream.GetChunckId(id, num++));
    return true;
  }
}
