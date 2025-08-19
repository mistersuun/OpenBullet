// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ExceptionHelpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class ExceptionHelpers
{
  private const string prevStackTraces = "PreviousStackTraces";

  public static Exception UpdateForRethrow(Exception rethrow)
  {
    StackTrace stackTrace = new StackTrace(rethrow, true);
    List<StackTrace> traces;
    if (!ExceptionHelpers.TryGetAssociatedStackTraces(rethrow, out traces))
    {
      traces = new List<StackTrace>();
      ExceptionHelpers.AssociateStackTraces(rethrow, traces);
    }
    traces.Add(stackTrace);
    return rethrow;
  }

  public static IList<StackTrace> GetExceptionStackTraces(Exception rethrow)
  {
    List<StackTrace> traces;
    return !ExceptionHelpers.TryGetAssociatedStackTraces(rethrow, out traces) ? (IList<StackTrace>) null : (IList<StackTrace>) traces;
  }

  private static void AssociateStackTraces(Exception e, List<StackTrace> traces)
  {
    e.Data[(object) "PreviousStackTraces"] = (object) traces;
  }

  private static bool TryGetAssociatedStackTraces(Exception e, out List<StackTrace> traces)
  {
    traces = e.Data[(object) "PreviousStackTraces"] as List<StackTrace>;
    return traces != null;
  }
}
