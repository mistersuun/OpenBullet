// Decompiled with JetBrains decompiler
// Type: LiteDB.FileDiskService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace LiteDB;

public class FileDiskService : IDiskService, IDisposable
{
  private const int PAGE_TYPE_POSITION = 4;
  internal const int LOCK_INITIAL_POSITION = 4096 /*0x1000*/;
  internal const int LOCK_READ_LENGTH = 1;
  internal const int LOCK_WRITE_LENGTH = 3000;
  private FileStream _stream;
  private string _filename;
  private Logger _log;
  private FileOptions _options;
  private Random _lockReadRand = new Random();

  public FileDiskService(string filename, bool journal = true)
    : this(filename, new FileOptions() { Journal = journal })
  {
  }

  public FileDiskService(string filename, FileOptions options)
  {
    if (filename.IsNullOrWhiteSpace())
      throw new ArgumentNullException(nameof (filename));
    if (options.InitialSize > options.LimitSize)
      throw new ArgumentException("limit size less than initial size");
    this._filename = filename;
    this._options = options;
  }

  public void Initialize(Logger log, string password)
  {
    this._log = log;
    if (this._options.FileMode == FileMode.ReadOnly)
      this._options.Journal = false;
    this._log.Write((byte) 128 /*0x80*/, "open datafile '{0}'", (object) Path.GetFileName(this._filename));
    this._stream = this.CreateFileStream(this._filename, this._options.FileMode == FileMode.ReadOnly ? System.IO.FileMode.Open : System.IO.FileMode.OpenOrCreate, this._options.FileMode == FileMode.ReadOnly ? FileAccess.Read : FileAccess.ReadWrite, this._options.FileMode == FileMode.Exclusive ? FileShare.None : FileShare.ReadWrite);
    if (this._stream.Length != 0L)
      return;
    this._log.Write((byte) 128 /*0x80*/, "initialize new datafile");
    LiteEngine.CreateDatabase((Stream) this._stream, password, this._options.InitialSize);
  }

  public virtual void Dispose()
  {
    if (this._stream == null)
      return;
    this._log.Write((byte) 128 /*0x80*/, "close datafile '{0}'", (object) Path.GetFileName(this._filename));
    this._stream.Dispose();
    this._stream = (FileStream) null;
  }

  public virtual byte[] ReadPage(uint pageID)
  {
    byte[] buffer = new byte[4096 /*0x1000*/];
    long sizeOfPages = BasePage.GetSizeOfPages(pageID);
    lock (this._stream)
    {
      if (this._stream.Position != sizeOfPages)
        this._stream.Seek(sizeOfPages, SeekOrigin.Begin);
      this._stream.Read(buffer, 0, 4096 /*0x1000*/);
    }
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

  public void SetLength(long fileSize)
  {
    if (fileSize > this._options.LimitSize)
      throw LiteException.FileSizeExceeded(this._options.LimitSize);
    this._stream.SetLength(fileSize);
  }

  public long FileLength => this._stream.Length;

  public bool IsJournalEnabled => this._options.Journal;

  public void WriteJournal(ICollection<byte[]> pages, uint lastPageID)
  {
    if (!this._options.Journal)
      return;
    long num = BasePage.GetSizeOfPages(lastPageID + 1U) + BasePage.GetSizeOfPages(pages.Count);
    this._log.Write((byte) 32 /*0x20*/, "extend datafile to journal - {0} pages", (object) pages.Count);
    this._stream.SetLength(num);
    this._stream.Seek(BasePage.GetSizeOfPages(lastPageID + 1U), SeekOrigin.Begin);
    foreach (byte[] page in (IEnumerable<byte[]>) pages)
    {
      this._log.Write((byte) 32 /*0x20*/, "write page #{0:0000} :: {1}", (object) BitConverter.ToUInt32(page, 0), (object) (PageType) page[4]);
      this._stream.Write(page, 0, 4096 /*0x1000*/);
    }
    this._log.Write((byte) 32 /*0x20*/, "flush journal to disk");
    this.Flush();
  }

  public IEnumerable<byte[]> ReadJournal(uint lastPageID)
  {
    long pos = BasePage.GetSizeOfPages(lastPageID + 1U);
    this._stream.Seek(pos, SeekOrigin.Begin);
    byte[] buffer = new byte[4096 /*0x1000*/];
    while (this._stream.Position < this._stream.Length)
    {
      this._stream.Read(buffer, 0, 4096 /*0x1000*/);
      yield return buffer;
      pos += 4096L /*0x1000*/;
      this._stream.Seek(pos, SeekOrigin.Begin);
    }
  }

  public void ClearJournal(uint lastPageID)
  {
    this._log.Write((byte) 32 /*0x20*/, "shrink datafile to remove journal area");
    this.SetLength(BasePage.GetSizeOfPages(lastPageID + 1U));
  }

  public void Flush()
  {
    this._log.Write((byte) 128 /*0x80*/, "flush data from memory to disk");
    this._stream.Flush(this._options.Flush);
  }

  public bool IsExclusive => this._options.FileMode == FileMode.Exclusive;

  public int Lock(LockState state, TimeSpan timeout)
  {
    if (this._options.FileMode != FileMode.Shared)
      return 0;
    int position = state == LockState.Read ? this._lockReadRand.Next(4096 /*0x1000*/, 7096) : 4096 /*0x1000*/;
    int length = state == LockState.Read ? 1 : 3000;
    this._log.Write((byte) 8, "locking file in {0} mode (position: {1})", (object) state.ToString().ToLower(), (object) position);
    this._stream.TryLock((long) position, (long) length, timeout);
    return position;
  }

  public void Unlock(LockState state, int position)
  {
    if (this._options.FileMode != FileMode.Shared)
      return;
    int num;
    switch (state)
    {
      case LockState.Unlocked:
        return;
      case LockState.Read:
        num = 1;
        break;
      default:
        num = 3000;
        break;
    }
    int length = num;
    this._log.Write((byte) 8, "unlocking file in {0} mode (position: {1})", (object) state.ToString().ToLower(), (object) position);
    this._stream.TryUnlock((long) position, (long) length);
  }

  private FileStream CreateFileStream(
    string path,
    System.IO.FileMode mode,
    FileAccess access,
    FileShare share)
  {
    return new FileStream(path, mode, access, share, 4096 /*0x1000*/, System.IO.FileOptions.RandomAccess);
  }
}
