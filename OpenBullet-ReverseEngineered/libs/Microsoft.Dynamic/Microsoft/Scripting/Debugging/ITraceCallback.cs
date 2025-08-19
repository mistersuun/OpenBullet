// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.ITraceCallback
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Debugging;

public interface ITraceCallback
{
  void OnTraceEvent(
    TraceEventKind kind,
    string name,
    string sourceFileName,
    SourceSpan sourceSpan,
    Func<IDictionary<object, object>> scopeCallback,
    object payload,
    object customPayload);
}
