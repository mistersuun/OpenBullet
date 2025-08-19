// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugGenerator`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal sealed class DebugGenerator<T> : IEnumerator<T>, IDisposable, IEnumerator
{
  private DebugFrame _frame;

  internal DebugGenerator(DebugFrame frame)
  {
    this._frame = frame;
    this._frame.RemapToGenerator(frame.FunctionInfo.Version);
  }

  public T Current => (T) ((IEnumerator) this).Current;

  void IDisposable.Dispose()
  {
    if (this._frame.Generator is IDisposable generator)
      generator.Dispose();
    GC.SuppressFinalize((object) this);
  }

  object IEnumerator.Current => ((IEnumerator) this._frame.Generator).Current;

  public bool MoveNext()
  {
    this._frame.Thread.PushExistingFrame(this._frame);
    if (this._frame.FunctionInfo.SequencePoints[this._frame.CurrentLocationCookie].SourceFile.DebugMode == DebugMode.FullyEnabled || this._frame.FunctionInfo.SequencePoints[this._frame.CurrentLocationCookie].SourceFile.DebugMode == DebugMode.TracePoints && this._frame.FunctionInfo.GetTraceLocations()[this._frame.CurrentLocationCookie])
    {
      try
      {
        this._frame.DebugContext.DispatchDebugEvent(this._frame.Thread, this._frame.CurrentLocationCookie, TraceEventKind.FrameEnter, (object) null);
      }
      catch (ForceToGeneratorLoopException ex)
      {
      }
    }
    try
    {
      bool moveNext;
      this._frame.DebugContext.GeneratorLoopProc(this._frame, out moveNext);
      return moveNext;
    }
    finally
    {
      if (this._frame.FunctionInfo.SequencePoints[0].SourceFile.DebugMode == DebugMode.FullyEnabled || this._frame.FunctionInfo.SequencePoints[this._frame.CurrentLocationCookie].SourceFile.DebugMode == DebugMode.TracePoints && this._frame.FunctionInfo.GetTraceLocations()[this._frame.CurrentLocationCookie])
        this._frame.DebugContext.DispatchDebugEvent(this._frame.Thread, this._frame.CurrentLocationCookie, TraceEventKind.FrameExit, (object) this.Current);
      if (this._frame.Thread.PopFrame() && this._frame.DebugContext.DebugMode == DebugMode.FullyEnabled)
        this._frame.DebugContext.DispatchDebugEvent(this._frame.Thread, int.MaxValue, TraceEventKind.ThreadExit, (object) null);
    }
  }

  public void Reset() => ((IEnumerator) this._frame.Generator).Reset();
}
