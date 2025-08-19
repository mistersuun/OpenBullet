// Decompiled with JetBrains decompiler
// Type: RuriLib.SBlockBrowserAction
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Extreme.Net;
using Microsoft.CSharp.RuntimeBinder;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using RuriLib.LS;
using RuriLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class SBlockBrowserAction : BlockBase
{
  private BrowserAction action;
  private string input = "";

  public BrowserAction Action
  {
    get => this.action;
    set
    {
      this.action = value;
      this.OnPropertyChanged(nameof (Action));
    }
  }

  public string Input
  {
    get => this.input;
    set
    {
      this.input = value;
      this.OnPropertyChanged(nameof (Input));
    }
  }

  public SBlockBrowserAction() => this.Label = "BROWSER ACTION";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    // ISSUE: reference to a compiler-generated field
    if (SBlockBrowserAction.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      SBlockBrowserAction.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, BrowserAction>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (BrowserAction), typeof (SBlockBrowserAction)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Action = SBlockBrowserAction.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) SBlockBrowserAction.\u003C\u003Eo__9.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "ACTION", typeof (BrowserAction)));
    if (input != "")
      this.Input = LineParser.ParseLiteral(ref input, "INPUT");
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "BROWSERACTION").Token((object) this.Action).Literal(this.Input, "Input");
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.Driver == null && this.action != BrowserAction.Open)
    {
      data.Log(new LogEntry("Open a browser first!", Colors.White));
      throw new Exception("Browser not open");
    }
    string s = BlockBase.ReplaceValues(this.input, data);
    switch (this.action)
    {
      case BrowserAction.Open:
        SBlockBrowserAction.OpenBrowser(data);
        try
        {
          BlockBase.UpdateSeleniumData(data);
          break;
        }
        catch
        {
          break;
        }
      case BrowserAction.Close:
        data.Driver.Close();
        data.BrowserOpen = false;
        break;
      case BrowserAction.Quit:
        data.Driver.Quit();
        data.BrowserOpen = false;
        break;
      case BrowserAction.ClearCookies:
        data.Driver.Manage().Cookies.DeleteAllCookies();
        break;
      case BrowserAction.SendKeys:
        Actions actions = new Actions((IWebDriver) data.Driver);
        string str = s;
        string[] separator = new string[1]{ "||" };
        foreach (string keysToSend in str.Split(separator, StringSplitOptions.None))
        {
          switch (keysToSend)
          {
            case "<TAB>":
              actions.SendKeys(Keys.Tab);
              break;
            case "<ENTER>":
              actions.SendKeys(Keys.Enter);
              break;
            case "<BACKSPACE>":
              actions.SendKeys(Keys.Backspace);
              break;
            case "<ESC>":
              actions.SendKeys(Keys.Escape);
              break;
            default:
              actions.SendKeys(keysToSend);
              break;
          }
        }
        actions.Perform();
        Thread.Sleep(1000);
        if (s.Contains("<ENTER>") || s.Contains("<BACKSPACE>"))
        {
          BlockBase.UpdateSeleniumData(data);
          break;
        }
        break;
      case BrowserAction.Screenshot:
        RuriLib.Functions.Files.Files.SaveScreenshot(data.Driver.GetScreenshot(), data);
        break;
      case BrowserAction.SwitchToTab:
        data.Driver.SwitchTo().Window(data.Driver.WindowHandles[int.Parse(s)]);
        BlockBase.UpdateSeleniumData(data);
        break;
      case BrowserAction.Refresh:
        data.Driver.Navigate().Refresh();
        break;
      case BrowserAction.Back:
        data.Driver.Navigate().Back();
        break;
      case BrowserAction.Forward:
        data.Driver.Navigate().Forward();
        break;
      case BrowserAction.Maximize:
        data.Driver.Manage().Window.Maximize();
        break;
      case BrowserAction.Minimize:
        data.Driver.Manage().Window.Minimize();
        break;
      case BrowserAction.FullScreen:
        data.Driver.Manage().Window.FullScreen();
        break;
      case BrowserAction.SetWidth:
        data.Driver.Manage().Window.Size = new Size(int.Parse(s), data.Driver.Manage().Window.Size.Height);
        break;
      case BrowserAction.SetHeight:
        data.Driver.Manage().Window.Size = new Size(data.Driver.Manage().Window.Size.Width, int.Parse(s));
        break;
      case BrowserAction.DOMtoSOURCE:
        data.ResponseSource = data.Driver.FindElement(By.TagName("body")).GetAttribute("innerHTML");
        break;
      case BrowserAction.GetCookies:
        using (IEnumerator<Cookie> enumerator = data.Driver.Manage().Cookies.AllCookies.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Cookie current = enumerator.Current;
            try
            {
              data.Cookies.Add(current.Name, current.Value);
            }
            catch
            {
            }
          }
          break;
        }
      case BrowserAction.SetCookies:
        string domain = Regex.Match(BlockBase.ReplaceValues(this.input, data), "^(?:https?:\\/\\/)?(?:[^@\\/\n]+@)?([^:\\/?\n]+)").Groups[1].Value;
        using (Dictionary<string, string>.Enumerator enumerator = data.Cookies.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<string, string> current = enumerator.Current;
            try
            {
              data.Driver.Manage().Cookies.AddCookie(new Cookie(current.Key, current.Value, domain, "/", new DateTime?(DateTime.MaxValue)));
            }
            catch
            {
            }
          }
          break;
        }
      case BrowserAction.SwitchToDefault:
        data.Driver.SwitchTo().DefaultContent();
        break;
      case BrowserAction.SwitchToAlert:
        data.Driver.SwitchTo().Alert();
        break;
      case BrowserAction.SwitchToParentFrame:
        data.Driver.SwitchTo().ParentFrame();
        break;
    }
    data.Log(new LogEntry($"Executed browser action {this.action} on input {BlockBase.ReplaceValues(this.input, data)}", Colors.White));
  }

  public static void OpenBrowser(BotData data)
  {
    if (!data.BrowserOpen)
    {
      data.Log(new LogEntry("Opening browser...", Colors.White));
      switch (data.GlobalSettings.Selenium.Browser)
      {
        case BrowserType.Chrome:
          try
          {
            ChromeOptions options = new ChromeOptions();
            ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
            defaultService.SuppressInitialDiagnosticInformation = true;
            defaultService.HideCommandPromptWindow = true;
            defaultService.EnableVerboseLogging = false;
            options.AddArgument("--log-level=3");
            options.BinaryLocation = data.GlobalSettings.Selenium.ChromeBinaryLocation;
            if (data.GlobalSettings.Selenium.Headless || data.ConfigSettings.ForceHeadless)
              options.AddArgument("--headless");
            else if (data.GlobalSettings.Selenium.ChromeExtensions.Count > 0)
              options.AddExtensions(data.GlobalSettings.Selenium.ChromeExtensions.Where<string>((Func<string, bool>) (ext => ext.EndsWith(".crx"))).Select<string, string>((Func<string, string>) (ext => $"{Directory.GetCurrentDirectory()}\\ChromeExtensions\\{ext}")));
            if (data.ConfigSettings.DisableNotifications)
              options.AddArgument("--disable-notifications");
            if (data.ConfigSettings.CustomCMDArgs != "")
              options.AddArgument(data.ConfigSettings.CustomCMDArgs);
            if (data.ConfigSettings.RandomUA)
              options.AddArgument("--user-agent=" + BlockFunction.RandomUserAgent(data.Random));
            else if (data.ConfigSettings.CustomUserAgent != "")
              options.AddArgument("--user-agent=" + data.ConfigSettings.CustomUserAgent);
            if (data.UseProxies)
              options.AddArgument($"--proxy-server={data.Proxy.Type.ToString().ToLower()}://{data.Proxy.Proxy}");
            data.Driver = (RemoteWebDriver) new ChromeDriver(defaultService, options);
            break;
          }
          catch (Exception ex)
          {
            data.Log(new LogEntry(ex.ToString(), Colors.White));
            return;
          }
        case BrowserType.Firefox:
          try
          {
            FirefoxOptions options = new FirefoxOptions();
            FirefoxDriverService defaultService = FirefoxDriverService.CreateDefaultService();
            FirefoxProfile firefoxProfile = new FirefoxProfile();
            defaultService.SuppressInitialDiagnosticInformation = true;
            defaultService.HideCommandPromptWindow = true;
            options.AddArgument("--log-level=3");
            options.BrowserExecutableLocation = data.GlobalSettings.Selenium.FirefoxBinaryLocation;
            if (data.GlobalSettings.Selenium.Headless || data.ConfigSettings.ForceHeadless)
              options.AddArgument("--headless");
            if (data.ConfigSettings.DisableNotifications)
              firefoxProfile.SetPreference("dom.webnotifications.enabled", false);
            if (data.ConfigSettings.CustomCMDArgs != "")
              options.AddArgument(data.ConfigSettings.CustomCMDArgs);
            if (data.ConfigSettings.RandomUA)
              firefoxProfile.SetPreference("general.useragent.override", BlockFunction.RandomUserAgent(data.Random));
            else if (data.ConfigSettings.CustomUserAgent != "")
              firefoxProfile.SetPreference("general.useragent.override", data.ConfigSettings.CustomUserAgent);
            if (data.UseProxies)
            {
              firefoxProfile.SetPreference("network.proxy.type", 1);
              if (data.Proxy.Type == ProxyType.Http)
              {
                firefoxProfile.SetPreference("network.proxy.http", data.Proxy.Host);
                firefoxProfile.SetPreference("network.proxy.http_port", int.Parse(data.Proxy.Port));
                firefoxProfile.SetPreference("network.proxy.ssl", data.Proxy.Host);
                firefoxProfile.SetPreference("network.proxy.ssl_port", int.Parse(data.Proxy.Port));
              }
              else
              {
                firefoxProfile.SetPreference("network.proxy.socks", data.Proxy.Host);
                firefoxProfile.SetPreference("network.proxy.socks_port", int.Parse(data.Proxy.Port));
                if (data.Proxy.Type == ProxyType.Socks4)
                  firefoxProfile.SetPreference("network.proxy.socks_version", 4);
                else if (data.Proxy.Type == ProxyType.Socks5)
                  firefoxProfile.SetPreference("network.proxy.socks_version", 5);
              }
            }
            options.Profile = firefoxProfile;
            data.Driver = (RemoteWebDriver) new FirefoxDriver(defaultService, options, new TimeSpan(0, 1, 0));
            break;
          }
          catch (Exception ex)
          {
            data.Log(new LogEntry(ex.ToString(), Colors.White));
            return;
          }
      }
      data.Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds((double) data.GlobalSettings.Selenium.PageLoadTimeout);
      data.Log(new LogEntry("Opened!", Colors.White));
      data.BrowserOpen = true;
    }
    else
    {
      try
      {
        BlockBase.UpdateSeleniumData(data);
      }
      catch
      {
      }
    }
  }
}
