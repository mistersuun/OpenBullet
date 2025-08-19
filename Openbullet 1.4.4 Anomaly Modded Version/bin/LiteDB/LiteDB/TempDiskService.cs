// Decompiled with JetBrains decompiler
// Type: LiteDB.TempDiskService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace LiteDB;

public class TempDiskService : IDiskService, IDisposable
{
  private const int PAGE_TYPE_POSITION = 4;
  private FileStream _stream;
  private string _filename;

  public void Initialize(Logger log, string password)
  {
  }

  private void InternalInitialize()
  {
    this._filename = Path.Combine(Path.GetTempPath(), $"litedb-sort-{Guid.NewGuid().ToString("n").Substring(0, 6)}.db");
    this._stream = this.CreateFileStream(this._filename, System.IO.FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
  }

  public virtual void Dispose()
  {
    if (this._stream == null)
      return;
    this._stream.Dispose();
    this._stream = (FileStream) null;
    FileHelper.TryDelete(this._filename);
  }

  public virtual byte[] ReadPage(uint pageID)
  {
    if (this._stream == null && pageID == 0U)
      return new HeaderPage() { LastPageID = 1U }.WritePage();
    if (this._stream == null)
      this.InternalInitialize();
    byte[] buffer = new byte[4096 /*0x1000*/];
    long sizeOfPages = BasePage.GetSizeOfPages(pageID);
    if (this._stream.Position != sizeOfPages)
      this._stream.Seek(sizeOfPages, SeekOrigin.Begin);
    this._stream.Read(buffer, 0, 4096 /*0x1000*/);
    return buffer;
  }

  public virtual void WritePage(uint pageID, byte[] buffer)
  {
    if (this._stream == null)
      this.InternalInitialize();
    long sizeOfPages = BasePage.GetSizeOfPages(pageID);
    if (this._stream.Position != sizeOfPages)
      this._stream.Seek(sizeOfPages, SeekOrigin.Begin);
    this._stream.Write(buffer, 0, 4096 /*0x1000*/);
  }

  public void SetLength(long fileSize)
  {
    if (this._stream == null)
      this.InternalInitialize();
    this._stream.SetLength(fileSize);
  }

  public long FileLength
  {
    get
    {
      FileStream stream = this._stream;
      return stream == null ? 0L : stream.Length;
    }
  }

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

  public void Flush()
  {
    if (this._stream == null)
      return;
    this._stream.Flush();
  }

  public bool IsExclusive => true;

  public int Lock(LockState state, TimeSpan timeout) => 0;

  public void Unlock(LockState state, int position)
  {
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
