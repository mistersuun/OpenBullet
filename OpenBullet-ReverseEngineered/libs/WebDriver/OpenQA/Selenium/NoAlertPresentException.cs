// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.NoAlertPresentException
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace OpenQA.Selenium;

[Serializable]
public class NoAlertPresentException : NotFoundException
{
  public NoAlertPresentException()
  {
  }

  public NoAlertPresentException(string message)
    : base(message)
  {
  }

  public NoAlertPresentException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected NoAlertPresentException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
