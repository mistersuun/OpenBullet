// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ITraceWriter
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Diagnostics;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal interface ITraceWriter
{
  TraceLevel LevelFilter { get; }

  void Trace(TraceLevel level, string message, Exception ex);
}
