// Decompiled with JetBrains decompiler
// Type: LiteDB.StreamDiskService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace LiteDB;

public class StreamDiskService : IDiskService, IDisposable
{
  private const int PAGE_TYPE_POSITION = 4;
  private Stream _stream;
  private Logger _log;
  private bool _disposeStream;

  public StreamDiskService(Stream stream, bool disposeStream = false)
  {
    this._stream = stream;
    this._disposeStream = disposeStream;
  }

  public void Initialize(Logger log, string password)
  {
    this._log = log;
    if (this._stream.Length != 0L)
      return;
    this._log.Write((byte) 128 /*0x80*/, "initialize new datafile");
    LiteEngine.CreateDatabase(this._stream, password);
  }

  public virtual void Dispose()
  {
    if (!this._disposeStream)
      return;
    this._stream.Dispose();
  }

  public virtual byte[] ReadPage(uint pageID)
  {
    byte[] buffer = new byte[4096 /*0x1000*/];
    long sizeOfPages = BasePage.GetSizeOfPages(pageID);
    if (this._stream.Position != sizeOfPages)
      this._stream.Seek(sizeOfPages, SeekOrigin.Begin);
    this._stream.Read(buffer, 0, 4096 /*0x1000*/);
    this._log.Write((byte) 128 /*0x80*/, "read page #{0:0000} :: {1}", (object) pageID, (object) (PageType) buffer[4]);
    return buffer;
  }

  public virtual void WritePage(uint pageID, byte[] buffer)
  {
    long sizeOfPages = BasePage.GetSizeOfPages(pageID);
    this._log.Write((byte) 128 /*0x80*/, "write page #{0:0000} :: {1}", (object) pageID, (object) (PageType) buffer[4]);
    if (this._stream.Position != sizeOfPages)
      this._stream.Seek(sizeOfPages, SeekOrigin.Begin);
    this._stream.Write(buffer, 0, 4096 /*0x1000*/);
  }

  public void SetLength(long fileSize) => this._stream.SetLength(fileSize);

  public long FileLength => this._stream.Length;

  public bool IsExclusive => true;

  public bool IsJournalEnabled => false;

  public void WriteJournal(ICollection<byte[]> pages, uint lastPageID)
  {
  }

  public IEnumerable<byte[]> ReadJournal(uint lastPageID)
  {
    yield break;
  }

  public void ClearJournal(uint lastPageID)
  {
  }

  public int Lock(LockState state, TimeSpan timeout) => 0;

  public void Unlock(LockState state, int position)
  {
  }

  public void Flush()
  {
  }
}
