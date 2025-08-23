# Jint.dll Documentation

## Overview
Jint is a JavaScript interpreter for .NET, providing full ECMAScript 5.1 compliance. It allows execution of JavaScript code within .NET applications without requiring external JavaScript engines.

## Purpose in OpenBullet
- Execute JavaScript for data processing
- Evaluate JavaScript expressions
- Handle JavaScript-based challenges
- Process dynamic JavaScript responses
- Alternative scripting to Python/LoliScript

## Key Components

### Core Engine

#### `Engine`
- **Purpose**: JavaScript execution engine
- **Key Methods**:
  - `Execute()` - Execute JavaScript code
  - `SetValue()` - Set global variables
  - `GetValue()` - Get variable values
  - `Invoke()` - Call JavaScript functions
- **Configuration**:
```csharp
var engine = new Engine(cfg => cfg
    .AllowClr()  // Allow .NET interop
    .LimitRecursion(100)
    .TimeoutInterval(TimeSpan.FromSeconds(10))
    .MaxStatements(10000)
);
```

### JavaScript Execution

#### Basic Usage
```csharp
public class JavaScriptProcessor
{
    private Engine engine;
    
    public JavaScriptProcessor()
    {
        engine = new Engine();
        SetupEnvironment();
    }
    
    private void SetupEnvironment()
    {
        // Add custom functions
        engine.SetValue("log", new Action<object>(Console.WriteLine));
        engine.SetValue("btoa", new Func<string, string>(Base64Encode));
        engine.SetValue("atob", new Func<string, string>(Base64Decode));
    }
    
    public object ExecuteScript(string script, Dictionary<string, object> variables)
    {
        // Set variables
        foreach (var kvp in variables)
        {
            engine.SetValue(kvp.Key, kvp.Value);
        }
        
        // Execute
        var result = engine.Execute(script).GetCompletionValue();
        return result.ToObject();
    }
}
```

## Implementation Examples

### JavaScript Challenge Solver
```csharp
public class JsChallengesSolver
{
    public string SolveChallenge(string challengeScript)
    {
        var engine = new Engine();
        
        // Mock browser environment
        engine.Execute(@"
            var window = {};
            var document = {
                createElement: function() { return {}; },
                getElementById: function() { return {}; }
            };
            var navigator = {
                userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)'
            };
        ");
        
        // Execute challenge
        engine.Execute(challengeScript);
        
        // Get result (usually stored in specific variable)
        var result = engine.GetValue("challengeResult");
        return result.ToString();
    }
}
```

### Data Transformation
```csharp
public class DataTransformer
{
    private Engine engine;
    
    public string TransformData(string data, string transformScript)
    {
        engine = new Engine();
        engine.SetValue("input", data);
        
        engine.Execute(transformScript);
        
        return engine.GetValue("output").ToString();
    }
    
    // Example transform script
    string script = @"
        var parsed = JSON.parse(input);
        var transformed = {
            id: parsed.user_id,
            name: parsed.full_name.toUpperCase(),
            timestamp: Date.now()
        };
        var output = JSON.stringify(transformed);
    ";
}
```

### Complex Expression Evaluation
```csharp
public class ExpressionEvaluator
{
    public object EvaluateExpression(string expression, object context)
    {
        var engine = new Engine();
        engine.SetValue("ctx", context);
        
        return engine.Execute($"({expression})").GetCompletionValue().ToObject();
    }
    
    // Usage
    var result = EvaluateExpression(
        "ctx.price * ctx.quantity * (1 - ctx.discount)", 
        new { price = 10.5, quantity = 3, discount = 0.1 }
    );
}
```

## Integration with OpenBullet

### JavaScript Block
```csharp
public class JavaScriptBlock : BlockBase
{
    public string Script { get; set; }
    
    public override BlockResult Process(BotData data)
    {
        var engine = new Engine(cfg => cfg
            .AllowClr()
            .TimeoutInterval(TimeSpan.FromSeconds(5))
        );
        
        // Expose bot data
        engine.SetValue("variables", data.Variables);
        engine.SetValue("response", data.ResponseSource);
        engine.SetValue("cookies", data.Cookies);
        
        try
        {
            engine.Execute(Script);
            
            // Get modified variables back
            var vars = engine.GetValue("variables").ToObject() as Dictionary<string, object>;
            foreach (var kvp in vars)
            {
                data.Variables[kvp.Key] = kvp.Value?.ToString();
            }
            
            return BlockResult.Continue;
        }
        catch (JavaScriptException ex)
        {
            data.Log.Add($"JavaScript error: {ex.Message}");
            return BlockResult.Error;
        }
    }
}
```

### Anti-Bot Challenge Handler
```csharp
public class AntiBotHandler
{
    public string SolveJsChallenge(string html)
    {
        // Extract JavaScript challenge
        var match = Regex.Match(html, @"<script>(.*?)</script>", RegexOptions.Singleline);
        if (!match.Success) return null;
        
        var challengeScript = match.Groups[1].Value;
        
        var engine = new Engine();
        
        // Setup fake DOM
        engine.Execute(@"
            var document = {
                forms: { challenge: { submit: function() {} } },
                location: { href: '' }
            };
            var window = { location: document.location };
        ");
        
        // Execute challenge
        engine.Execute(challengeScript);
        
        // Extract answer
        var answer = engine.GetValue("answer")?.ToString() 
                  ?? engine.GetValue("jschl_answer")?.ToString();
                  
        return answer;
    }
}
```

## Advanced Features

### .NET Interoperability
```csharp
var engine = new Engine(cfg => cfg.AllowClr(typeof(HttpClient).Assembly));

engine.Execute(@"
    var HttpClient = importNamespace('System.Net.Http').HttpClient;
    var client = new HttpClient();
    var response = client.GetStringAsync('https://api.example.com').Result;
");
```

### Custom Objects
```csharp
engine.SetValue("api", new {
    get = new Func<string, string>(url => {
        using (var client = new WebClient())
            return client.DownloadString(url);
    }),
    post = new Func<string, string, string>((url, data) => {
        using (var client = new WebClient())
            return client.UploadString(url, data);
    })
});

engine.Execute("var data = api.get('https://api.example.com/data');");
```

## Performance Optimization
1. Reuse Engine instances when possible
2. Pre-compile frequently used scripts
3. Set appropriate timeout limits
4. Limit recursion depth
5. Use statement counters to prevent infinite loops

## Security Considerations
- Always set execution limits
- Sanitize user-provided scripts
- Disable CLR access for untrusted code
- Use sandboxed execution
- Monitor resource usage

## Best Practices
1. Handle JavaScript exceptions properly
2. Set appropriate timeouts
3. Mock browser APIs as needed
4. Validate script output
5. Use type conversion carefully
6. Implement proper error logging

## Limitations
- ECMAScript 5.1 (no ES6+ features)
- No DOM implementation
- Performance overhead vs native JavaScript
- Limited debugging capabilities

## Dependencies
- .NET Framework 4.5+ or .NET Core 2.0+
- No external dependencies