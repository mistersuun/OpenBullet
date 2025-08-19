// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.ErrorEventArgs
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class ErrorEventArgs : EventArgs
{
  public object CurrentObject { get; }

  public ErrorContext ErrorContext { get; }

  public ErrorEventArgs(object currentObject, ErrorContext errorContext)
  {
    this.CurrentObject = currentObject;
    this.ErrorContext = errorContext;
  }
}
