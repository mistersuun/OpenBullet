// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.RangeInfo
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Modules;

internal class RangeInfo : CharInfo
{
  internal readonly int First;
  internal readonly int Last;

  internal RangeInfo(int first, int last, string[] info)
    : base(info)
  {
    this.First = first;
    this.Last = last;
  }
}
