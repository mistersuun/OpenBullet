// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugThread
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Debugging;

[DebuggerDisplay("ThreadId = {_threadId}")]
public abstract class DebugThread
{
  private readonly int _threadId;

  internal DebugThread(DebugContext debugContext)
  {
    this.DebugContext = debugContext;
    this._threadId = ThreadingUtils.GetCurrentThreadId();
  }

  internal DebugContext DebugContext { get; }

  internal Exception ThrownException { get; set; }

  internal bool IsCurrentThread => this._threadId == ThreadingUtils.GetCurrentThreadId();

  internal bool IsInTraceback { get; set; }

  internal abstract IEnumerable<DebugFrame> Frames { get; }

  internal abstract DebugFrame GetLeafFrame();

  internal abstract bool TryGetLeafFrame(ref DebugFrame frame);

  internal abstract int FrameCount { get; }

  internal abstract void PushExistingFrame(DebugFrame frame);

  internal abstract bool PopFrame();

  internal abstract FunctionInfo GetLeafFrameFunctionInfo(out int stackDepth);
}
