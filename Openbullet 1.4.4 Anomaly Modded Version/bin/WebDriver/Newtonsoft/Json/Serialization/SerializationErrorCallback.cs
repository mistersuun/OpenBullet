// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.SerializationErrorCallback
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Runtime.Serialization;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal delegate void SerializationErrorCallback(
  object o,
  StreamingContext context,
  ErrorContext errorContext);
