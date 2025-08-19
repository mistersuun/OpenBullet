// Decompiled with JetBrains decompiler
// Type: LiteDB.BinaryExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

internal static class BinaryExtensions
{
  public static int BinaryCompareTo(this byte[] lh, byte[] rh)
  {
    if (lh == null)
      return rh != null ? -1 : 0;
    if (rh == null)
      return 1;
    int num = 0;
    int index1 = 0;
    for (int index2 = Math.Min(lh.Length, rh.Length); num == 0 && index1 < index2; ++index1)
      num = lh[index1].CompareTo(rh[index1]);
    if (num != 0)
      return num >= 0 ? 1 : -1;
    if (index1 != lh.Length)
      return 1;
    return index1 != rh.Length ? -1 : 0;
  }
}
