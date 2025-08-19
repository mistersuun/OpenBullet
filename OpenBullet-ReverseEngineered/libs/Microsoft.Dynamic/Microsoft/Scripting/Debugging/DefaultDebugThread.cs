// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DefaultDebugThread
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Debugging.CompilerServices;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal sealed class DefaultDebugThread : DebugThread
{
  private readonly List<DefaultDebugThread.FrameRuntimeVariablesPair> _frames;

  internal DefaultDebugThread(DebugContext debugContext)
    : base(debugContext)
  {
    this._frames = new List<DefaultDebugThread.FrameRuntimeVariablesPair>();
  }

  internal void LiftVariables(IRuntimeVariables runtimeVariables)
  {
    this._frames.Add(new DefaultDebugThread.FrameRuntimeVariablesPair((IRuntimeVariables) new DebugRuntimeVariables(runtimeVariables), (DebugFrame) null));
  }

  internal override IEnumerable<DebugFrame> Frames
  {
    get
    {
      for (int i = this._frames.Count - 1; i >= 0; --i)
        yield return this.GetFrame(i);
    }
  }

  internal override DebugFrame GetLeafFrame() => this.GetFrame(this._frames.Count - 1);

  internal override bool TryGetLeafFrame(ref DebugFrame frame)
  {
    if (this._frames.Count > 0)
    {
      frame = this._frames[this._frames.Count - 1].Frame;
      return frame != null;
    }
    frame = (DebugFrame) null;
    return false;
  }

  internal override int FrameCount => this._frames.Count;

  internal override void PushExistingFrame(DebugFrame frame)
  {
    this._frames.Add(new DefaultDebugThread.FrameRuntimeVariablesPair((IRuntimeVariables) null, frame));
  }

  internal override bool PopFrame()
  {
    this._frames.RemoveAt(this._frames.Count - 1);
    return this._frames.Count == 0;
  }

  internal override FunctionInfo GetLeafFrameFunctionInfo(out int stackDepth)
  {
    int index = this._frames.Count - 1;
    if (index >= 0)
    {
      stackDepth = index;
      DebugFrame frame = this._frames[index].Frame;
      return frame != null ? frame.FunctionInfo : ((IDebugRuntimeVariables) this._frames[index].RuntimeVariables).FunctionInfo;
    }
    stackDepth = int.MaxValue;
    return (FunctionInfo) null;
  }

  private DebugFrame GetFrame(int index)
  {
    DebugFrame frame = (DebugFrame) null;
    if (index >= 0)
    {
      frame = this._frames[index].Frame;
      if (frame == null)
      {
        IDebugRuntimeVariables runtimeVariables = this._frames[index].RuntimeVariables as IDebugRuntimeVariables;
        frame = new DebugFrame((DebugThread) this, runtimeVariables.FunctionInfo, (IRuntimeVariables) runtimeVariables, index);
        this._frames[index] = new DefaultDebugThread.FrameRuntimeVariablesPair((IRuntimeVariables) null, frame);
      }
    }
    if (index == this._frames.Count - 1)
    {
      frame.IsInTraceback = this.IsInTraceback;
      frame.ThrownException = this.ThrownException;
    }
    return frame;
  }

  private struct FrameRuntimeVariablesPair(IRuntimeVariables runtimeVariables, DebugFrame frame)
  {
    public IRuntimeVariables RuntimeVariables = runtimeVariables;
    public DebugFrame Frame = frame;
  }
}
