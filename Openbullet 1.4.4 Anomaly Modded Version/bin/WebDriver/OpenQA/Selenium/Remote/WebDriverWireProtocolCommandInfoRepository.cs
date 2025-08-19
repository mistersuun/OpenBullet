// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.WebDriverWireProtocolCommandInfoRepository
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Remote;

public sealed class WebDriverWireProtocolCommandInfoRepository : CommandInfoRepository
{
  public WebDriverWireProtocolCommandInfoRepository() => this.InitializeCommandDictionary();

  public override int SpecificationLevel => 0;

  protected override void InitializeCommandDictionary()
  {
    this.TryAddCommand(DriverCommand.DefineDriverMapping, new CommandInfo("POST", "/config/drivers"));
    this.TryAddCommand(DriverCommand.Status, new CommandInfo("GET", "/status"));
    this.TryAddCommand(DriverCommand.NewSession, new CommandInfo("POST", "/session"));
    this.TryAddCommand(DriverCommand.GetSessionList, new CommandInfo("GET", "/sessions"));
    this.TryAddCommand(DriverCommand.GetSessionCapabilities, new CommandInfo("GET", "/session/{sessionId}"));
    this.TryAddCommand(DriverCommand.Quit, new CommandInfo("DELETE", "/session/{sessionId}"));
    this.TryAddCommand(DriverCommand.GetCurrentWindowHandle, new CommandInfo("GET", "/session/{sessionId}/window_handle"));
    this.TryAddCommand(DriverCommand.GetWindowHandles, new CommandInfo("GET", "/session/{sessionId}/window_handles"));
    this.TryAddCommand(DriverCommand.GetCurrentUrl, new CommandInfo("GET", "/session/{sessionId}/url"));
    this.TryAddCommand(DriverCommand.Get, new CommandInfo("POST", "/session/{sessionId}/url"));
    this.TryAddCommand(DriverCommand.GoForward, new CommandInfo("POST", "/session/{sessionId}/forward"));
    this.TryAddCommand(DriverCommand.GoBack, new CommandInfo("POST", "/session/{sessionId}/back"));
    this.TryAddCommand(DriverCommand.Refresh, new CommandInfo("POST", "/session/{sessionId}/refresh"));
    this.TryAddCommand(DriverCommand.ExecuteScript, new CommandInfo("POST", "/session/{sessionId}/execute"));
    this.TryAddCommand(DriverCommand.ExecuteAsyncScript, new CommandInfo("POST", "/session/{sessionId}/execute_async"));
    this.TryAddCommand(DriverCommand.Screenshot, new CommandInfo("GET", "/session/{sessionId}/screenshot"));
    this.TryAddCommand(DriverCommand.ElementScreenshot, new CommandInfo("GET", "/session/{sessionId}/screenshot/{id}"));
    this.TryAddCommand(DriverCommand.SwitchToFrame, new CommandInfo("POST", "/session/{sessionId}/frame"));
    this.TryAddCommand(DriverCommand.SwitchToParentFrame, new CommandInfo("POST", "/session/{sessionId}/frame/parent"));
    this.TryAddCommand(DriverCommand.SwitchToWindow, new CommandInfo("POST", "/session/{sessionId}/window"));
    this.TryAddCommand(DriverCommand.GetAllCookies, new CommandInfo("GET", "/session/{sessionId}/cookie"));
    this.TryAddCommand(DriverCommand.AddCookie, new CommandInfo("POST", "/session/{sessionId}/cookie"));
    this.TryAddCommand(DriverCommand.DeleteAllCookies, new CommandInfo("DELETE", "/session/{sessionId}/cookie"));
    this.TryAddCommand(DriverCommand.DeleteCookie, new CommandInfo("DELETE", "/session/{sessionId}/cookie/{name}"));
    this.TryAddCommand(DriverCommand.GetPageSource, new CommandInfo("GET", "/session/{sessionId}/source"));
    this.TryAddCommand(DriverCommand.GetTitle, new CommandInfo("GET", "/session/{sessionId}/title"));
    this.TryAddCommand(DriverCommand.FindElement, new CommandInfo("POST", "/session/{sessionId}/element"));
    this.TryAddCommand(DriverCommand.FindElements, new CommandInfo("POST", "/session/{sessionId}/elements"));
    this.TryAddCommand(DriverCommand.GetActiveElement, new CommandInfo("POST", "/session/{sessionId}/element/active"));
    this.TryAddCommand(DriverCommand.FindChildElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/element"));
    this.TryAddCommand(DriverCommand.FindChildElements, new CommandInfo("POST", "/session/{sessionId}/element/{id}/elements"));
    this.TryAddCommand(DriverCommand.DescribeElement, new CommandInfo("GET", "/session/{sessionId}/element/{id}"));
    this.TryAddCommand(DriverCommand.ClickElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/click"));
    this.TryAddCommand(DriverCommand.GetElementText, new CommandInfo("GET", "/session/{sessionId}/element/{id}/text"));
    this.TryAddCommand(DriverCommand.SubmitElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/submit"));
    this.TryAddCommand(DriverCommand.SendKeysToElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/value"));
    this.TryAddCommand(DriverCommand.GetElementTagName, new CommandInfo("GET", "/session/{sessionId}/element/{id}/name"));
    this.TryAddCommand(DriverCommand.ClearElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/clear"));
    this.TryAddCommand(DriverCommand.IsElementSelected, new CommandInfo("GET", "/session/{sessionId}/element/{id}/selected"));
    this.TryAddCommand(DriverCommand.IsElementEnabled, new CommandInfo("GET", "/session/{sessionId}/element/{id}/enabled"));
    this.TryAddCommand(DriverCommand.IsElementDisplayed, new CommandInfo("GET", "/session/{sessionId}/element/{id}/displayed"));
    this.TryAddCommand(DriverCommand.GetElementLocation, new CommandInfo("GET", "/session/{sessionId}/element/{id}/location"));
    this.TryAddCommand(DriverCommand.GetElementLocationOnceScrolledIntoView, new CommandInfo("GET", "/session/{sessionId}/element/{id}/location_in_view"));
    this.TryAddCommand(DriverCommand.GetElementSize, new CommandInfo("GET", "/session/{sessionId}/element/{id}/size"));
    this.TryAddCommand(DriverCommand.GetElementValueOfCssProperty, new CommandInfo("GET", "/session/{sessionId}/element/{id}/css/{propertyName}"));
    this.TryAddCommand(DriverCommand.GetElementAttribute, new CommandInfo("GET", "/session/{sessionId}/element/{id}/attribute/{name}"));
    this.TryAddCommand(DriverCommand.ElementEquals, new CommandInfo("GET", "/session/{sessionId}/element/{id}/equals/{other}"));
    this.TryAddCommand(DriverCommand.Close, new CommandInfo("DELETE", "/session/{sessionId}/window"));
    this.TryAddCommand(DriverCommand.GetWindowSize, new CommandInfo("GET", "/session/{sessionId}/window/{windowHandle}/size"));
    this.TryAddCommand(DriverCommand.SetWindowSize, new CommandInfo("POST", "/session/{sessionId}/window/{windowHandle}/size"));
    this.TryAddCommand(DriverCommand.GetWindowPosition, new CommandInfo("GET", "/session/{sessionId}/window/{windowHandle}/position"));
    this.TryAddCommand(DriverCommand.SetWindowPosition, new CommandInfo("POST", "/session/{sessionId}/window/{windowHandle}/position"));
    this.TryAddCommand(DriverCommand.MaximizeWindow, new CommandInfo("POST", "/session/{sessionId}/window/{windowHandle}/maximize"));
    this.TryAddCommand(DriverCommand.MinimizeWindow, new CommandInfo("POST", "/session/{sessionId}/window/minimize"));
    this.TryAddCommand(DriverCommand.FullScreenWindow, new CommandInfo("POST", "/session/{sessionId}/window/fullscreen"));
    this.TryAddCommand(DriverCommand.GetOrientation, new CommandInfo("GET", "/session/{sessionId}/orientation"));
    this.TryAddCommand(DriverCommand.SetOrientation, new CommandInfo("POST", "/session/{sessionId}/orientation"));
    this.TryAddCommand(DriverCommand.DismissAlert, new CommandInfo("POST", "/session/{sessionId}/dismiss_alert"));
    this.TryAddCommand(DriverCommand.AcceptAlert, new CommandInfo("POST", "/session/{sessionId}/accept_alert"));
    this.TryAddCommand(DriverCommand.GetAlertText, new CommandInfo("GET", "/session/{sessionId}/alert_text"));
    this.TryAddCommand(DriverCommand.SetAlertValue, new CommandInfo("POST", "/session/{sessionId}/alert_text"));
    this.TryAddCommand(DriverCommand.SetAlertCredentials, new CommandInfo("POST", "/session/{sessionId}/alert/credentials"));
    this.TryAddCommand(DriverCommand.SetTimeouts, new CommandInfo("POST", "/session/{sessionId}/timeouts"));
    this.TryAddCommand(DriverCommand.ImplicitlyWait, new CommandInfo("POST", "/session/{sessionId}/timeouts/implicit_wait"));
    this.TryAddCommand(DriverCommand.SetAsyncScriptTimeout, new CommandInfo("POST", "/session/{sessionId}/timeouts/async_script"));
    this.TryAddCommand(DriverCommand.MouseClick, new CommandInfo("POST", "/session/{sessionId}/click"));
    this.TryAddCommand(DriverCommand.MouseDoubleClick, new CommandInfo("POST", "/session/{sessionId}/doubleclick"));
    this.TryAddCommand(DriverCommand.MouseDown, new CommandInfo("POST", "/session/{sessionId}/buttondown"));
    this.TryAddCommand(DriverCommand.MouseUp, new CommandInfo("POST", "/session/{sessionId}/buttonup"));
    this.TryAddCommand(DriverCommand.MouseMoveTo, new CommandInfo("POST", "/session/{sessionId}/moveto"));
    this.TryAddCommand(DriverCommand.SendKeysToActiveElement, new CommandInfo("POST", "/session/{sessionId}/keys"));
    this.TryAddCommand(DriverCommand.TouchSingleTap, new CommandInfo("POST", "/session/{sessionId}/touch/click"));
    this.TryAddCommand(DriverCommand.TouchPress, new CommandInfo("POST", "/session/{sessionId}/touch/down"));
    this.TryAddCommand(DriverCommand.TouchRelease, new CommandInfo("POST", "/session/{sessionId}/touch/up"));
    this.TryAddCommand(DriverCommand.TouchMove, new CommandInfo("POST", "/session/{sessionId}/touch/move"));
    this.TryAddCommand(DriverCommand.TouchScroll, new CommandInfo("POST", "/session/{sessionId}/touch/scroll"));
    this.TryAddCommand(DriverCommand.TouchDoubleTap, new CommandInfo("POST", "/session/{sessionId}/touch/doubleclick"));
    this.TryAddCommand(DriverCommand.TouchLongPress, new CommandInfo("POST", "/session/{sessionId}/touch/longclick"));
    this.TryAddCommand(DriverCommand.TouchFlick, new CommandInfo("POST", "/session/{sessionId}/touch/flick"));
    this.TryAddCommand(DriverCommand.UploadFile, new CommandInfo("POST", "/session/{sessionId}/file"));
    this.TryAddCommand(DriverCommand.GetAvailableLogTypes, new CommandInfo("GET", "/session/{sessionId}/log/types"));
    this.TryAddCommand(DriverCommand.GetLog, new CommandInfo("POST", "/session/{sessionId}/log"));
    this.TryAddCommand(DriverCommand.GetLocation, new CommandInfo("GET", "/session/{sessionId}/location"));
    this.TryAddCommand(DriverCommand.SetLocation, new CommandInfo("POST", "/session/{sessionId}/location"));
    this.TryAddCommand(DriverCommand.GetAppCache, new CommandInfo("GET", "/session/{sessionId}/application_cache"));
    this.TryAddCommand(DriverCommand.GetAppCacheStatus, new CommandInfo("GET", "/session/{sessionId}/application_cache/status"));
    this.TryAddCommand(DriverCommand.ClearAppCache, new CommandInfo("DELETE", "/session/{sessionId}/application_cache/clear"));
    this.TryAddCommand(DriverCommand.GetLocalStorageKeys, new CommandInfo("GET", "/session/{sessionId}/local_storage"));
    this.TryAddCommand(DriverCommand.SetLocalStorageItem, new CommandInfo("POST", "/session/{sessionId}/local_storage"));
    this.TryAddCommand(DriverCommand.ClearLocalStorage, new CommandInfo("DELETE", "/session/{sessionId}/local_storage"));
    this.TryAddCommand(DriverCommand.GetLocalStorageItem, new CommandInfo("GET", "/session/{sessionId}/local_storage/key/{key}"));
    this.TryAddCommand(DriverCommand.RemoveLocalStorageItem, new CommandInfo("DELETE", "/session/{sessionId}/local_storage/key/{key}"));
    this.TryAddCommand(DriverCommand.GetLocalStorageSize, new CommandInfo("GET", "/session/{sessionId}/local_storage/size"));
    this.TryAddCommand(DriverCommand.GetSessionStorageKeys, new CommandInfo("GET", "/session/{sessionId}/session_storage"));
    this.TryAddCommand(DriverCommand.SetSessionStorageItem, new CommandInfo("POST", "/session/{sessionId}/session_storage"));
    this.TryAddCommand(DriverCommand.ClearSessionStorage, new CommandInfo("DELETE", "/session/{sessionId}/session_storage"));
    this.TryAddCommand(DriverCommand.GetSessionStorageItem, new CommandInfo("GET", "/session/{sessionId}/session_storage/key/{key}"));
    this.TryAddCommand(DriverCommand.RemoveSessionStorageItem, new CommandInfo("DELETE", "/session/{sessionId}/session_storage/key/{key}"));
    this.TryAddCommand(DriverCommand.GetSessionStorageSize, new CommandInfo("GET", "/session/{sessionId}/session_storage/size"));
  }
}
