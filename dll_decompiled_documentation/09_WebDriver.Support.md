# WebDriver.Support.dll Documentation

## Overview
WebDriver.Support.dll provides additional support classes and utilities for Selenium WebDriver, including the Page Object Model pattern, wait conditions, and enhanced element selection capabilities.

## Purpose in OpenBullet
- Implement Page Object Model for complex sites
- Advanced wait conditions and synchronization
- Enhanced element selection and interaction
- Event-driven WebDriver operations
- Select element (dropdown) handling

## Key Components

### Wait Support

#### `WebDriverWait`
- **Purpose**: Explicit wait implementation
- **Key Methods**:
  - `Until()` - Wait for condition
  - `IgnoreExceptionTypes()` - Ignore specific exceptions
  - `Message` - Custom timeout message
- **Usage**:
```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
var element = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("submit")));
```

#### `ExpectedConditions`
- **Purpose**: Pre-built wait conditions
- **Common Conditions**:
```csharp
// Element presence
ExpectedConditions.ElementExists(By.Id("element"))
ExpectedConditions.ElementIsVisible(By.ClassName("visible"))

// Element state
ExpectedConditions.ElementToBeClickable(By.XPath("//button"))
ExpectedConditions.ElementToBeSelected(By.Name("checkbox"))

// Text conditions
ExpectedConditions.TextToBePresentInElement(element, "text")
ExpectedConditions.TextToBePresentInElementValue(By.Id("input"), "value")

// Window/Frame conditions
ExpectedConditions.FrameToBeAvailableAndSwitchToIt("frameName")
ExpectedConditions.NumberOfWindowsToBe(2)

// JavaScript conditions
ExpectedConditions.JsReturnsValue("return document.readyState === 'complete'")

// Alert conditions
ExpectedConditions.AlertIsPresent()
ExpectedConditions.AlertState(true)
```

### Page Object Model

#### `PageFactory`
- **Purpose**: Initialize page objects with element proxies
- **Methods**:
  - `InitElements()` - Initialize page elements
- **Attributes**:
  - `[FindsBy]` - Element locator attribute
  - `[CacheLookup]` - Cache element reference

```csharp
public class LoginPage
{
    private IWebDriver driver;
    
    [FindsBy(How = How.Id, Using = "username")]
    [CacheLookup]
    public IWebElement UsernameField { get; set; }
    
    [FindsBy(How = How.Id, Using = "password")]
    public IWebElement PasswordField { get; set; }
    
    [FindsBy(How = How.CssSelector, Using = "button[type='submit']")]
    public IWebElement LoginButton { get; set; }
    
    [FindsBy(How = How.ClassName, Using = "error-message")]
    public IList<IWebElement> ErrorMessages { get; set; }
    
    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
        PageFactory.InitElements(driver, this);
    }
    
    public void Login(string username, string password)
    {
        UsernameField.Clear();
        UsernameField.SendKeys(username);
        PasswordField.Clear();
        PasswordField.SendKeys(password);
        LoginButton.Click();
    }
    
    public bool HasErrors()
    {
        return ErrorMessages.Any();
    }
}
```

### UI Support

#### `SelectElement`
- **Purpose**: Handle HTML select dropdowns
- **Methods**:
  - `SelectByText()` - Select by visible text
  - `SelectByValue()` - Select by value attribute
  - `SelectByIndex()` - Select by index
  - `DeselectAll()` - Clear multi-select
  - `AllSelectedOptions` - Get selected options
  - `Options` - Get all options

```csharp
var dropdown = driver.FindElement(By.Id("country"));
var select = new SelectElement(dropdown);

// Different selection methods
select.SelectByText("United States");
select.SelectByValue("US");
select.SelectByIndex(0);

// Get selected option
var selected = select.SelectedOption.Text;

// Multi-select handling
select.SelectByText("Option 1");
select.SelectByText("Option 2");
var allSelected = select.AllSelectedOptions;
select.DeselectAll();
```

### Event-Driven WebDriver

#### `EventFiringWebDriver`
- **Purpose**: WebDriver with event hooks
- **Events**:
  - `Navigating` - Before navigation
  - `Navigated` - After navigation
  - `ElementClicking` - Before click
  - `ElementClicked` - After click
  - `ElementValueChanging` - Before value change
  - `ElementValueChanged` - After value change
  - `ExceptionThrown` - On exception
  - `ScriptExecuting` - Before script
  - `ScriptExecuted` - After script

```csharp
public class LoggingWebDriver
{
    private EventFiringWebDriver driver;
    
    public LoggingWebDriver(IWebDriver baseDriver)
    {
        driver = new EventFiringWebDriver(baseDriver);
        
        // Attach event handlers
        driver.Navigating += OnNavigating;
        driver.ElementClicking += OnElementClicking;
        driver.ExceptionThrown += OnException;
    }
    
    private void OnNavigating(object sender, WebDriverNavigationEventArgs e)
    {
        Console.WriteLine($"Navigating to: {e.Url}");
    }
    
    private void OnElementClicking(object sender, WebElementEventArgs e)
    {
        Console.WriteLine($"Clicking element: {e.Element.TagName}");
        TakeScreenshot($"before_click_{DateTime.Now.Ticks}.png");
    }
    
    private void OnException(object sender, WebDriverExceptionEventArgs e)
    {
        Console.WriteLine($"Exception: {e.ThrownException.Message}");
        TakeScreenshot($"error_{DateTime.Now.Ticks}.png");
    }
}
```

## Implementation Examples

### Advanced Wait Strategies
```csharp
public class SmartWaiter
{
    private WebDriverWait wait;
    
    public SmartWaiter(IWebDriver driver, int timeoutSeconds = 10)
    {
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.PollingInterval = TimeSpan.FromMilliseconds(500);
    }
    
    public IWebElement WaitForClickable(By locator)
    {
        return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
    }
    
    public void WaitForAjaxComplete()
    {
        wait.Until(driver => 
        {
            var js = (IJavaScriptExecutor)driver;
            return (bool)js.ExecuteScript("return jQuery.active == 0");
        });
    }
    
    public void WaitForAngular()
    {
        wait.Until(driver =>
        {
            var js = (IJavaScriptExecutor)driver;
            return (bool)js.ExecuteScript(
                "return window.getAllAngularTestabilities().every(t => t.isStable())");
        });
    }
    
    public void WaitForCustomCondition(Func<IWebDriver, bool> condition)
    {
        wait.Until(condition);
    }
}
```

### Complex Page Objects
```csharp
public class ProductListPage
{
    private IWebDriver driver;
    private WebDriverWait wait;
    
    [FindsBy(How = How.CssSelector, Using = ".product-item")]
    public IList<IWebElement> Products { get; set; }
    
    [FindsBy(How = How.Id, Using = "sort-dropdown")]
    public IWebElement SortDropdown { get; set; }
    
    [FindsBy(How = How.ClassName, Using = "filter-checkbox")]
    public IList<IWebElement> Filters { get; set; }
    
    public ProductListPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        PageFactory.InitElements(driver, this);
    }
    
    public void SortBy(string option)
    {
        var select = new SelectElement(SortDropdown);
        select.SelectByText(option);
        WaitForProductsToReload();
    }
    
    public void ApplyFilter(string filterName)
    {
        var filter = Filters.FirstOrDefault(f => 
            f.FindElement(By.TagName("label")).Text == filterName);
        filter?.Click();
        WaitForProductsToReload();
    }
    
    public List<ProductInfo> GetProducts()
    {
        return Products.Select(p => new ProductInfo
        {
            Name = p.FindElement(By.ClassName("product-name")).Text,
            Price = decimal.Parse(p.FindElement(By.ClassName("price")).Text.Replace("$", "")),
            InStock = p.FindElements(By.ClassName("out-of-stock")).Count == 0
        }).ToList();
    }
    
    private void WaitForProductsToReload()
    {
        wait.Until(ExpectedConditions.StalenessOf(Products.First()));
        wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(
            By.CssSelector(".product-item")));
    }
}
```

### Custom Expected Conditions
```csharp
public static class CustomExpectedConditions
{
    public static Func<IWebDriver, bool> ElementHasClass(By locator, string className)
    {
        return driver =>
        {
            try
            {
                var element = driver.FindElement(locator);
                var classes = element.GetAttribute("class");
                return classes != null && classes.Contains(className);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        };
    }
    
    public static Func<IWebDriver, bool> ElementTextChanges(IWebElement element, string currentText)
    {
        return driver =>
        {
            try
            {
                return element.Text != currentText;
            }
            catch (StaleElementReferenceException)
            {
                return true; // Element refreshed
            }
        };
    }
    
    public static Func<IWebDriver, bool> AjaxCallsComplete()
    {
        return driver =>
        {
            var js = (IJavaScriptExecutor)driver;
            var ajaxComplete = (bool)js.ExecuteScript(
                "return (typeof jQuery !== 'undefined') ? jQuery.active == 0 : true");
            var docReady = (bool)js.ExecuteScript(
                "return document.readyState == 'complete'");
            return ajaxComplete && docReady;
        };
    }
}

// Usage
wait.Until(CustomExpectedConditions.ElementHasClass(By.Id("status"), "success"));
wait.Until(CustomExpectedConditions.AjaxCallsComplete());
```

## Integration with OpenBullet

### Enhanced Selenium Block
```csharp
public class EnhancedSeleniumBlock : BlockBase
{
    private WebDriverWait wait;
    private EventFiringWebDriver eventDriver;
    
    public override BlockResult Process(BotData data)
    {
        if (data.Driver == null)
        {
            InitializeEnhancedDriver(data);
        }
        
        wait = new WebDriverWait(data.Driver, TimeSpan.FromSeconds(10));
        
        try
        {
            // Use PageFactory for complex pages
            var page = CreatePageObject(data.Driver, PageType);
            ExecutePageAction(page, data);
            
            return BlockResult.Continue;
        }
        catch (WebDriverTimeoutException)
        {
            data.Log.Add("Timeout waiting for element");
            return BlockResult.Retry;
        }
    }
    
    private void InitializeEnhancedDriver(BotData data)
    {
        var baseDriver = new ChromeDriver();
        eventDriver = new EventFiringWebDriver(baseDriver);
        
        eventDriver.ExceptionThrown += (s, e) =>
        {
            data.Log.Add($"WebDriver Exception: {e.ThrownException.Message}");
        };
        
        data.Driver = eventDriver;
    }
}
```

### Dropdown Automation
```csharp
public class SelectElementBlock : BlockBase
{
    public string ElementLocator { get; set; }
    public SelectionMethod Method { get; set; }
    public string Value { get; set; }
    
    public override BlockResult Process(BotData data)
    {
        var element = data.Driver.FindElement(By.XPath(ElementLocator));
        var select = new SelectElement(element);
        
        switch (Method)
        {
            case SelectionMethod.ByText:
                select.SelectByText(ReplaceValues(Value, data));
                break;
            case SelectionMethod.ByValue:
                select.SelectByValue(ReplaceValues(Value, data));
                break;
            case SelectionMethod.ByIndex:
                select.SelectByIndex(int.Parse(Value));
                break;
            case SelectionMethod.Random:
                var options = select.Options;
                var random = new Random();
                select.SelectByIndex(random.Next(options.Count));
                data.Variables["SELECTED"] = select.SelectedOption.Text;
                break;
        }
        
        return BlockResult.Continue;
    }
}
```

## Best Practices
1. Use PageFactory for maintainable page objects
2. Implement custom expected conditions for specific needs
3. Use EventFiringWebDriver for debugging
4. Cache elements when appropriate with [CacheLookup]
5. Handle StaleElementReferenceException properly
6. Use explicit waits over implicit waits
7. Organize page objects by functionality

## Performance Tips
1. Cache frequently accessed elements
2. Use CSS selectors over XPath when possible
3. Minimize DOM traversal
4. Batch element operations
5. Use JavaScript execution for bulk operations

## Limitations
- Page Factory doesn't support dynamic locators
- CacheLookup can cause stale element issues
- EventFiringWebDriver adds overhead
- Some expected conditions are browser-specific

## Dependencies
- WebDriver.dll (core Selenium)
- .NET Framework 4.5+
- Browser-specific drivers