// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.UnhandledAlertException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace OpenQA.Selenium;

[Serializable]
public class UnhandledAlertException : WebDriverException
{
  private string alertText;

  public UnhandledAlertException()
  {
  }

  public UnhandledAlertException(string message)
    : base(message)
  {
  }

  public UnhandledAlertException(string message, string alertText)
    : base(message)
  {
    this.alertText = alertText;
  }

  public UnhandledAlertException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected UnhandledAlertException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public string AlertText => this.alertText;

  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData(info, context);
  }
}
