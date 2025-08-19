// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.MethodCall`2
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal delegate TResult MethodCall<T, TResult>(T target, params object[] args);
