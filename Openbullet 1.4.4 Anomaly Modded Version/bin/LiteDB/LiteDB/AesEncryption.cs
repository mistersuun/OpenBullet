// Decompiled with JetBrains decompiler
// Type: LiteDB.AesEncryption
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace LiteDB;

internal class AesEncryption
{
  private Aes _aes;

  public AesEncryption(string password, byte[] salt)
  {
    this._aes = Aes.Create();
    this._aes.Padding = PaddingMode.Zeros;
    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt);
    using ((IDisposable) rfc2898DeriveBytes)
    {
      this._aes.Key = rfc2898DeriveBytes.GetBytes(32 /*0x20*/);
      this._aes.IV = rfc2898DeriveBytes.GetBytes(16 /*0x10*/);
    }
  }

  public byte[] Encrypt(byte[] bytes)
  {
    using (ICryptoTransform encryptor = this._aes.CreateEncryptor())
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
        {
          cryptoStream.Write(bytes, 0, bytes.Length);
          cryptoStream.FlushFinalBlock();
          memoryStream.Position = 0L;
          byte[] buffer = new byte[memoryStream.Length];
          memoryStream.Read(buffer, 0, buffer.Length);
          return buffer;
        }
      }
    }
  }

  public byte[] Decrypt(byte[] encryptedValue)
  {
    using (ICryptoTransform decryptor = this._aes.CreateDecryptor())
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Write))
        {
          cryptoStream.Write(encryptedValue, 0, encryptedValue.Length);
          cryptoStream.FlushFinalBlock();
          memoryStream.Position = 0L;
          byte[] buffer = new byte[memoryStream.Length];
          memoryStream.Read(buffer, 0, buffer.Length);
          return buffer;
        }
      }
    }
  }

  public static byte[] HashSHA1(string password)
  {
    return SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
  }

  public static byte[] Salt(int maxLength = 16 /*0x10*/)
  {
    byte[] data = new byte[maxLength];
    RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
    using ((IDisposable) randomNumberGenerator)
      randomNumberGenerator.GetBytes(data);
    return data;
  }

  public void Dispose()
  {
    if (this._aes == null)
      return;
    this._aes = (Aes) null;
  }
}
