// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.DebugOptions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Security;

#nullable disable
namespace Microsoft.Scripting;

internal static class DebugOptions
{
  private static readonly bool _trackPerformance = DebugOptions.ReadDebugOption(nameof (TrackPerformance));

  private static bool ReadOption(string name)
  {
    string str = DebugOptions.ReadString(name);
    return str != null && str.ToLowerInvariant() == "true";
  }

  private static bool ReadDebugOption(string name) => false;

  private static string ReadString(string name)
  {
    try
    {
      return Environment.GetEnvironmentVariable("DLR_" + name);
    }
    catch (SecurityException ex)
    {
      return (string) null;
    }
  }

  private static string ReadDebugString(string name) => (string) null;

  internal static bool TrackPerformance => DebugOptions._trackPerformance;
}
