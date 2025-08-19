// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.UnexpectedTagNameException
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

[Serializable]
public class UnexpectedTagNameException : WebDriverException
{
  public UnexpectedTagNameException(string expected, string actual)
    : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Element should have been {0} but was {1}", (object) expected, (object) actual))
  {
  }

  public UnexpectedTagNameException()
  {
  }

  public UnexpectedTagNameException(string message)
    : base(message)
  {
  }

  public UnexpectedTagNameException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected UnexpectedTagNameException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
