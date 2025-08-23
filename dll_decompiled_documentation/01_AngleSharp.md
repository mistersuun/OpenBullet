# AngleSharp.dll Documentation

## Overview
AngleSharp is a .NET library that provides a powerful HTML5 parser and DOM implementation. It allows parsing and manipulating HTML/XML documents without requiring a full browser engine.

## Purpose in OpenBullet
- Parse HTML responses from HTTP requests
- Extract data using CSS selectors and DOM navigation
- Validate response content structure
- Handle malformed HTML gracefully

## Key Components

### Core Classes

#### `BrowsingContext`
- **Purpose**: Main context for document parsing and management
- **Key Methods**:
  - `OpenAsync()` - Opens and parses HTML content
  - `GetService<T>()` - Retrieves services like cookie management
- **Implementation Example**:
```csharp
var config = Configuration.Default;
var context = BrowsingContext.New(config);
var document = await context.OpenAsync(response => response.Content(htmlString));
```

#### `Configuration`
- **Purpose**: Configures parsing behavior and available services
- **Key Properties**:
  - `Default` - Standard HTML5 configuration
  - `WithCss()` - Adds CSS parsing support
  - `WithJs()` - Adds JavaScript engine support
- **Usage**: Customizes parser behavior for specific requirements

#### `IDocument` Interface
- **Purpose**: Represents parsed HTML/XML document
- **Key Methods**:
  - `QuerySelector()` - Find single element by CSS selector
  - `QuerySelectorAll()` - Find all matching elements
  - `GetElementById()` - Direct ID-based element access
  - `GetElementsByClassName()` - Class-based element selection

### Parsing Capabilities

#### HTML5 Compliance
- Full HTML5 specification support
- Handles malformed HTML with error recovery
- Supports HTML fragments and full documents

#### CSS Selector Engine
- Complete CSS3 selector support
- Pseudo-classes and pseudo-elements
- Attribute selectors with operators
- Complex combinators

### URL Handling

#### `Url` Class
- **Purpose**: Parse and manipulate URLs
- **Key Features**:
  - Absolute/relative URL resolution
  - Query parameter manipulation
  - Fragment handling
  - Protocol and host parsing
- **Methods**:
  - `Parse()` - Create URL from string
  - `IsRelative` - Check if URL is relative
  - `Href` - Get complete URL string

## Integration with OpenBullet

### Response Parsing
```csharp
// Parse HTTP response HTML
var document = await context.OpenAsync(req => req.Content(httpResponse.Body));

// Extract specific data
var loginForm = document.QuerySelector("form#login");
var csrfToken = document.QuerySelector("input[name='csrf']")?.GetAttribute("value");
```

### Data Extraction
```csharp
// Extract multiple values
var prices = document.QuerySelectorAll(".price")
    .Select(e => e.TextContent.Trim())
    .ToList();

// Navigate DOM tree
var table = document.QuerySelector("table.results");
var rows = table?.QuerySelectorAll("tr").Skip(1); // Skip header
```

### Form Analysis
```csharp
// Analyze form structure
var form = document.QuerySelector("form");
var action = form?.GetAttribute("action");
var method = form?.GetAttribute("method");
var inputs = form?.QuerySelectorAll("input")
    .Select(i => new {
        Name = i.GetAttribute("name"),
        Type = i.GetAttribute("type"),
        Value = i.GetAttribute("value")
    });
```

## Dependencies
- .NET Framework 4.5+ or .NET Core 2.0+
- No external dependencies for core functionality
- Optional CSS/JavaScript engines for extended features

## Performance Considerations
- Lightweight compared to full browser engines
- Fast parsing with streaming support
- Memory efficient for large documents
- Thread-safe when properly configured

## Common Use Cases in Bots

### 1. Login Form Detection
```csharp
var hasLoginForm = document.QuerySelector("form[action*='login']") != null;
var usernameField = document.QuerySelector("input[type='email'], input[name*='user']");
```

### 2. CAPTCHA Detection
```csharp
var hasCaptcha = document.QuerySelector(".g-recaptcha, #recaptcha, iframe[src*='captcha']") != null;
```

### 3. Success/Failure Detection
```csharp
var successMessage = document.QuerySelector(".success-message, .alert-success");
var errorMessage = document.QuerySelector(".error, .alert-danger");
```

### 4. Data Scraping
```csharp
var productData = document.QuerySelectorAll(".product").Select(p => new {
    Title = p.QuerySelector(".title")?.TextContent,
    Price = p.QuerySelector(".price")?.TextContent,
    Link = p.QuerySelector("a")?.GetAttribute("href")
});
```

## Best Practices
1. Always dispose of contexts when done
2. Use CSS selectors for maintainable code
3. Handle null results from queries
4. Cache parsed documents when reusing
5. Configure only needed features for performance

## Limitations
- No JavaScript execution (unless JS engine added)
- No rendering engine (layout/styling)
- Limited AJAX/dynamic content support
- Requires separate HTTP client for requests

## Security Considerations
- Sanitize user-provided selectors
- Validate extracted data before use
- Be aware of XSS when rendering extracted content
- Limit document size to prevent DoS

## Version Information
- Current Version: Varies by OpenBullet version
- .NET Compatibility: Framework 4.5+, Core 2.0+, Standard 2.0+
- License: MIT License