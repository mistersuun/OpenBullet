// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ErrorContext
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class ErrorContext
{
  internal ErrorContext(object originalObject, object member, string path, Exception error)
  {
    this.OriginalObject = originalObject;
    this.Member = member;
    this.Error = error;
    this.Path = path;
  }

  internal bool Traced { get; set; }

  public Exception Error { get; }

  public object OriginalObject { get; }

  public object Member { get; }

  public string Path { get; }

  public bool Handled { get; set; }
}
