// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.LoadableComponentException
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

[Serializable]
public class LoadableComponentException : WebDriverException
{
  public LoadableComponentException()
  {
  }

  public LoadableComponentException(string message)
    : base(message)
  {
  }

  public LoadableComponentException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected LoadableComponentException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
