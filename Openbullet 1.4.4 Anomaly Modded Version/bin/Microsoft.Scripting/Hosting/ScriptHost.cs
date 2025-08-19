// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ScriptHost
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public class ScriptHost : MarshalByRefObject
{
  private ScriptRuntime _runtime;

  internal void SetRuntime(ScriptRuntime runtime)
  {
    this._runtime = runtime;
    this.RuntimeAttached();
  }

  public ScriptRuntime Runtime
  {
    get
    {
      return this._runtime != null ? this._runtime : throw new InvalidOperationException("Host not initialized");
    }
  }

  public virtual PlatformAdaptationLayer PlatformAdaptationLayer => PlatformAdaptationLayer.Default;

  protected virtual void RuntimeAttached()
  {
  }

  protected internal virtual void EngineCreated(ScriptEngine engine)
  {
  }

  public override object InitializeLifetimeService() => (object) null;
}
