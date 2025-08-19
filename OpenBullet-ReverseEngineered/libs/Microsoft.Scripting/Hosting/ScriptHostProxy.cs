// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptHostProxy
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;

#nullable disable
namespace Microsoft.Scripting.Hosting;

internal sealed class ScriptHostProxy : DynamicRuntimeHostingProvider
{
  private readonly ScriptHost _host;

  public ScriptHostProxy(ScriptHost host) => this._host = host;

  public override PlatformAdaptationLayer PlatformAdaptationLayer
  {
    get => this._host.PlatformAdaptationLayer;
  }
}
