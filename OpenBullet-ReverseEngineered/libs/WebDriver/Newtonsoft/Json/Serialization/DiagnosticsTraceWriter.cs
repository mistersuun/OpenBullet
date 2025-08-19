// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.DiagnosticsTraceWriter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Diagnostics;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class DiagnosticsTraceWriter : ITraceWriter
{
  public TraceLevel LevelFilter { get; set; }

  private TraceEventType GetTraceEventType(TraceLevel level)
  {
    switch (level)
    {
      case TraceLevel.Error:
        return TraceEventType.Error;
      case TraceLevel.Warning:
        return TraceEventType.Warning;
      case TraceLevel.Info:
        return TraceEventType.Information;
      case TraceLevel.Verbose:
        return TraceEventType.Verbose;
      default:
        throw new ArgumentOutOfRangeException(nameof (level));
    }
  }

  public void Trace(TraceLevel level, string message, Exception ex)
  {
    if (level == TraceLevel.Off)
      return;
    TraceEventCache eventCache = new TraceEventCache();
    TraceEventType traceEventType = this.GetTraceEventType(level);
    foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
    {
      if (!listener.IsThreadSafe)
      {
        lock (listener)
          listener.TraceEvent(eventCache, "Newtonsoft.Json", traceEventType, 0, message);
      }
      else
        listener.TraceEvent(eventCache, "Newtonsoft.Json", traceEventType, 0, message);
      if (System.Diagnostics.Trace.AutoFlush)
        listener.Flush();
    }
  }
}
