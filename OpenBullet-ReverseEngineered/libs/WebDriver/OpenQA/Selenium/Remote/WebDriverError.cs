// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.WebDriverError
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal static class WebDriverError
{
  public const string ElementClickIntercepted = "element click intercepted";
  public const string ElementNotSelectable = "element not selectable";
  public const string ElementNotInteractable = "element not interactable";
  public const string ElementNotVisible = "element not visible";
  public const string InsecureCertificate = "insecure certificate";
  public const string InvalidArgument = "invalid argument";
  public const string InvalidCookieDomain = "invalid cookie domain";
  public const string InvalidCoordinates = "invalid coordinates";
  public const string InvalidElementCoordinates = "invalid element coordinates";
  public const string InvalidElementState = "invalid element state";
  public const string InvalidSelector = "invalid selector";
  public const string InvalidSessionId = "invalid session id";
  public const string JavaScriptError = "javascript error";
  public const string MoveTargetOutOfBounds = "move target out of bounds";
  public const string NoSuchAlert = "no such alert";
  public const string NoSuchCookie = "no such cookie";
  public const string NoSuchElement = "no such element";
  public const string NoSuchFrame = "no such frame";
  public const string NoSuchWindow = "no such window";
  public const string ScriptTimeout = "script timeout";
  public const string SessionNotCreated = "session not created";
  public const string StaleElementReference = "stale element reference";
  public const string Timeout = "timeout";
  public const string UnableToSetCookie = "unable to set cookie";
  public const string UnableToCaptureScreen = "unable to capture screen";
  public const string UnexpectedAlertOpen = "unexpected alert open";
  public const string UnknownCommand = "unknown command";
  public const string UnknownError = "unknown error";
  public const string UnknownMethod = "unknown method";
  public const string UnsupportedOperation = "unsupported operation";
  private static Dictionary<string, WebDriverResult> resultMap;
  private static object lockObject = new object();

  public static WebDriverResult ResultFromError(string error)
  {
    lock (WebDriverError.lockObject)
    {
      if (WebDriverError.resultMap == null)
        WebDriverError.InitializeResultMap();
    }
    if (!WebDriverError.resultMap.ContainsKey(error))
      error = "unsupported operation";
    return WebDriverError.resultMap[error];
  }

  private static void InitializeResultMap()
  {
    WebDriverError.resultMap = new Dictionary<string, WebDriverResult>();
    WebDriverError.resultMap["element click intercepted"] = WebDriverResult.ElementClickIntercepted;
    WebDriverError.resultMap["element not selectable"] = WebDriverResult.ElementNotSelectable;
    WebDriverError.resultMap["element not visible"] = WebDriverResult.ElementNotDisplayed;
    WebDriverError.resultMap["element not interactable"] = WebDriverResult.ElementNotInteractable;
    WebDriverError.resultMap["insecure certificate"] = WebDriverResult.InsecureCertificate;
    WebDriverError.resultMap["invalid argument"] = WebDriverResult.InvalidArgument;
    WebDriverError.resultMap["invalid cookie domain"] = WebDriverResult.InvalidCookieDomain;
    WebDriverError.resultMap["invalid coordinates"] = WebDriverResult.InvalidElementCoordinates;
    WebDriverError.resultMap["invalid element coordinates"] = WebDriverResult.InvalidElementCoordinates;
    WebDriverError.resultMap["invalid element state"] = WebDriverResult.InvalidElementState;
    WebDriverError.resultMap["invalid selector"] = WebDriverResult.InvalidSelector;
    WebDriverError.resultMap["invalid session id"] = WebDriverResult.NoSuchDriver;
    WebDriverError.resultMap["javascript error"] = WebDriverResult.UnexpectedJavaScriptError;
    WebDriverError.resultMap["move target out of bounds"] = WebDriverResult.MoveTargetOutOfBounds;
    WebDriverError.resultMap["no such alert"] = WebDriverResult.NoAlertPresent;
    WebDriverError.resultMap["no such cookie"] = WebDriverResult.NoSuchCookie;
    WebDriverError.resultMap["no such element"] = WebDriverResult.NoSuchElement;
    WebDriverError.resultMap["no such frame"] = WebDriverResult.NoSuchFrame;
    WebDriverError.resultMap["no such window"] = WebDriverResult.NoSuchWindow;
    WebDriverError.resultMap["script timeout"] = WebDriverResult.AsyncScriptTimeout;
    WebDriverError.resultMap["session not created"] = WebDriverResult.SessionNotCreated;
    WebDriverError.resultMap["stale element reference"] = WebDriverResult.ObsoleteElement;
    WebDriverError.resultMap["timeout"] = WebDriverResult.Timeout;
    WebDriverError.resultMap["unable to set cookie"] = WebDriverResult.UnableToSetCookie;
    WebDriverError.resultMap["unable to capture screen"] = WebDriverResult.UnableToCaptureScreen;
    WebDriverError.resultMap["unexpected alert open"] = WebDriverResult.UnexpectedAlertOpen;
    WebDriverError.resultMap["unknown command"] = WebDriverResult.UnknownCommand;
    WebDriverError.resultMap["unknown error"] = WebDriverResult.UnhandledError;
    WebDriverError.resultMap["unknown method"] = WebDriverResult.UnknownCommand;
    WebDriverError.resultMap["unsupported operation"] = WebDriverResult.UnhandledError;
  }
}
