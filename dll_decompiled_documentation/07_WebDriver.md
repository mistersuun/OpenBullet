# WebDriver.dll Documentation

## Overview
WebDriver.dll is the core Selenium WebDriver library for .NET, providing browser automation capabilities. It enables programmatic control of web browsers for testing, scraping, and automation tasks that require JavaScript execution and full browser rendering.

## Purpose in OpenBullet
- Automate JavaScript-heavy websites
- Bypass anti-bot measures requiring real browser
- Handle complex authentication flows
- Solve CAPTCHAs with visual interaction
- Execute JavaScript for data extraction
- Handle dynamic content and AJAX

## Key Components

### Core WebDriver Classes

#### `IWebDriver` Interface
- **Purpose**: Main browser control interface
- **Implementations**:
  - `ChromeDriver` - Google Chrome
  - `FirefoxDriver` - Mozilla Firefox
  - `EdgeDriver` - Microsoft Edge
  - `SafariDriver` - Apple Safari
  - `InternetExplorerDriver` - IE (legacy)
- **Key Methods**:
  - `Navigate()` - Navigation control
  - `FindElement()` - Locate single element
  - `FindElements()` - Locate multiple elements
  - `Manage()` - Browser management
  - `SwitchTo()` - Context switching
  - `Quit()` - Close browser

#### `ChromeDriver`
- **Purpose**: Chrome browser automation
- **Configuration**:
```csharp
var options = new ChromeOptions();
options.AddArgument("--headless"); // Run without GUI
options.AddArgument("--disable-gpu");
options.AddArgument("--no-sandbox");
options.AddArgument("--disable-dev-shm-usage");
options.AddArgument($"--user-agent={userAgent}");
options.AddExtension("extension.crx"); // Add extension

var service = ChromeDriverService.CreateDefaultService();
service.HideCommandPromptWindow = true;

var driver = new ChromeDriver(service, options);
```

### Element Interaction

#### `IWebElement` Interface
- **Purpose**: Represents HTML element
- **Key Methods**:
  - `Click()` - Click element
  - `SendKeys()` - Type text
  - `Clear()` - Clear input
  - `Submit()` - Submit form
  - `GetAttribute()` - Get attribute value
  - `GetCssValue()` - Get CSS property
- **Properties**:
  - `Text` - Element text content
  - `TagName` - HTML tag name
  - `Enabled` - Is enabled
  - `Selected` - Is selected
  - `Displayed` - Is visible
  - `Location` - Position on page
  - `Size` - Element dimensions

#### Locator Strategies (`By` class)
```csharp
// Different ways to find elements
By.Id("element-id")
By.Name("element-name")
By.ClassName("class-name")
By.TagName("div")
By.LinkText("Click here")
By.PartialLinkText("Click")
By.CssSelector("div.class > span")
By.XPath("//div[@class='container']//span")
```

## Implementation Examples

### Basic Browser Automation
```csharp
public class SeleniumAutomation
{
    private IWebDriver driver;
    
    public void Initialize()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
        
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    public void Login(string username, string password)
    {
        driver.Navigate().GoToUrl("https://example.com/login");
        
        var userField = driver.FindElement(By.Id("username"));
        userField.Clear();
        userField.SendKeys(username);
        
        var passField = driver.FindElement(By.Id("password"));
        passField.Clear();
        passField.SendKeys(password);
        
        var loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
        loginButton.Click();
        
        // Wait for login to complete
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.FindElement(By.ClassName("dashboard")));
    }
    
    public void Cleanup()
    {
        driver?.Quit();
    }
}
```

### JavaScript Execution
```csharp
public class JavaScriptExecutor
{
    private IJavaScriptExecutor jsExecutor;
    
    public JavaScriptExecutor(IWebDriver driver)
    {
        jsExecutor = (IJavaScriptExecutor)driver;
    }
    
    public object ExecuteScript(string script, params object[] args)
    {
        return jsExecutor.ExecuteScript(script, args);
    }
    
    public void ScrollToElement(IWebElement element)
    {
        jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
    }
    
    public void ClickElement(IWebElement element)
    {
        // Use JS click when regular click doesn't work
        jsExecutor.ExecuteScript("arguments[0].click();", element);
    }
    
    public string GetPageSource()
    {
        return (string)jsExecutor.ExecuteScript("return document.documentElement.outerHTML;");
    }
    
    public void SetAttribute(IWebElement element, string attribute, string value)
    {
        jsExecutor.ExecuteScript(
            "arguments[0].setAttribute(arguments[1], arguments[2]);",
            element, attribute, value);
    }
}
```

### Wait Strategies
```csharp
public class WaitHelpers
{
    private IWebDriver driver;
    private WebDriverWait wait;
    
    public WaitHelpers(IWebDriver webDriver, int timeoutSeconds = 10)
    {
        driver = webDriver;
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
    }
    
    public IWebElement WaitForElement(By locator)
    {
        return wait.Until(ExpectedConditions.ElementIsVisible(locator));
    }
    
    public void WaitForPageLoad()
    {
        wait.Until(driver => ((IJavaScriptExecutor)driver)
            .ExecuteScript("return document.readyState").Equals("complete"));
    }
    
    public void WaitForAjax()
    {
        wait.Until(driver => (bool)((IJavaScriptExecutor)driver)
            .ExecuteScript("return jQuery.active == 0"));
    }
    
    public bool WaitForCondition(Func<IWebDriver, bool> condition)
    {
        try
        {
            return wait.Until(condition);
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
}
```

### CAPTCHA Handling
```csharp
public class CaptchaSolver
{
    private IWebDriver driver;
    
    public string SolveReCaptcha(string siteKey, string pageUrl)
    {
        // Wait for reCAPTCHA frame
        var frame = driver.FindElement(By.XPath("//iframe[contains(@src, 'recaptcha')]"));
        driver.SwitchTo().Frame(frame);
        
        // Click checkbox
        var checkbox = driver.FindElement(By.ClassName("recaptcha-checkbox"));
        checkbox.Click();
        
        // Switch back to main content
        driver.SwitchTo().DefaultContent();
        
        // Wait for challenge or success
        Thread.Sleep(2000);
        
        // Check if solved
        var response = (string)((IJavaScriptExecutor)driver)
            .ExecuteScript("return grecaptcha.getResponse();");
            
        if (!string.IsNullOrEmpty(response))
            return response;
            
        // If challenge appears, use external solver
        return SolveWithExternalService(siteKey, pageUrl);
    }
    
    public void InjectCaptchaSolution(string solution)
    {
        // Inject solution into page
        var js = $@"
            document.getElementById('g-recaptcha-response').innerHTML = '{solution}';
            document.getElementById('g-recaptcha-response').style.display = 'block';
            if (typeof ___grecaptcha_cfg !== 'undefined') {{
                Object.entries(___grecaptcha_cfg.clients).forEach(([key, client]) => {{
                    if (client.callback) {{
                        client.callback('{solution}');
                    }}
                }});
            }}
        ";
        
        ((IJavaScriptExecutor)driver).ExecuteScript(js);
    }
}
```

### Cookie Management
```csharp
public class CookieManager
{
    private IWebDriver driver;
    
    public void SaveCookies(string filePath)
    {
        var cookies = driver.Manage().Cookies.AllCookies;
        var json = JsonConvert.SerializeObject(cookies);
        File.WriteAllText(filePath, json);
    }
    
    public void LoadCookies(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var cookies = JsonConvert.DeserializeObject<List<Cookie>>(json);
        
        foreach (var cookie in cookies)
        {
            driver.Manage().Cookies.AddCookie(cookie);
        }
    }
    
    public void TransferSession(IWebDriver fromDriver, IWebDriver toDriver)
    {
        var cookies = fromDriver.Manage().Cookies.AllCookies;
        
        foreach (var cookie in cookies)
        {
            toDriver.Manage().Cookies.AddCookie(cookie);
        }
    }
}
```

### Proxy Configuration
```csharp
public class ProxyConfiguration
{
    public static ChromeOptions ConfigureProxy(string proxyAddress)
    {
        var options = new ChromeOptions();
        
        if (proxyAddress.Contains("@"))
        {
            // Proxy with authentication
            var parts = proxyAddress.Split('@');
            var auth = parts[0].Split(':');
            var proxy = parts[1];
            
            // Use extension for auth proxy
            options.AddExtension(CreateProxyAuthExtension(
                proxy, auth[0], auth[1]));
        }
        else
        {
            // Simple proxy
            options.AddArgument($"--proxy-server={proxyAddress}");
        }
        
        return options;
    }
    
    private static string CreateProxyAuthExtension(
        string proxy, string username, string password)
    {
        // Create Chrome extension for proxy authentication
        // Implementation creates manifest.json and background.js
        // Returns path to .crx file
        return "proxy_auth.crx";
    }
}
```

## Advanced Features

### Screenshot Capture
```csharp
public void CaptureScreenshot(string path)
{
    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
    screenshot.SaveAsFile(path, ScreenshotImageFormat.Png);
}

public void CaptureElementScreenshot(IWebElement element, string path)
{
    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
    var img = Image.FromStream(new MemoryStream(screenshot.AsByteArray));
    
    var cropArea = new Rectangle(
        element.Location.X,
        element.Location.Y,
        element.Size.Width,
        element.Size.Height);
    
    var bitmap = new Bitmap(cropArea.Width, cropArea.Height);
    using (var g = Graphics.FromImage(bitmap))
    {
        g.DrawImage(img, 0, 0, cropArea, GraphicsUnit.Pixel);
    }
    
    bitmap.Save(path);
}
```

### Multiple Windows/Tabs
```csharp
public class WindowManager
{
    private IWebDriver driver;
    
    public void OpenNewTab()
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
        driver.SwitchTo().Window(driver.WindowHandles.Last());
    }
    
    public void SwitchToWindow(string title)
    {
        foreach (var handle in driver.WindowHandles)
        {
            driver.SwitchTo().Window(handle);
            if (driver.Title == title)
                return;
        }
    }
    
    public void CloseCurrentTab()
    {
        driver.Close();
        driver.SwitchTo().Window(driver.WindowHandles.First());
    }
}
```

### Mobile Emulation
```csharp
var options = new ChromeOptions();
options.EnableMobileEmulation("iPhone X");

// Or custom device
var mobileEmulation = new Dictionary<string, object>
{
    ["deviceMetrics"] = new Dictionary<string, object>
    {
        ["width"] = 375,
        ["height"] = 812,
        ["pixelRatio"] = 3.0
    },
    ["userAgent"] = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X)"
};
options.AddAdditionalCapability("mobileEmulation", mobileEmulation);
```

## Integration with OpenBullet

### Selenium Block Implementation
```csharp
public class SeleniumBlock : BlockBase
{
    public SeleniumAction Action { get; set; }
    public string Target { get; set; }
    public string Value { get; set; }
    
    public override BlockResult Process(BotData data)
    {
        if (data.Driver == null)
        {
            InitializeDriver(data);
        }
        
        switch (Action)
        {
            case SeleniumAction.Navigate:
                data.Driver.Navigate().GoToUrl(ReplaceValues(Target, data));
                break;
                
            case SeleniumAction.Click:
                var element = FindElement(data.Driver, Target);
                element.Click();
                break;
                
            case SeleniumAction.Type:
                var input = FindElement(data.Driver, Target);
                input.SendKeys(ReplaceValues(Value, data));
                break;
                
            case SeleniumAction.GetText:
                var textElement = FindElement(data.Driver, Target);
                data.Variables[Value] = textElement.Text;
                break;
        }
        
        return BlockResult.Continue;
    }
}
```

## Performance Optimization
1. Use headless mode when possible
2. Disable images and CSS for speed
3. Reuse browser instances
4. Use explicit waits over implicit
5. Minimize JavaScript execution
6. Batch operations when possible

## Best Practices
1. Always use proper wait strategies
2. Handle StaleElementException
3. Clean up drivers in finally blocks
4. Use Page Object Model pattern
5. Implement proper error handling
6. Log browser console errors
7. Take screenshots on failure

## Limitations
- Resource intensive (CPU/RAM)
- Slower than HTTP requests
- Requires browser drivers
- Detection by anti-bot systems
- Limited mobile browser support
- No built-in proxy authentication

## Dependencies
- ChromeDriver.exe (for Chrome)
- GeckoDriver.exe (for Firefox)
- EdgeDriver.exe (for Edge)
- .NET Framework 4.5+