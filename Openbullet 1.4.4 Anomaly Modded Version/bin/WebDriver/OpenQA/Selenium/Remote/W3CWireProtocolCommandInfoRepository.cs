// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.W3CWireProtocolCommandInfoRepository
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Remote;

public sealed class W3CWireProtocolCommandInfoRepository : CommandInfoRepository
{
  public W3CWireProtocolCommandInfoRepository() => this.InitializeCommandDictionary();

  public override int SpecificationLevel => 1;

  protected override void InitializeCommandDictionary()
  {
    this.TryAddCommand(DriverCommand.Status, new CommandInfo("GET", "/status"));
    this.TryAddCommand(DriverCommand.NewSession, new CommandInfo("POST", "/session"));
    this.TryAddCommand(DriverCommand.Quit, new CommandInfo("DELETE", "/session/{sessionId}"));
    this.TryAddCommand(DriverCommand.GetTimeouts, new CommandInfo("GET", "/session/{sessionId}/timeouts"));
    this.TryAddCommand(DriverCommand.SetTimeouts, new CommandInfo("POST", "/session/{sessionId}/timeouts"));
    this.TryAddCommand(DriverCommand.Get, new CommandInfo("POST", "/session/{sessionId}/url"));
    this.TryAddCommand(DriverCommand.GetCurrentUrl, new CommandInfo("GET", "/session/{sessionId}/url"));
    this.TryAddCommand(DriverCommand.GoBack, new CommandInfo("POST", "/session/{sessionId}/back"));
    this.TryAddCommand(DriverCommand.GoForward, new CommandInfo("POST", "/session/{sessionId}/forward"));
    this.TryAddCommand(DriverCommand.Refresh, new CommandInfo("POST", "/session/{sessionId}/refresh"));
    this.TryAddCommand(DriverCommand.GetTitle, new CommandInfo("GET", "/session/{sessionId}/title"));
    this.TryAddCommand(DriverCommand.GetCurrentWindowHandle, new CommandInfo("GET", "/session/{sessionId}/window"));
    this.TryAddCommand(DriverCommand.Close, new CommandInfo("DELETE", "/session/{sessionId}/window"));
    this.TryAddCommand(DriverCommand.SwitchToWindow, new CommandInfo("POST", "/session/{sessionId}/window"));
    this.TryAddCommand(DriverCommand.GetWindowHandles, new CommandInfo("GET", "/session/{sessionId}/window/handles"));
    this.TryAddCommand(DriverCommand.SwitchToFrame, new CommandInfo("POST", "/session/{sessionId}/frame"));
    this.TryAddCommand(DriverCommand.SwitchToParentFrame, new CommandInfo("POST", "/session/{sessionId}/frame/parent"));
    this.TryAddCommand(DriverCommand.GetWindowSize, new CommandInfo("GET", "/session/{sessionId}/window/size"));
    this.TryAddCommand(DriverCommand.SetWindowSize, new CommandInfo("POST", "/session/{sessionId}/window/size"));
    this.TryAddCommand(DriverCommand.GetWindowPosition, new CommandInfo("GET", "/session/{sessionId}/window/position"));
    this.TryAddCommand(DriverCommand.SetWindowPosition, new CommandInfo("POST", "/session/{sessionId}/window/position"));
    this.TryAddCommand(DriverCommand.GetWindowRect, new CommandInfo("GET", "/session/{sessionId}/window/rect"));
    this.TryAddCommand(DriverCommand.SetWindowRect, new CommandInfo("POST", "/session/{sessionId}/window/rect"));
    this.TryAddCommand(DriverCommand.MaximizeWindow, new CommandInfo("POST", "/session/{sessionId}/window/maximize"));
    this.TryAddCommand(DriverCommand.MinimizeWindow, new CommandInfo("POST", "/session/{sessionId}/window/minimize"));
    this.TryAddCommand(DriverCommand.FullScreenWindow, new CommandInfo("POST", "/session/{sessionId}/window/fullscreen"));
    this.TryAddCommand(DriverCommand.GetActiveElement, new CommandInfo("GET", "/session/{sessionId}/element/active"));
    this.TryAddCommand(DriverCommand.FindElement, new CommandInfo("POST", "/session/{sessionId}/element"));
    this.TryAddCommand(DriverCommand.FindElements, new CommandInfo("POST", "/session/{sessionId}/elements"));
    this.TryAddCommand(DriverCommand.FindChildElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/element"));
    this.TryAddCommand(DriverCommand.FindChildElements, new CommandInfo("POST", "/session/{sessionId}/element/{id}/elements"));
    this.TryAddCommand(DriverCommand.IsElementSelected, new CommandInfo("GET", "/session/{sessionId}/element/{id}/selected"));
    this.TryAddCommand(DriverCommand.ClickElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/click"));
    this.TryAddCommand(DriverCommand.ClearElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/clear"));
    this.TryAddCommand(DriverCommand.SendKeysToElement, new CommandInfo("POST", "/session/{sessionId}/element/{id}/value"));
    this.TryAddCommand(DriverCommand.GetElementAttribute, new CommandInfo("GET", "/session/{sessionId}/element/{id}/attribute/{name}"));
    this.TryAddCommand(DriverCommand.GetElementProperty, new CommandInfo("GET", "/session/{sessionId}/element/{id}/property/{name}"));
    this.TryAddCommand(DriverCommand.GetElementValueOfCssProperty, new CommandInfo("GET", "/session/{sessionId}/element/{id}/css/{name}"));
    this.TryAddCommand(DriverCommand.GetElementText, new CommandInfo("GET", "/session/{sessionId}/element/{id}/text"));
    this.TryAddCommand(DriverCommand.GetElementTagName, new CommandInfo("GET", "/session/{sessionId}/element/{id}/name"));
    this.TryAddCommand(DriverCommand.GetElementRect, new CommandInfo("GET", "/session/{sessionId}/element/{id}/rect"));
    this.TryAddCommand(DriverCommand.IsElementEnabled, new CommandInfo("GET", "/session/{sessionId}/element/{id}/enabled"));
    this.TryAddCommand(DriverCommand.GetPageSource, new CommandInfo("GET", "/session/{sessionId}/source"));
    this.TryAddCommand(DriverCommand.ExecuteScript, new CommandInfo("POST", "/session/{sessionId}/execute/sync"));
    this.TryAddCommand(DriverCommand.ExecuteAsyncScript, new CommandInfo("POST", "/session/{sessionId}/execute/async"));
    this.TryAddCommand(DriverCommand.GetAllCookies, new CommandInfo("GET", "/session/{sessionId}/cookie"));
    this.TryAddCommand(DriverCommand.GetCookie, new CommandInfo("POST", "/session/{sessionId}/cookie/{name}"));
    this.TryAddCommand(DriverCommand.AddCookie, new CommandInfo("POST", "/session/{sessionId}/cookie"));
    this.TryAddCommand(DriverCommand.DeleteCookie, new CommandInfo("DELETE", "/session/{sessionId}/cookie/{name}"));
    this.TryAddCommand(DriverCommand.DeleteAllCookies, new CommandInfo("DELETE", "/session/{sessionId}/cookie"));
    this.TryAddCommand(DriverCommand.Actions, new CommandInfo("POST", "/session/{sessionId}/actions"));
    this.TryAddCommand(DriverCommand.CancelActions, new CommandInfo("DELETE", "/session/{sessionId}/actions"));
    this.TryAddCommand(DriverCommand.DismissAlert, new CommandInfo("POST", "/session/{sessionId}/alert/dismiss"));
    this.TryAddCommand(DriverCommand.AcceptAlert, new CommandInfo("POST", "/session/{sessionId}/alert/accept"));
    this.TryAddCommand(DriverCommand.GetAlertText, new CommandInfo("GET", "/session/{sessionId}/alert/text"));
    this.TryAddCommand(DriverCommand.SetAlertValue, new CommandInfo("POST", "/session/{sessionId}/alert/text"));
    this.TryAddCommand(DriverCommand.Screenshot, new CommandInfo("GET", "/session/{sessionId}/screenshot"));
    this.TryAddCommand(DriverCommand.ElementScreenshot, new CommandInfo("GET", "/session/{sessionId}/element/{id}/screenshot"));
    this.TryAddCommand(DriverCommand.GetSessionCapabilities, new CommandInfo("GET", "/session/{sessionId}"));
    this.TryAddCommand(DriverCommand.IsElementDisplayed, new CommandInfo("GET", "/session/{sessionId}/element/{id}/displayed"));
    this.TryAddCommand(DriverCommand.ElementEquals, new CommandInfo("GET", "/session/{sessionId}/element/{id}/equals/{other}"));
    this.TryAddCommand(DriverCommand.DefineDriverMapping, new CommandInfo("POST", "/config/drivers"));
    this.TryAddCommand(DriverCommand.UploadFile, new CommandInfo("POST", "/session/{sessionId}/file"));
    this.TryAddCommand(DriverCommand.SetAlertCredentials, new CommandInfo("POST", "/session/{sessionId}/alert/credentials"));
    this.TryAddCommand(DriverCommand.GetSessionList, new CommandInfo("GET", "/sessions"));
    this.TryAddCommand(DriverCommand.GetElementLocationOnceScrolledIntoView, new CommandInfo("GET", "/session/{sessionId}/element/{id}/location_in_view"));
    this.TryAddCommand(DriverCommand.DescribeElement, new CommandInfo("GET", "/session/{sessionId}/element/{id}"));
    this.TryAddCommand(DriverCommand.GetOrientation, new CommandInfo("GET", "/session/{sessionId}/orientation"));
    this.TryAddCommand(DriverCommand.SetOrientation, new CommandInfo("POST", "/session/{sessionId}/orientation"));
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
