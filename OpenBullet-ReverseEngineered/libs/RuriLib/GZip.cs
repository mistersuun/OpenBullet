// Decompiled with JetBrains decompiler
// Type: RuriLib.GZip
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System.IO;
using System.IO.Compression;
using System.Text;

#nullable disable
namespace RuriLib;

public static class GZip
{
  private static void CopyTo(Stream src, Stream dest)
  {
    byte[] buffer = new byte[4096 /*0x1000*/];
    int count;
    while ((count = src.Read(buffer, 0, buffer.Length)) != 0)
      dest.Write(buffer, 0, count);
  }

  public static byte[] Zip(string str)
  {
    using (MemoryStream memoryStream1 = new MemoryStream(Encoding.UTF8.GetBytes(str)))
    {
      using (MemoryStream memoryStream2 = new MemoryStream())
      {
        using (GZipStream destination = new GZipStream((Stream) memoryStream2, CompressionMode.Compress))
          memoryStream1.CopyTo((Stream) destination);
        return memoryStream2.ToArray();
      }
    }
  }

  public static string Unzip(byte[] bytes)
  {
    using (MemoryStream memoryStream = new MemoryStream(bytes))
    {
      using (MemoryStream destination = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress))
          gzipStream.CopyTo((Stream) destination);
        return Encoding.UTF8.GetString(destination.ToArray());
      }
    }
  }
}
