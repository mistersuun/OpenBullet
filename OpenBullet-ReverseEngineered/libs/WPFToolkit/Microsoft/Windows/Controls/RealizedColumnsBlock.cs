// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.RealizedColumnsBlock
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

#nullable disable
namespace Microsoft.Windows.Controls;

internal struct RealizedColumnsBlock
{
  public RealizedColumnsBlock(int startIndex, int endIndex, int startIndexOffset)
    : this()
  {
    this.StartIndex = startIndex;
    this.EndIndex = endIndex;
    this.StartIndexOffset = startIndexOffset;
  }

  public int StartIndex { get; private set; }

  public int EndIndex { get; private set; }

  public int StartIndexOffset { get; private set; }
}
