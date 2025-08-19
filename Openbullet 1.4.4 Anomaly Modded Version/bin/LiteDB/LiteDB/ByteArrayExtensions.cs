// Decompiled with JetBrains decompiler
// Type: LiteDB.ByteArrayExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

internal static class ByteArrayExtensions
{
  public static string ToHex(this byte[] bytes)
  {
    char[] chArray = new char[bytes.Length * 2];
    int index1 = 0;
    int index2 = 0;
    while (index1 < bytes.Length)
    {
      byte num1 = (byte) ((uint) bytes[index1] >> 4);
      chArray[index2] = num1 > (byte) 9 ? (char) ((int) num1 + 55 + 32 /*0x20*/) : (char) ((int) num1 + 48 /*0x30*/);
      byte num2 = (byte) ((uint) bytes[index1] & 15U);
      int num3;
      chArray[num3 = index2 + 1] = num2 > (byte) 9 ? (char) ((int) num2 + 55 + 32 /*0x20*/) : (char) ((int) num2 + 48 /*0x30*/);
      ++index1;
      index2 = num3 + 1;
    }
    return new string(chArray);
  }
}
