// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.ErrorResponse
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class ErrorResponse
{
  private StackTraceElement[] stackTrace;
  private string message = string.Empty;
  private string className = string.Empty;
  private string screenshot = string.Empty;

  public ErrorResponse()
  {
  }

  public ErrorResponse(Dictionary<string, object> responseValue)
  {
    if (responseValue == null)
      return;
    if (responseValue.ContainsKey(nameof (message)))
      this.message = responseValue[nameof (message)] == null ? "The error did not contain a message." : responseValue[nameof (message)].ToString();
    if (responseValue.ContainsKey("screen") && responseValue["screen"] != null)
      this.screenshot = responseValue["screen"].ToString();
    if (responseValue.ContainsKey("class") && responseValue["class"] != null)
      this.className = responseValue["class"].ToString();
    if (!responseValue.ContainsKey(nameof (stackTrace)) || !(responseValue[nameof (stackTrace)] is object[] objArray))
      return;
    List<StackTraceElement> stackTraceElementList = new List<StackTraceElement>();
    for (int index = 0; index < objArray.Length; ++index)
    {
      if (objArray[index] is Dictionary<string, object> elementAttributes)
        stackTraceElementList.Add(new StackTraceElement(elementAttributes));
    }
    this.stackTrace = stackTraceElementList.ToArray();
  }

  public string Message
  {
    get => this.message;
    set => this.message = value;
  }

  public string ClassName
  {
    get => this.className;
    set => this.className = value;
  }

  public string Screenshot
  {
    get => this.screenshot;
    set => this.screenshot = value;
  }

  public StackTraceElement[] StackTrace
  {
    get => this.stackTrace;
    set => this.stackTrace = value;
  }
}
