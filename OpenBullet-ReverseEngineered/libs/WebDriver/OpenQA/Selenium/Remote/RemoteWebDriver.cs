// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteWebDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteWebDriver : 
  IWebDriver,
  ISearchContext,
  IDisposable,
  IJavaScriptExecutor,
  IFindsById,
  IFindsByClassName,
  IFindsByLinkText,
  IFindsByName,
  IFindsByTagName,
  IFindsByXPath,
  IFindsByPartialLinkText,
  IFindsByCssSelector,
  ITakesScreenshot,
  IHasInputDevices,
  IHasCapabilities,
  IHasWebStorage,
  IHasLocationContext,
  IHasApplicationCache,
  IAllowsFileDetection,
  IHasSessionId,
  IActionExecutor
{
  protected static readonly TimeSpan DefaultCommandTimeout = TimeSpan.FromSeconds(60.0);
  private const string DefaultRemoteServerUrl = "http://127.0.0.1:4444/wd/hub";
  private ICommandExecutor executor;
  private ICapabilities capabilities;
  private IMouse mouse;
  private IKeyboard keyboard;
  private SessionId sessionId;
  private IWebStorage storage;
  private IApplicationCache appCache;
  private ILocationContext locationContext;
  private IFileDetector fileDetector = (IFileDetector) new DefaultFileDetector();
  private RemoteWebElementFactory elementFactory;

  public RemoteWebDriver(DriverOptions options)
    : this(RemoteWebDriver.ConvertOptionsToCapabilities(options))
  {
  }

  public RemoteWebDriver(ICapabilities desiredCapabilities)
    : this(new Uri("http://127.0.0.1:4444/wd/hub"), desiredCapabilities)
  {
  }

  public RemoteWebDriver(Uri remoteAddress, DriverOptions options)
    : this(remoteAddress, RemoteWebDriver.ConvertOptionsToCapabilities(options))
  {
  }

  public RemoteWebDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
    : this(remoteAddress, desiredCapabilities, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public RemoteWebDriver(
    Uri remoteAddress,
    ICapabilities desiredCapabilities,
    TimeSpan commandTimeout)
    : this((ICommandExecutor) new HttpCommandExecutor(remoteAddress, commandTimeout), desiredCapabilities)
  {
  }

  public RemoteWebDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
  {
    this.executor = commandExecutor;
    this.StartClient();
    this.StartSession(desiredCapabilities);
    this.mouse = (IMouse) new RemoteMouse(this);
    this.keyboard = (IKeyboard) new RemoteKeyboard(this);
    this.elementFactory = new RemoteWebElementFactory(this);
    if (this.capabilities.HasCapability(CapabilityType.SupportsApplicationCache) && this.capabilities.GetCapability(CapabilityType.SupportsApplicationCache) is bool capability1 && capability1)
      this.appCache = (IApplicationCache) new RemoteApplicationCache(this);
    if (this.capabilities.HasCapability(CapabilityType.SupportsLocationContext) && this.capabilities.GetCapability(CapabilityType.SupportsLocationContext) is bool capability2 && capability2)
      this.locationContext = (ILocationContext) new RemoteLocationContext(this);
    if (!this.capabilities.HasCapability(CapabilityType.SupportsWebStorage) || !(this.capabilities.GetCapability(CapabilityType.SupportsWebStorage) is bool capability3) || !capability3)
      return;
    this.storage = (IWebStorage) new RemoteWebStorage(this);
  }

  public string Url
  {
    get
    {
      return this.Execute(DriverCommand.GetCurrentUrl, (Dictionary<string, object>) null).Value.ToString();
    }
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value), "Argument 'url' cannot be null.");
      this.Execute(DriverCommand.Get, new Dictionary<string, object>()
      {
        {
          "url",
          (object) value
        }
      });
    }
  }

  public string Title
  {
    get
    {
      Response response = this.Execute(DriverCommand.GetTitle, (Dictionary<string, object>) null);
      return (response != null ? response.Value : (object) string.Empty).ToString();
    }
  }

  public string PageSource
  {
    get
    {
      string empty = string.Empty;
      return this.Execute(DriverCommand.GetPageSource, (Dictionary<string, object>) null).Value.ToString();
    }
  }

  public string CurrentWindowHandle
  {
    get
    {
      return this.Execute(DriverCommand.GetCurrentWindowHandle, (Dictionary<string, object>) null).Value.ToString();
    }
  }

  public ReadOnlyCollection<string> WindowHandles
  {
    get
    {
      object[] objArray = (object[]) this.Execute(DriverCommand.GetWindowHandles, (Dictionary<string, object>) null).Value;
      List<string> stringList = new List<string>();
      foreach (object obj in objArray)
        stringList.Add(obj.ToString());
      return stringList.AsReadOnly();
    }
  }

  [Obsolete("This property was never intended to be used in user code. Use the Actions or ActionBuilder class to send direct keyboard input.")]
  public IKeyboard Keyboard => this.keyboard;

  [Obsolete("This property was never intended to be used in user code. Use the Actions or ActionBuilder class to send direct mouse input.")]
  public IMouse Mouse => this.mouse;

  public bool HasWebStorage => this.storage != null;

  public IWebStorage WebStorage
  {
    get
    {
      return this.storage != null ? this.storage : throw new InvalidOperationException("Driver does not support manipulating HTML5 web storage. Use the HasWebStorage property to test for the driver capability");
    }
  }

  public bool HasApplicationCache => this.appCache != null;

  public IApplicationCache ApplicationCache
  {
    get
    {
      return this.appCache != null ? this.appCache : throw new InvalidOperationException("Driver does not support manipulating the HTML5 application cache. Use the HasApplicationCache property to test for the driver capability");
    }
  }

  public bool HasLocationContext => this.locationContext != null;

  public ILocationContext LocationContext
  {
    get
    {
      return this.locationContext != null ? this.locationContext : throw new InvalidOperationException("Driver does not support setting HTML5 geolocation information. Use the HasLocationContext property to test for the driver capability");
    }
  }

  public ICapabilities Capabilities => this.capabilities;

  public virtual IFileDetector FileDetector
  {
    get => this.fileDetector;
    set
    {
      this.fileDetector = value != null ? value : throw new ArgumentNullException(nameof (value), "FileDetector cannot be null");
    }
  }

  public SessionId SessionId => this.sessionId;

  public bool IsActionExecutor => this.IsSpecificationCompliant;

  internal bool IsSpecificationCompliant
  {
    get => this.CommandExecutor.CommandInfoRepository.SpecificationLevel > 0;
  }

  protected ICommandExecutor CommandExecutor => this.executor;

  protected RemoteWebElementFactory ElementFactory
  {
    get => this.elementFactory;
    set => this.elementFactory = value;
  }

  public IWebElement FindElement(By by)
  {
    return !(by == (By) null) ? by.FindElement((ISearchContext) this) : throw new ArgumentNullException(nameof (by), "by cannot be null");
  }

  public ReadOnlyCollection<IWebElement> FindElements(By by)
  {
    return !(by == (By) null) ? by.FindElements((ISearchContext) this) : throw new ArgumentNullException(nameof (by), "by cannot be null");
  }

  public void Close() => this.Execute(DriverCommand.Close, (Dictionary<string, object>) null);

  public void Quit() => this.Dispose();

  public IOptions Manage() => (IOptions) new RemoteOptions(this);

  public INavigation Navigate() => (INavigation) new RemoteNavigator(this);

  public ITargetLocator SwitchTo() => (ITargetLocator) new RemoteTargetLocator(this);

  public object ExecuteScript(string script, params object[] args)
  {
    return this.ExecuteScriptCommand(script, DriverCommand.ExecuteScript, args);
  }

  public object ExecuteAsyncScript(string script, params object[] args)
  {
    return this.ExecuteScriptCommand(script, DriverCommand.ExecuteAsyncScript, args);
  }

  public IWebElement FindElementById(string id)
  {
    return this.IsSpecificationCompliant ? this.FindElement("css selector", "#" + RemoteWebDriver.EscapeCssSelector(id)) : this.FindElement(nameof (id), id);
  }

  public ReadOnlyCollection<IWebElement> FindElementsById(string id)
  {
    if (!this.IsSpecificationCompliant)
      return this.FindElements(nameof (id), id);
    string str = RemoteWebDriver.EscapeCssSelector(id);
    return string.IsNullOrEmpty(str) ? new List<IWebElement>().AsReadOnly() : this.FindElements("css selector", "#" + str);
  }

  public IWebElement FindElementByClassName(string className)
  {
    if (!this.IsSpecificationCompliant)
      return this.FindElement("class name", className);
    string str = RemoteWebDriver.EscapeCssSelector(className);
    return !str.Contains(" ") ? this.FindElement("css selector", "." + str) : throw new InvalidSelectorException("Compound class names not allowed. Cannot have whitespace in class name. Use CSS selectors instead.");
  }

  public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
  {
    if (!this.IsSpecificationCompliant)
      return this.FindElements("class name", className);
    string str = RemoteWebDriver.EscapeCssSelector(className);
    return !str.Contains(" ") ? this.FindElements("css selector", "." + str) : throw new InvalidSelectorException("Compound class names not allowed. Cannot have whitespace in class name. Use CSS selectors instead.");
  }

  public IWebElement FindElementByLinkText(string linkText)
  {
    return this.FindElement("link text", linkText);
  }

  public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
  {
    return this.FindElements("link text", linkText);
  }

  public IWebElement FindElementByPartialLinkText(string partialLinkText)
  {
    return this.FindElement("partial link text", partialLinkText);
  }

  public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
  {
    return this.FindElements("partial link text", partialLinkText);
  }

  public IWebElement FindElementByName(string name)
  {
    return this.IsSpecificationCompliant ? this.FindElement("css selector", $"*[name=\"{name}\"]") : this.FindElement(nameof (name), name);
  }

  public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
  {
    return this.IsSpecificationCompliant ? this.FindElements("css selector", $"*[name=\"{name}\"]") : this.FindElements(nameof (name), name);
  }

  public IWebElement FindElementByTagName(string tagName)
  {
    return this.IsSpecificationCompliant ? this.FindElement("css selector", tagName) : this.FindElement("tag name", tagName);
  }

  public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
  {
    return this.IsSpecificationCompliant ? this.FindElements("css selector", tagName) : this.FindElements("tag name", tagName);
  }

  public IWebElement FindElementByXPath(string xpath) => this.FindElement(nameof (xpath), xpath);

  public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
  {
    return this.FindElements(nameof (xpath), xpath);
  }

  public IWebElement FindElementByCssSelector(string cssSelector)
  {
    return this.FindElement("css selector", cssSelector);
  }

  public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
  {
    return this.FindElements("css selector", cssSelector);
  }

  public Screenshot GetScreenshot()
  {
    return new Screenshot(this.Execute(DriverCommand.Screenshot, (Dictionary<string, object>) null).Value.ToString());
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  public void PerformActions(IList<ActionSequence> actionSequenceList)
  {
    if (actionSequenceList == null)
      throw new ArgumentNullException(nameof (actionSequenceList), "List of action sequences must not be null");
    if (!this.IsSpecificationCompliant)
      return;
    List<object> objectList = new List<object>();
    foreach (ActionSequence actionSequence in (IEnumerable<ActionSequence>) actionSequenceList)
      objectList.Add((object) actionSequence.ToDictionary());
    this.Execute(DriverCommand.Actions, new Dictionary<string, object>()
    {
      ["actions"] = (object) objectList
    });
  }

  public void ResetInputState()
  {
    if (!this.IsSpecificationCompliant)
      return;
    this.Execute(DriverCommand.CancelActions, (Dictionary<string, object>) null);
  }

  internal static string EscapeCssSelector(string selector)
  {
    string str = Regex.Replace(selector, "([ '\"\\\\#.:;,!?+<>=~*^$|%&@`{}\\-/\\[\\]\\(\\)])", "\\$1");
    if (selector.Length > 0 && char.IsDigit(selector[0]))
      str = $"\\{(30 + int.Parse(selector.Substring(0, 1), (IFormatProvider) CultureInfo.InvariantCulture)).ToString((IFormatProvider) CultureInfo.InvariantCulture)} {selector.Substring(1)}";
    return str;
  }

  internal Response InternalExecute(
    string driverCommandToExecute,
    Dictionary<string, object> parameters)
  {
    return this.Execute(driverCommandToExecute, parameters);
  }

  internal IWebElement GetElementFromResponse(Response response)
  {
    if (response == null)
      throw new NoSuchElementException();
    RemoteWebElement elementFromResponse = (RemoteWebElement) null;
    if (response.Value is Dictionary<string, object> elementDictionary)
      elementFromResponse = this.elementFactory.CreateElement(elementDictionary);
    return (IWebElement) elementFromResponse;
  }

  internal ReadOnlyCollection<IWebElement> GetElementsFromResponse(Response response)
  {
    List<IWebElement> webElementList = new List<IWebElement>();
    if (response.Value is object[] objArray)
    {
      for (int index = 0; index < objArray.Length; ++index)
      {
        if (objArray[index] is Dictionary<string, object> elementDictionary)
        {
          RemoteWebElement element = this.elementFactory.CreateElement(elementDictionary);
          webElementList.Add((IWebElement) element);
        }
      }
    }
    return webElementList.AsReadOnly();
  }

  protected virtual void Dispose(bool disposing)
  {
    try
    {
      this.Execute(DriverCommand.Quit, (Dictionary<string, object>) null);
    }
    catch (NotImplementedException ex)
    {
    }
    catch (InvalidOperationException ex)
    {
    }
    catch (WebDriverException ex)
    {
    }
    finally
    {
      this.StopClient();
      this.sessionId = (SessionId) null;
    }
  }

  protected void StartSession(ICapabilities desiredCapabilities)
  {
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    if (!(desiredCapabilities is RemoteSessionSettings remoteSessionSettings))
    {
      parameters.Add(nameof (desiredCapabilities), (object) this.GetLegacyCapabilitiesDictionary(desiredCapabilities));
      Dictionary<string, object> capabilitiesDictionary = this.GetCapabilitiesDictionary(desiredCapabilities);
      parameters.Add("capabilities", (object) new Dictionary<string, object>()
      {
        ["firstMatch"] = (object) new List<object>()
        {
          (object) capabilitiesDictionary
        }
      });
    }
    else
      parameters.Add("capabilities", (object) remoteSessionSettings.ToDictionary());
    Response response = this.Execute(DriverCommand.NewSession, parameters);
    this.capabilities = (ICapabilities) new ReturnedCapabilities((Dictionary<string, object>) response.Value);
    this.sessionId = new SessionId(response.SessionId);
  }

  protected virtual Dictionary<string, object> GetLegacyCapabilitiesDictionary(
    ICapabilities legacyCapabilities)
  {
    Dictionary<string, object> capabilitiesDictionary = new Dictionary<string, object>();
    foreach (KeyValuePair<string, object> capabilities in (legacyCapabilities as IHasCapabilitiesDictionary).CapabilitiesDictionary)
      capabilitiesDictionary.Add(capabilities.Key, capabilities.Value);
    return capabilitiesDictionary;
  }

  protected virtual Dictionary<string, object> GetCapabilitiesDictionary(
    ICapabilities capabilitiesToConvert)
  {
    Dictionary<string, object> capabilitiesDictionary = new Dictionary<string, object>();
    foreach (KeyValuePair<string, object> capabilities in (capabilitiesToConvert as IHasCapabilitiesDictionary).CapabilitiesDictionary)
    {
      if (CapabilityType.IsSpecCompliantCapabilityName(capabilities.Key))
        capabilitiesDictionary.Add(capabilities.Key, capabilities.Value);
    }
    return capabilitiesDictionary;
  }

  protected virtual Response Execute(
    string driverCommandToExecute,
    Dictionary<string, object> parameters)
  {
    Command commandToExecute = new Command(this.sessionId, driverCommandToExecute, parameters);
    Response errorResponse = new Response();
    try
    {
      errorResponse = this.executor.Execute(commandToExecute);
    }
    catch (WebException ex)
    {
      errorResponse.Status = WebDriverResult.UnhandledError;
      errorResponse.Value = (object) ex;
    }
    if (errorResponse.Status != WebDriverResult.Success)
      RemoteWebDriver.UnpackAndThrowOnError(errorResponse);
    return errorResponse;
  }

  protected virtual void StartClient()
  {
  }

  protected virtual void StopClient()
  {
  }

  protected IWebElement FindElement(string mechanism, string value)
  {
    return this.GetElementFromResponse(this.Execute(DriverCommand.FindElement, new Dictionary<string, object>()
    {
      {
        "using",
        (object) mechanism
      },
      {
        nameof (value),
        (object) value
      }
    }));
  }

  protected ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value)
  {
    return this.GetElementsFromResponse(this.Execute(DriverCommand.FindElements, new Dictionary<string, object>()
    {
      {
        "using",
        (object) mechanism
      },
      {
        nameof (value),
        (object) value
      }
    }));
  }

  protected object ExecuteScriptCommand(string script, string commandName, params object[] args)
  {
    object[] javaScriptObjects = RemoteWebDriver.ConvertArgumentsToJavaScriptObjects(args);
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    parameters.Add(nameof (script), (object) script);
    if (javaScriptObjects != null && javaScriptObjects.Length != 0)
      parameters.Add(nameof (args), (object) javaScriptObjects);
    else
      parameters.Add(nameof (args), (object) new object[0]);
    return this.ParseJavaScriptReturnValue(this.Execute(commandName, parameters).Value);
  }

  private static object ConvertObjectToJavaScriptObject(object arg)
  {
    IWrapsElement wrapsElement = arg as IWrapsElement;
    IWebElementReference elementReference = arg as IWebElementReference;
    IEnumerable enumerable = arg as IEnumerable;
    IDictionary dictionary1 = arg as IDictionary;
    if (elementReference == null && wrapsElement != null)
      elementReference = wrapsElement.WrappedElement as IWebElementReference;
    switch (arg)
    {
      case string _:
      case float _:
      case double _:
      case int _:
      case long _:
      case bool _:
      case null:
        return arg;
      default:
        if (elementReference != null)
        {
          Dictionary<string, object> dictionary2 = elementReference.ToDictionary();
          dictionary2.Add("ELEMENT", (object) elementReference.ElementReferenceId);
          return (object) dictionary2;
        }
        if (dictionary1 != null)
        {
          Dictionary<string, object> javaScriptObject = new Dictionary<string, object>();
          foreach (object key in (IEnumerable) dictionary1.Keys)
            javaScriptObject.Add(key.ToString(), RemoteWebDriver.ConvertObjectToJavaScriptObject(dictionary1[key]));
          return (object) javaScriptObject;
        }
        if (enumerable == null)
          throw new ArgumentException("Argument is of an illegal type" + arg.ToString(), nameof (arg));
        List<object> objectList = new List<object>();
        foreach (object obj in enumerable)
          objectList.Add(RemoteWebDriver.ConvertObjectToJavaScriptObject(obj));
        return (object) objectList.ToArray();
    }
  }

  private static object[] ConvertArgumentsToJavaScriptObjects(object[] args)
  {
    if (args == null)
      return new object[1];
    for (int index = 0; index < args.Length; ++index)
      args[index] = RemoteWebDriver.ConvertObjectToJavaScriptObject(args[index]);
    return args;
  }

  private static void UnpackAndThrowOnError(Response errorResponse)
  {
    if (errorResponse.Status == WebDriverResult.Success)
      return;
    if (!(errorResponse.Value is Dictionary<string, object> responseValue))
      throw new WebDriverException("Unexpected error. " + errorResponse.Value.ToString());
    string message = new ErrorResponse(responseValue).Message;
    switch (errorResponse.Status)
    {
      case WebDriverResult.NoSuchDriver:
        throw new WebDriverException(message);
      case WebDriverResult.NoSuchElement:
        throw new NoSuchElementException(message);
      case WebDriverResult.NoSuchFrame:
        throw new NoSuchFrameException(message);
      case WebDriverResult.UnknownCommand:
        throw new NotImplementedException(message);
      case WebDriverResult.ObsoleteElement:
        throw new StaleElementReferenceException(message);
      case WebDriverResult.ElementNotDisplayed:
        throw new ElementNotVisibleException(message);
      case WebDriverResult.InvalidElementState:
      case WebDriverResult.ElementNotSelectable:
        throw new InvalidElementStateException(message);
      case WebDriverResult.UnhandledError:
        throw new WebDriverException(message);
      case WebDriverResult.NoSuchDocument:
        throw new NoSuchElementException(message);
      case WebDriverResult.UnexpectedJavaScriptError:
        throw new WebDriverException(message);
      case WebDriverResult.Timeout:
        throw new WebDriverTimeoutException(message);
      case WebDriverResult.NoSuchWindow:
        throw new NoSuchWindowException(message);
      case WebDriverResult.InvalidCookieDomain:
        throw new InvalidCookieDomainException(message);
      case WebDriverResult.UnableToSetCookie:
        throw new UnableToSetCookieException(message);
      case WebDriverResult.UnexpectedAlertOpen:
        string empty = string.Empty;
        if (responseValue.ContainsKey("alert"))
        {
          if (responseValue["alert"] is Dictionary<string, object> dictionary1 && dictionary1.ContainsKey("text"))
            empty = dictionary1["text"].ToString();
        }
        else if (responseValue.ContainsKey("data") && responseValue["data"] is Dictionary<string, object> dictionary2 && dictionary2.ContainsKey("text"))
          empty = dictionary2["text"].ToString();
        throw new UnhandledAlertException(message, empty);
      case WebDriverResult.NoAlertPresent:
        throw new NoAlertPresentException(message);
      case WebDriverResult.AsyncScriptTimeout:
        throw new WebDriverTimeoutException(message);
      case WebDriverResult.InvalidSelector:
        throw new InvalidSelectorException(message);
      case WebDriverResult.MoveTargetOutOfBounds:
        throw new WebDriverException(message);
      case WebDriverResult.ElementNotInteractable:
        throw new ElementNotInteractableException(message);
      case WebDriverResult.InvalidArgument:
        throw new WebDriverException(message);
      case WebDriverResult.ElementClickIntercepted:
        throw new ElementClickInterceptedException(message);
      default:
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) message, (object) errorResponse.Status));
    }
  }

  private static ICapabilities ConvertOptionsToCapabilities(DriverOptions options)
  {
    return options != null ? options.ToCapabilities() : throw new ArgumentNullException(nameof (options), "Driver options must not be null");
  }

  private object ParseJavaScriptReturnValue(object responseValue)
  {
    Dictionary<string, object> elementDictionary = responseValue as Dictionary<string, object>;
    object[] objArray = responseValue as object[];
    object scriptReturnValue1;
    if (elementDictionary != null)
    {
      if (this.elementFactory.ContainsElementReference(elementDictionary))
      {
        scriptReturnValue1 = (object) this.elementFactory.CreateElement(elementDictionary);
      }
      else
      {
        string[] array = new string[elementDictionary.Keys.Count];
        elementDictionary.Keys.CopyTo(array, 0);
        foreach (string key in array)
          elementDictionary[key] = this.ParseJavaScriptReturnValue(elementDictionary[key]);
        scriptReturnValue1 = (object) elementDictionary;
      }
    }
    else if (objArray != null)
    {
      bool flag = true;
      List<object> objectList = new List<object>();
      foreach (object responseValue1 in objArray)
      {
        object scriptReturnValue2 = this.ParseJavaScriptReturnValue(responseValue1);
        if (!(scriptReturnValue2 is IWebElement))
          flag = false;
        objectList.Add(scriptReturnValue2);
      }
      if (objectList.Count > 0 & flag)
      {
        List<IWebElement> webElementList = new List<IWebElement>();
        foreach (object obj in objectList)
        {
          IWebElement webElement = obj as IWebElement;
          webElementList.Add(webElement);
        }
        scriptReturnValue1 = (object) webElementList.AsReadOnly();
      }
      else
        scriptReturnValue1 = (object) objectList.AsReadOnly();
    }
    else
      scriptReturnValue1 = responseValue;
    return scriptReturnValue1;
  }
}
