// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.DebugInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

[Serializable]
public class DebugInfo
{
  public int StartLine;
  public int EndLine;
  public int Index;
  public string FileName;
  public bool IsClear;
  private static readonly DebugInfo.DebugInfoComparer _debugComparer = new DebugInfo.DebugInfoComparer();

  public static DebugInfo GetMatchingDebugInfo(DebugInfo[] debugInfos, int index)
  {
    DebugInfo debugInfo = new DebugInfo() { Index = index };
    int index1 = Array.BinarySearch<DebugInfo>(debugInfos, debugInfo, (IComparer<DebugInfo>) DebugInfo._debugComparer);
    if (index1 < 0)
    {
      int num = ~index1;
      if (num == 0)
        return (DebugInfo) null;
      index1 = num - 1;
    }
    return debugInfos[index1];
  }

  public override string ToString()
  {
    if (this.IsClear)
      return $"{this.Index}: clear";
    return $"{this.Index}: [{this.StartLine}-{this.EndLine}] '{this.FileName}'";
  }

  private class DebugInfoComparer : IComparer<DebugInfo>
  {
    int IComparer<DebugInfo>.Compare(DebugInfo d1, DebugInfo d2)
    {
      if (d1.Index > d2.Index)
        return 1;
      return d1.Index == d2.Index ? 0 : -1;
    }
  }
}
