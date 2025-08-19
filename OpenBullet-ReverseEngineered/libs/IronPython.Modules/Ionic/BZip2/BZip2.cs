// Decompiled with JetBrains decompiler
// Type: Ionic.BZip2.BZip2
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

#nullable disable
namespace Ionic.BZip2;

internal static class BZip2
{
  public static readonly int BlockSizeMultiple = 100000;
  public static readonly int MinBlockSize = 1;
  public static readonly int MaxBlockSize = 9;
  public static readonly int MaxAlphaSize = 258;
  public static readonly int MaxCodeLength = 23;
  public static readonly char RUNA = char.MinValue;
  public static readonly char RUNB = '\u0001';
  public static readonly int NGroups = 6;
  public static readonly int G_SIZE = 50;
  public static readonly int N_ITERS = 4;
  public static readonly int MaxSelectors = 2 + 900000 / Ionic.BZip2.BZip2.G_SIZE;
  public static readonly int NUM_OVERSHOOT_BYTES = 20;
  internal static readonly int QSORT_STACK_SIZE = 1000;

  internal static T[][] InitRectangularArray<T>(int d1, int d2)
  {
    T[][] objArray = new T[d1][];
    for (int index = 0; index < d1; ++index)
      objArray[index] = new T[d2];
    return objArray;
  }
}
