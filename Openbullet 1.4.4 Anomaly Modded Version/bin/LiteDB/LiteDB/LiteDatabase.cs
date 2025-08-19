// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteDatabase
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB;

public class LiteDatabase : IDisposable
{
  private LazyLoad<LiteEngine> _engine;
  private BsonMapper _mapper = BsonMapper.Global;
  private Logger _log;
  private ConnectionString _connectionString;
  private LiteStorage _fs;

  public Logger Log => this._log;

  public BsonMapper Mapper => this._mapper;

  public LiteEngine Engine => this._engine.Value;

  public LiteDatabase(string connectionString, BsonMapper mapper = null, Logger log = null)
    : this(new ConnectionString(connectionString), mapper, log)
  {
  }

  public LiteDatabase(ConnectionString connectionString, BsonMapper mapper = null, Logger log = null)
  {
    this._connectionString = connectionString != null ? connectionString : throw new ArgumentNullException(nameof (connectionString));
    this._log = log ?? new Logger();
    this._log.Level = log != null ? log.Level : this._connectionString.Log;
    if (this._connectionString.Upgrade)
      LiteEngine.Upgrade(this._connectionString.Filename, this._connectionString.Password);
    this._mapper = mapper ?? BsonMapper.Global;
    FileOptions options = new FileOptions()
    {
      Flush = this._connectionString.Flush,
      InitialSize = this._connectionString.InitialSize,
      LimitSize = this._connectionString.LimitSize,
      Journal = this._connectionString.Journal,
      FileMode = this._connectionString.Mode
    };
    this._engine = new LazyLoad<LiteEngine>((Func<LiteEngine>) (() => new LiteEngine((IDiskService) new FileDiskService(this._connectionString.Filename, options), this._connectionString.Password, new TimeSpan?(this._connectionString.Timeout), this._connectionString.CacheSize, this._log, this._connectionString.UtcDate)));
  }

  public LiteDatabase(Stream stream, BsonMapper mapper = null, string password = null, bool disposeStream = false)
  {
    LiteDatabase liteDatabase = this;
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    this._mapper = mapper ?? BsonMapper.Global;
    this._log = new Logger();
    this._engine = new LazyLoad<LiteEngine>((Func<LiteEngine>) (() => new LiteEngine((IDiskService) new StreamDiskService(stream, disposeStream), password, log: liteDatabase._log)));
  }

  public LiteDatabase(
    IDiskService diskService,
    BsonMapper mapper = null,
    string password = null,
    TimeSpan? timeout = null,
    int cacheSize = 5000,
    Logger log = null)
  {
    LiteDatabase liteDatabase = this;
    if (diskService == null)
      throw new ArgumentNullException(nameof (diskService));
    this._mapper = mapper ?? BsonMapper.Global;
    this._log = log ?? new Logger();
    this._engine = new LazyLoad<LiteEngine>((Func<LiteEngine>) (() => new LiteEngine(diskService, password, timeout, cacheSize, liteDatabase._log)));
  }

  public LiteCollection<T> GetCollection<T>(string name)
  {
    return new LiteCollection<T>(name, this._engine, this._mapper, this._log);
  }

  public LiteCollection<T> GetCollection<T>() => this.GetCollection<T>((string) null);

  public LiteCollection<BsonDocument> GetCollection(string name)
  {
    if (name.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (name));
    return new LiteCollection<BsonDocument>(name, this._engine, this._mapper, this._log);
  }

  public LiteStorage FileStorage => this._fs ?? (this._fs = new LiteStorage(this._engine.Value));

  public IEnumerable<string> GetCollectionNames() => this._engine.Value.GetCollectionNames();

  public bool CollectionExists(string name)
  {
    if (name.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (name));
    return this._engine.Value.GetCollectionNames().Contains<string>(name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }

  public bool DropCollection(string name)
  {
    if (name.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (name));
    return this._engine.Value.DropCollection(name);
  }

  public bool RenameCollection(string oldName, string newName)
  {
    if (oldName.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (oldName));
    if (newName.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (newName));
    return this._engine.Value.RenameCollection(oldName, newName);
  }

  public long Shrink() => this.Shrink(this._connectionString?.Password);

  public long Shrink(string password)
  {
    if (this._connectionString == null)
      return this._engine.Value.Shrink(password);
    string tempFile = FileHelper.GetTempFile(this._connectionString.Filename);
    FileDiskService tempDisk = new FileDiskService(tempFile, false);
    long num = this._engine.Value.Shrink(password, (IDiskService) tempDisk);
    File.Delete(tempFile);
    return num;
  }

  public void Dispose()
  {
    if (!this._engine.IsValueCreated)
      return;
    this._engine.Value.Dispose();
  }
}
