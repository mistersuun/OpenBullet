// Decompiled with JetBrains decompiler
// Type: LiteDB.StreamExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;
using System.IO;

#nullable disable
namespace LiteDB;

internal static class StreamExtensions
{
  public static byte ReadByte(this Stream stream, long position)
  {
    byte[] buffer = new byte[1];
    stream.Seek(position, SeekOrigin.Begin);
    stream.Read(buffer, 0, 1);
    return buffer[0];
  }

  public static void WriteByte(this Stream stream, long position, byte value)
  {
    stream.Seek(position, SeekOrigin.Begin);
    stream.Write(new byte[1]{ value }, 0, 1);
  }

  public static void CopyTo(this Stream input, Stream output)
  {
    byte[] buffer = new byte[4096 /*0x1000*/];
    int count;
    while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
      output.Write(buffer, 0, count);
  }

  public static bool TryUnlock(this FileStream stream, long position, long length)
  {
    if (length == 0L)
      return true;
    try
    {
      stream.Unlock(position, length);
      return true;
    }
    catch (IOException ex)
    {
      return false;
    }
    catch (PlatformNotSupportedException ex)
    {
      throw StreamExtensions.CreateLockNotSupportedException(ex);
    }
  }

  public static void TryLock(this FileStream stream, long position, long length, TimeSpan timeout)
  {
    if (length == 0L)
      return;
    FileHelper.TryExec((Action) (() =>
    {
      try
      {
        stream.Lock(position, length);
      }
      catch (PlatformNotSupportedException ex)
      {
        throw StreamExtensions.CreateLockNotSupportedException(ex);
      }
    }), timeout);
  }

  private static Exception CreateLockNotSupportedException(PlatformNotSupportedException innerEx)
  {
    throw new InvalidOperationException("Your platform does not support FileStream.Lock. Please set mode=Exclusive in your connnection string to avoid this error.", (Exception) innerEx);
  }
}
