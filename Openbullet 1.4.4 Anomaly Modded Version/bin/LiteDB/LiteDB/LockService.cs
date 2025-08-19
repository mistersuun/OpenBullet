// Decompiled with JetBrains decompiler
// Type: LiteDB.LockService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Threading;

#nullable disable
namespace LiteDB;

public class LockService
{
  private TimeSpan _timeout;
  private IDiskService _disk;
  private CacheService _cache;
  private Logger _log;
  private ReaderWriterLockSlim _thread = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

  internal LockService(IDiskService disk, CacheService cache, TimeSpan timeout, Logger log)
  {
    this._disk = disk;
    this._cache = cache;
    this._log = log;
    this._timeout = timeout;
  }

  public LockState ThreadState
  {
    get
    {
      if (this._thread.IsWriteLockHeld)
        return LockState.Write;
      return this._thread.CurrentReadCount <= 0 ? LockState.Unlocked : LockState.Read;
    }
  }

  public int ThreadId => Thread.CurrentThread.ManagedThreadId;

  public LockControl Read()
  {
    if (this._thread.IsReadLockHeld || this._thread.IsWriteLockHeld)
      return new LockControl(false, (Action) (() => { }));
    if (!this._thread.TryEnterReadLock(this._timeout))
      throw LiteException.LockTimeout(this._timeout);
    this._log.Write((byte) 8, "entered in read lock mode in thread #{0}", (object) this.ThreadId);
    int position = this._disk.Lock(LockState.Read, this._timeout);
    return new LockControl(this.DetectDatabaseChanges(), (Action) (() =>
    {
      this._disk.Unlock(LockState.Read, position);
      this._thread.ExitReadLock();
      this._log.Write((byte) 8, "exited read lock mode in thread #{0}", (object) this.ThreadId);
    }));
  }

  public LockControl Write()
  {
    if (this._thread.IsWriteLockHeld)
      return new LockControl(false, (Action) (() => { }));
    if (this._thread.IsReadLockHeld)
      throw new NotSupportedException("Not support Write lock inside a Read lock");
    if (!this._thread.TryEnterWriteLock(this._timeout))
      throw LiteException.LockTimeout(this._timeout);
    this._log.Write((byte) 8, "entered in write lock mode in thread #{0}", (object) this.ThreadId);
    int position = this._disk.Lock(LockState.Write, this._timeout);
    return new LockControl(this.DetectDatabaseChanges(), (Action) (() =>
    {
      this._disk.Unlock(LockState.Write, position);
      this._thread.ExitWriteLock();
      this._log.Write((byte) 8, "exited write lock mode in thread #{0}", (object) this.ThreadId);
    }));
  }

  private bool DetectDatabaseChanges()
  {
    if (this._disk.IsExclusive || this._cache.CleanUsed == 0)
      return false;
    this._log.Write((byte) 64 /*0x40*/, "checking disk to detect database changes from another process");
    int changeId = !(this._cache.GetPage(0U) is HeaderPage page1) ? 0 : (int) page1.ChangeID;
    HeaderPage page2 = BasePage.ReadPage(this._disk.ReadPage(0U)) as HeaderPage;
    if (page2.Recovery)
    {
      this._log.Write((byte) 1, "datafile in recovery mode, need re-open database");
      throw LiteException.NeedRecover();
    }
    if ((int) page2.ChangeID == changeId)
      return false;
    this._log.Write((byte) 64 /*0x40*/, "file changed from another process, cleaning all cache pages");
    this._cache.ClearPages();
    this._cache.AddPage((BasePage) page2);
    return true;
  }
}
