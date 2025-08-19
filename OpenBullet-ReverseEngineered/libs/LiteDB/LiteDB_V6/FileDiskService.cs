// Decompiled with JetBrains decompiler
// Type: LiteDB_V6.FileDiskService
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using LiteDB;
using System;
using System.IO;

#nullable disable
namespace LiteDB_V6;

internal class FileDiskService : IDisposable
{
  private const int PAGE_TYPE_POSITION = 4;
  private static byte[] SALT = new byte[16 /*0x10*/]
  {
    (byte) 22,
    (byte) 174,
    (byte) 191,
    (byte) 32 /*0x20*/,
    (byte) 1,
    (byte) 160 /*0xA0*/,
    (byte) 169,
    (byte) 82,
    (byte) 52,
    (byte) 26,
    (byte) 69,
    (byte) 85,
    (byte) 74,
    (byte) 225,
    (byte) 50,
    (byte) 29
  };
  private Stream _stream;
  private AesEncryption _crypto;
  private byte[] _password;

  public FileDiskService(Stream stream, string password)
  {
    this._stream = stream;
    if (password == null)
      return;
    this._crypto = new AesEncryption(password, FileDiskService.SALT);
    this._password = AesEncryption.HashSHA1(password);
  }

  public void Dispose()
  {
    if (this._crypto == null)
      return;
    this._crypto.Dispose();
  }

  public virtual byte[] ReadPage(uint pageID)
  {
    byte[] numArray = new byte[4096 /*0x1000*/];
    long sizeOfPages = BasePage.GetSizeOfPages(pageID);
    if (this._stream.Position != sizeOfPages)
      this._stream.Seek(sizeOfPages, SeekOrigin.Begin);
    this._stream.Read(numArray, 0, 4096 /*0x1000*/);
    if (pageID == 0U && this._crypto != null)
    {
      if (this._password.BinaryCompareTo(((HeaderPage) BasePage.ReadPage(numArray)).Password) != 0)
        throw LiteException.DatabaseWrongPassword();
    }
    else if (this._crypto != null)
      numArray = this._crypto.Decrypt(numArray);
    return numArray;
  }
}
