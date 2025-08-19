// Decompiled with JetBrains decompiler
// Type: LiteDB.LiteFileStream
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;
using System.Linq;

#nullable disable
namespace LiteDB;

public class LiteFileStream : Stream
{
  public const int MAX_CHUNK_SIZE = 261120;
  private LiteEngine _engine;
  private LiteFileInfo _file;
  private FileAccess _mode;
  private long _streamPosition;
  private int _currentChunkIndex;
  private byte[] _currentChunkData;
  private int _positionInChunk;
  private MemoryStream _buffer;
  private bool _disposed;

  internal LiteFileStream(LiteEngine engine, LiteFileInfo file, FileAccess mode)
  {
    this._engine = engine;
    this._file = file;
    this._mode = mode;
    switch (mode)
    {
      case FileAccess.Read:
        this._currentChunkData = this.GetChunkData(this._currentChunkIndex);
        break;
      case FileAccess.Write:
        this._buffer = new MemoryStream(261120);
        if (file.Length > 0L)
        {
          int num = 0;
          bool flag = true;
          while (flag)
            flag = this._engine.Delete("_chunks", (BsonValue) LiteFileStream.GetChunckId(this._file.Id, num++));
        }
        file.Length = 0L;
        file.Chunks = 0;
        break;
    }
  }

  public LiteFileInfo FileInfo => this._file;

  public override long Length => this._file.Length;

  public override bool CanRead => this._mode == FileAccess.Read;

  public override bool CanWrite => this._mode == FileAccess.Write;

  public override bool CanSeek => false;

  public override long Position
  {
    get => this._streamPosition;
    set => throw new NotSupportedException();
  }

  internal static string GetChunckId(string id, int index) => $"{id}\\{index:00000}";

  ~LiteFileStream() => this.Dispose(false);

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (this._disposed)
      return;
    if (this.CanWrite)
      this.Flush();
    this._disposed = true;
  }

  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

  public override void SetLength(long value) => throw new NotSupportedException();

  public override int Read(byte[] buffer, int offset, int count)
  {
    if (this._mode != FileAccess.Read)
      throw new NotSupportedException();
    int val1 = count;
    while (this._currentChunkData != null && val1 > 0)
    {
      int count1 = Math.Min(val1, this._currentChunkData.Length - this._positionInChunk);
      Buffer.BlockCopy((Array) this._currentChunkData, this._positionInChunk, (Array) buffer, offset, count1);
      this._positionInChunk += count1;
      val1 -= count1;
      offset += count1;
      this._streamPosition += (long) count1;
      if (this._positionInChunk >= this._currentChunkData.Length)
      {
        this._positionInChunk = 0;
        this._currentChunkData = this.GetChunkData(++this._currentChunkIndex);
      }
    }
    return count - val1;
  }

  private byte[] GetChunkData(int index)
  {
    BsonDocument bsonDocument = this._engine.Find("_chunks", Query.EQ("_id", (BsonValue) LiteFileStream.GetChunckId(this._file.Id, index))).FirstOrDefault<BsonDocument>();
    return !((BsonValue) bsonDocument == (BsonValue) null) ? bsonDocument["data"].AsBinary : (byte[]) null;
  }

  public override void Flush()
  {
    this.WriteChunks();
    this._file.UploadDate = DateTime.Now;
    this._file.Length = this._streamPosition;
    this._engine.Update("_files", this._file.AsDocument);
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
    this._streamPosition += (long) count;
    this._buffer.Write(buffer, offset, count);
    if (this._buffer.Length < 261120L)
      return;
    this.WriteChunks();
  }

  private void WriteChunks()
  {
    byte[] numArray = new byte[261120];
    this._buffer.Seek(0L, SeekOrigin.Begin);
    int count;
    while ((count = this._buffer.Read(numArray, 0, 261120)) > 0)
    {
      BsonDocument doc = new BsonDocument()
      {
        {
          "_id",
          (BsonValue) LiteFileStream.GetChunckId(this._file.Id, this._file.Chunks++)
        }
      };
      if (count != 261120)
      {
        byte[] dst = new byte[count];
        Buffer.BlockCopy((Array) numArray, 0, (Array) dst, 0, count);
        doc["data"] = (BsonValue) dst;
      }
      else
        doc["data"] = (BsonValue) numArray;
      this._engine.Insert("_chunks", doc);
    }
    this._buffer = new MemoryStream();
  }
}
