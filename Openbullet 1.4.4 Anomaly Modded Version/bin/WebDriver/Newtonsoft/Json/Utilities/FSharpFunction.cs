// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.FSharpFunction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

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
