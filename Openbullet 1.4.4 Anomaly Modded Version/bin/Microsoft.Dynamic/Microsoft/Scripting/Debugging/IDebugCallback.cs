// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.IDebugCallback
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal interface IDebugCallback
{
  void OnDebugEvent(
    TraceEventKind kind,
    DebugThread thread,
    FunctionInfo functionInfo,
    int sequencePointIndex,
    int stackDepth,
    object payload);
}
