// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.FSharpFunction
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal class FSharpFunction
{
  private readonly object _instance;
  private readonly MethodCall<object, object> _invoker;

  public FSharpFunction(object instance, MethodCall<object, object> invoker)
  {
    this._instance = instance;
    this._invoker = invoker;
  }

  public object Invoke(params object[] args) => this._invoker(this._instance, args);
}
