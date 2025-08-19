// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.StackTraceElement
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class StackTraceElement
{
  private string fileName = string.Empty;
  private string className = string.Empty;
  private int lineNumber;
  private string methodName = string.Empty;

  public StackTraceElement()
  {
  }

  public StackTraceElement(Dictionary<string, object> elementAttributes)
  {
    if (elementAttributes == null)
      return;
    if (elementAttributes.ContainsKey(nameof (className)) && elementAttributes[nameof (className)] != null)
      this.className = elementAttributes[nameof (className)].ToString();
    if (elementAttributes.ContainsKey(nameof (methodName)) && elementAttributes[nameof (methodName)] != null)
      this.methodName = elementAttributes[nameof (methodName)].ToString();
    if (elementAttributes.ContainsKey(nameof (lineNumber)))
    {
      int result = 0;
      if (int.TryParse(elementAttributes[nameof (lineNumber)].ToString(), out result))
        this.lineNumber = result;
    }
    if (!elementAttributes.ContainsKey(nameof (fileName)) || elementAttributes[nameof (fileName)] == null)
      return;
    this.fileName = elementAttributes[nameof (fileName)].ToString();
  }

  [JsonProperty("fileName")]
  public string FileName
  {
    get => this.fileName;
    set => this.fileName = value;
  }

  [JsonProperty("className")]
  public string ClassName
  {
    get => this.className;
    set => this.className = value;
  }

  [JsonProperty("lineNumber")]
  public int LineNumber
  {
    get => this.lineNumber;
    set => this.lineNumber = value;
  }

  [JsonProperty("methodName")]
  public string MethodName
  {
    get => this.methodName;
    set => this.methodName = value;
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "at {0}.{1} ({2}, {3})", (object) this.className, (object) this.methodName, (object) this.fileName, (object) this.lineNumber);
  }
}
