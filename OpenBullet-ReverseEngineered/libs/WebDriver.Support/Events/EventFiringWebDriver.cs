// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Events.EventFiringWebDriver
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

#nullable disable
namespace OpenQA.Selenium.Support.Events;

public class EventFiringWebDriver : 
  IWebDriver,
  ISearchContext,
  IDisposable,
  IJavaScriptExecutor,
  ITakesScreenshot,
  IWrapsDriver
{
  private IWebDriver driver;

  public EventFiringWebDriver(IWebDriver parentDriver) => this.driver = parentDriver;

  public event EventHandler<WebDriverNavigationEventArgs> Navigating;

  public event EventHandler<WebDriverNavigationEventArgs> Navigated;

  public event EventHandler<WebDriverNavigationEventArgs> NavigatingBack;

  public event EventHandler<WebDriverNavigationEventArgs> NavigatedBack;

  public event EventHandler<WebDriverNavigationEventArgs> NavigatingForward;

  public event EventHandler<WebDriverNavigationEventArgs> NavigatedForward;

  public event EventHandler<WebElementEventArgs> ElementClicking;

  public event EventHandler<WebElementEventArgs> ElementClicked;

  public event EventHandler<WebElementValueEventArgs> ElementValueChanging;

  public event EventHandler<WebElementValueEventArgs> ElementValueChanged;

  public event EventHandler<FindElementEventArgs> FindingElement;

  public event EventHandler<FindElementEventArgs> FindElementCompleted;

  public event EventHandler<WebDriverScriptEventArgs> ScriptExecuting;

  public event EventHandler<WebDriverScriptEventArgs> ScriptExecuted;

  public event EventHandler<WebDriverExceptionEventArgs> ExceptionThrown;

  public IWebDriver WrappedDriver => this.driver;

  public string Url
  {
    get
    {
      string empty = string.Empty;
      try
      {
        return this.driver.Url;
      }
      catch (Exception ex)
      {
        this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
        throw;
      }
    }
    set
    {
      try
      {
        WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.driver, value);
        this.OnNavigating(e);
        this.driver.Url = value;
        this.OnNavigated(e);
      }
      catch (Exception ex)
      {
        this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
        throw;
      }
    }
  }

  public string Title
  {
    get
    {
      string empty = string.Empty;
      try
      {
        return this.driver.Title;
      }
      catch (Exception ex)
      {
        this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
        throw;
      }
    }
  }

  public string PageSource
  {
    get
    {
      string empty = string.Empty;
      try
      {
        return this.driver.PageSource;
      }
      catch (Exception ex)
      {
        this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
        throw;
      }
    }
  }

  public string CurrentWindowHandle
  {
    get
    {
      string empty = string.Empty;
      try
      {
        return this.driver.CurrentWindowHandle;
      }
      catch (Exception ex)
      {
        this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
        throw;
      }
    }
  }

  public ReadOnlyCollection<string> WindowHandles
  {
    get
    {
      try
      {
        return this.driver.WindowHandles;
      }
      catch (Exception ex)
      {
        this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
        throw;
      }
    }
  }

  public void Close()
  {
    try
    {
      this.driver.Close();
    }
    catch (Exception ex)
    {
      this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
      throw;
    }
  }

  public void Quit()
  {
    try
    {
      this.driver.Quit();
    }
    catch (Exception ex)
    {
      this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
      throw;
    }
  }

  public IOptions Manage() => (IOptions) new EventFiringWebDriver.EventFiringOptions(this);

  public INavigation Navigate()
  {
    return (INavigation) new EventFiringWebDriver.EventFiringNavigation(this);
  }

  public ITargetLocator SwitchTo()
  {
    return (ITargetLocator) new EventFiringWebDriver.EventFiringTargetLocator(this);
  }

  public IWebElement FindElement(By by)
  {
    try
    {
      FindElementEventArgs e = new FindElementEventArgs(this.driver, by);
      this.OnFindingElement(e);
      IWebElement element = this.driver.FindElement(by);
      this.OnFindElementCompleted(e);
      return this.WrapElement(element);
    }
    catch (Exception ex)
    {
      this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
      throw;
    }
  }

  public ReadOnlyCollection<IWebElement> FindElements(By by)
  {
    List<IWebElement> webElementList = new List<IWebElement>();
    try
    {
      FindElementEventArgs e = new FindElementEventArgs(this.driver, by);
      this.OnFindingElement(e);
      ReadOnlyCollection<IWebElement> elements = this.driver.FindElements(by);
      this.OnFindElementCompleted(e);
      foreach (IWebElement underlyingElement in elements)
      {
        IWebElement webElement = this.WrapElement(underlyingElement);
        webElementList.Add(webElement);
      }
    }
    catch (Exception ex)
    {
      this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
      throw;
    }
    return webElementList.AsReadOnly();
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  public object ExecuteScript(string script, params object[] args)
  {
    if (!(this.driver is IJavaScriptExecutor driver))
      throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
    object obj;
    try
    {
      object[] objArray = EventFiringWebDriver.UnwrapElementArguments(args);
      WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.driver, script);
      this.OnScriptExecuting(e);
      obj = driver.ExecuteScript(script, objArray);
      this.OnScriptExecuted(e);
    }
    catch (Exception ex)
    {
      this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
      throw;
    }
    return obj;
  }

  public object ExecuteAsyncScript(string script, params object[] args)
  {
    if (!(this.driver is IJavaScriptExecutor driver))
      throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
    object obj;
    try
    {
      object[] objArray = EventFiringWebDriver.UnwrapElementArguments(args);
      WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.driver, script);
      this.OnScriptExecuting(e);
      obj = driver.ExecuteAsyncScript(script, objArray);
      this.OnScriptExecuted(e);
    }
    catch (Exception ex)
    {
      this.OnException(new WebDriverExceptionEventArgs(this.driver, ex));
      throw;
    }
    return obj;
  }

  public Screenshot GetScreenshot()
  {
    ITakesScreenshot driver = this.driver as ITakesScreenshot;
    if (this.driver == null)
      throw new NotSupportedException("Underlying driver instance does not support taking screenshots");
    return driver.GetScreenshot();
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing)
      return;
    this.driver.Dispose();
  }

  protected virtual void OnNavigating(WebDriverNavigationEventArgs e)
  {
    if (this.Navigating == null)
      return;
    this.Navigating((object) this, e);
  }

  protected virtual void OnNavigated(WebDriverNavigationEventArgs e)
  {
    if (this.Navigated == null)
      return;
    this.Navigated((object) this, e);
  }

  protected virtual void OnNavigatingBack(WebDriverNavigationEventArgs e)
  {
    if (this.NavigatingBack == null)
      return;
    this.NavigatingBack((object) this, e);
  }

  protected virtual void OnNavigatedBack(WebDriverNavigationEventArgs e)
  {
    if (this.NavigatedBack == null)
      return;
    this.NavigatedBack((object) this, e);
  }

  protected virtual void OnNavigatingForward(WebDriverNavigationEventArgs e)
  {
    if (this.NavigatingForward == null)
      return;
    this.NavigatingForward((object) this, e);
  }

  protected virtual void OnNavigatedForward(WebDriverNavigationEventArgs e)
  {
    if (this.NavigatedForward == null)
      return;
    this.NavigatedForward((object) this, e);
  }

  protected virtual void OnElementClicking(WebElementEventArgs e)
  {
    if (this.ElementClicking == null)
      return;
    this.ElementClicking((object) this, e);
  }

  protected virtual void OnElementClicked(WebElementEventArgs e)
  {
    if (this.ElementClicked == null)
      return;
    this.ElementClicked((object) this, e);
  }

  protected virtual void OnElementValueChanging(WebElementValueEventArgs e)
  {
    if (this.ElementValueChanging == null)
      return;
    this.ElementValueChanging((object) this, e);
  }

  protected virtual void OnElementValueChanged(WebElementValueEventArgs e)
  {
    if (this.ElementValueChanged == null)
      return;
    this.ElementValueChanged((object) this, e);
  }

  protected virtual void OnFindingElement(FindElementEventArgs e)
  {
    if (this.FindingElement == null)
      return;
    this.FindingElement((object) this, e);
  }

  protected virtual void OnFindElementCompleted(FindElementEventArgs e)
  {
    if (this.FindElementCompleted == null)
      return;
    this.FindElementCompleted((object) this, e);
  }

  protected virtual void OnScriptExecuting(WebDriverScriptEventArgs e)
  {
    if (this.ScriptExecuting == null)
      return;
    this.ScriptExecuting((object) this, e);
  }

  protected virtual void OnScriptExecuted(WebDriverScriptEventArgs e)
  {
    if (this.ScriptExecuted == null)
      return;
    this.ScriptExecuted((object) this, e);
  }

  protected virtual void OnException(WebDriverExceptionEventArgs e)
  {
    if (this.ExceptionThrown == null)
      return;
    this.ExceptionThrown((object) this, e);
  }

  private static object[] UnwrapElementArguments(object[] args)
  {
    List<object> objectList = new List<object>();
    foreach (object obj in args)
    {
      if (obj is IWrapsElement wrapsElement)
        objectList.Add((object) wrapsElement.WrappedElement);
      else
        objectList.Add(obj);
    }
    return objectList.ToArray();
  }

  private IWebElement WrapElement(IWebElement underlyingElement)
  {
    return (IWebElement) new EventFiringWebDriver.EventFiringWebElement(this, underlyingElement);
  }

  private class EventFiringNavigation : INavigation
  {
    private EventFiringWebDriver parentDriver;
    private INavigation wrappedNavigation;

    public EventFiringNavigation(EventFiringWebDriver driver)
    {
      this.parentDriver = driver;
      this.wrappedNavigation = this.parentDriver.WrappedDriver.Navigate();
    }

    public void Back()
    {
      try
      {
        WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs((IWebDriver) this.parentDriver);
        this.parentDriver.OnNavigatingBack(e);
        this.wrappedNavigation.Back();
        this.parentDriver.OnNavigatedBack(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void Forward()
    {
      try
      {
        WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs((IWebDriver) this.parentDriver);
        this.parentDriver.OnNavigatingForward(e);
        this.wrappedNavigation.Forward();
        this.parentDriver.OnNavigatedForward(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void GoToUrl(string url)
    {
      try
      {
        WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs((IWebDriver) this.parentDriver, url);
        this.parentDriver.OnNavigating(e);
        this.wrappedNavigation.GoToUrl(url);
        this.parentDriver.OnNavigated(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void GoToUrl(Uri url)
    {
      if (url == (Uri) null)
        throw new ArgumentNullException(nameof (url), "url cannot be null");
      try
      {
        WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs((IWebDriver) this.parentDriver, url.ToString());
        this.parentDriver.OnNavigating(e);
        this.wrappedNavigation.GoToUrl(url);
        this.parentDriver.OnNavigated(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void Refresh()
    {
      try
      {
        this.wrappedNavigation.Refresh();
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }
  }

  private class EventFiringOptions : IOptions
  {
    private IOptions wrappedOptions;

    public EventFiringOptions(EventFiringWebDriver driver)
    {
      this.wrappedOptions = driver.WrappedDriver.Manage();
    }

    public ICookieJar Cookies => this.wrappedOptions.Cookies;

    public IWindow Window => this.wrappedOptions.Window;

    public ILogs Logs => this.wrappedOptions.Logs;

    public ITimeouts Timeouts()
    {
      return (ITimeouts) new EventFiringWebDriver.EventFiringTimeouts(this.wrappedOptions);
    }
  }

  private class EventFiringTargetLocator : ITargetLocator
  {
    private ITargetLocator wrappedLocator;
    private EventFiringWebDriver parentDriver;

    public EventFiringTargetLocator(EventFiringWebDriver driver)
    {
      this.parentDriver = driver;
      this.wrappedLocator = this.parentDriver.WrappedDriver.SwitchTo();
    }

    public IWebDriver Frame(int frameIndex)
    {
      try
      {
        return this.wrappedLocator.Frame(frameIndex);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebDriver Frame(string frameName)
    {
      try
      {
        return this.wrappedLocator.Frame(frameName);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebDriver Frame(IWebElement frameElement)
    {
      try
      {
        return this.wrappedLocator.Frame((frameElement as IWrapsElement).WrappedElement);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebDriver ParentFrame()
    {
      try
      {
        return this.wrappedLocator.ParentFrame();
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebDriver Window(string windowName)
    {
      try
      {
        return this.wrappedLocator.Window(windowName);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebDriver DefaultContent()
    {
      try
      {
        return this.wrappedLocator.DefaultContent();
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebElement ActiveElement()
    {
      try
      {
        return this.wrappedLocator.ActiveElement();
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IAlert Alert()
    {
      try
      {
        return this.wrappedLocator.Alert();
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }
  }

  private class EventFiringTimeouts : ITimeouts
  {
    private ITimeouts wrappedTimeouts;

    public EventFiringTimeouts(IOptions options) => this.wrappedTimeouts = options.Timeouts();

    public TimeSpan ImplicitWait
    {
      get => this.wrappedTimeouts.ImplicitWait;
      set => this.wrappedTimeouts.ImplicitWait = value;
    }

    public TimeSpan AsynchronousJavaScript
    {
      get => this.wrappedTimeouts.AsynchronousJavaScript;
      set => this.wrappedTimeouts.AsynchronousJavaScript = value;
    }

    public TimeSpan PageLoad
    {
      get => this.wrappedTimeouts.PageLoad;
      set => this.wrappedTimeouts.PageLoad = value;
    }
  }

  private class EventFiringWebElement : IWebElement, ISearchContext, IWrapsElement
  {
    private IWebElement underlyingElement;
    private EventFiringWebDriver parentDriver;

    public EventFiringWebElement(EventFiringWebDriver driver, IWebElement element)
    {
      this.underlyingElement = element;
      this.parentDriver = driver;
    }

    public IWebElement WrappedElement => this.underlyingElement;

    public string TagName
    {
      get
      {
        string empty = string.Empty;
        try
        {
          return this.underlyingElement.TagName;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    public string Text
    {
      get
      {
        string empty = string.Empty;
        try
        {
          return this.underlyingElement.Text;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    public bool Enabled
    {
      get
      {
        try
        {
          return this.underlyingElement.Enabled;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    public bool Selected
    {
      get
      {
        try
        {
          return this.underlyingElement.Selected;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    public Point Location
    {
      get
      {
        Point point = new Point();
        try
        {
          return this.underlyingElement.Location;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    public Size Size
    {
      get
      {
        Size size = new Size();
        try
        {
          return this.underlyingElement.Size;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    public bool Displayed
    {
      get
      {
        try
        {
          return this.underlyingElement.Displayed;
        }
        catch (Exception ex)
        {
          this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
          throw;
        }
      }
    }

    protected EventFiringWebDriver ParentDriver => this.parentDriver;

    public void Clear()
    {
      try
      {
        WebElementValueEventArgs e = new WebElementValueEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement, (string) null);
        this.parentDriver.OnElementValueChanging(e);
        this.underlyingElement.Clear();
        this.parentDriver.OnElementValueChanged(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void SendKeys(string text)
    {
      try
      {
        WebElementValueEventArgs e = new WebElementValueEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement, text);
        this.parentDriver.OnElementValueChanging(e);
        this.underlyingElement.SendKeys(text);
        this.parentDriver.OnElementValueChanged(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void Submit()
    {
      try
      {
        this.underlyingElement.Submit();
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public void Click()
    {
      try
      {
        WebElementEventArgs e = new WebElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement);
        this.parentDriver.OnElementClicking(e);
        this.underlyingElement.Click();
        this.parentDriver.OnElementClicked(e);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public string GetAttribute(string attributeName)
    {
      string empty = string.Empty;
      try
      {
        return this.underlyingElement.GetAttribute(attributeName);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public string GetProperty(string propertyName)
    {
      string empty = string.Empty;
      try
      {
        return this.underlyingElement.GetProperty(propertyName);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public string GetCssValue(string propertyName)
    {
      string empty = string.Empty;
      try
      {
        return this.underlyingElement.GetCssValue(propertyName);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public IWebElement FindElement(By by)
    {
      try
      {
        FindElementEventArgs e = new FindElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement, by);
        this.parentDriver.OnFindingElement(e);
        IWebElement element = this.underlyingElement.FindElement(by);
        this.parentDriver.OnFindElementCompleted(e);
        return this.parentDriver.WrapElement(element);
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
    }

    public ReadOnlyCollection<IWebElement> FindElements(By by)
    {
      List<IWebElement> webElementList = new List<IWebElement>();
      try
      {
        FindElementEventArgs e = new FindElementEventArgs(this.parentDriver.WrappedDriver, this.underlyingElement, by);
        this.parentDriver.OnFindingElement(e);
        ReadOnlyCollection<IWebElement> elements = this.underlyingElement.FindElements(by);
        this.parentDriver.OnFindElementCompleted(e);
        foreach (IWebElement underlyingElement in elements)
        {
          IWebElement webElement = this.parentDriver.WrapElement(underlyingElement);
          webElementList.Add(webElement);
        }
      }
      catch (Exception ex)
      {
        this.parentDriver.OnException(new WebDriverExceptionEventArgs((IWebDriver) this.parentDriver, ex));
        throw;
      }
      return webElementList.AsReadOnly();
    }
  }
}
