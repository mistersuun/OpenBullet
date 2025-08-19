// Decompiled with JetBrains decompiler
// Type: LiteDB.IDiskService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace LiteDB;

public interface IDiskService : IDisposable
{
  void Initialize(Logger log, string password);

  byte[] ReadPage(uint pageID);

  void WritePage(uint pageID, byte[] buffer);

  void SetLength(long fileSize);

  long FileLength { get; }

  bool IsExclusive { get; }

  bool IsJournalEnabled { get; }

  IEnumerable<byte[]> ReadJournal(uint lastPageID);

  void WriteJournal(ICollection<byte[]> pages, uint lastPageID);

  void ClearJournal(uint lastPageID);

  void Flush();

  int Lock(LockState state, TimeSpan timeout);

  void Unlock(LockState state, int position);
}
