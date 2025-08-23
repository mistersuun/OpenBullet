# IronPython.dll Documentation

## Overview
IronPython is a .NET implementation of the Python programming language, enabling Python scripting within .NET applications. It provides seamless integration between Python and .NET, allowing Python scripts to access .NET libraries and vice versa.

## Purpose in OpenBullet
- Execute Python scripts within automation workflows
- Provide alternative scripting language to LoliScript
- Enable complex data processing with Python libraries
- Support custom Python-based automation logic
- Bridge Python ecosystem with .NET automation

## Key Components

### Core Runtime

#### `PythonEngine`
- **Purpose**: Main Python runtime engine
- **Key Methods**:
  - `CreateEngine()` - Initialize Python engine
  - `CreateScope()` - Create execution scope
  - `Execute()` - Execute Python code
  - `ExecuteFile()` - Run Python script file
  - `SetSearchPaths()` - Configure module paths
- **Configuration**:
```csharp
var engine = Python.CreateEngine();
var scope = engine.CreateScope();
scope.SetVariable("data", botData);
engine.Execute("result = data.Variables['USER'].upper()", scope);
var result = scope.GetVariable("result");
```

#### `ScriptScope`
- **Purpose**: Isolated execution environment
- **Features**:
  - Variable isolation
  - Module imports
  - Global/local namespace management
- **Methods**:
  - `SetVariable()` - Pass .NET objects to Python
  - `GetVariable()` - Retrieve Python objects
  - `ContainsVariable()` - Check variable existence

### Python Integration

#### Type Conversion
```csharp
// .NET to Python
scope.SetVariable("numbers", new List<int> { 1, 2, 3 });
scope.SetVariable("text", "Hello Python");
scope.SetVariable("dict", new Dictionary<string, object>());

// Python to .NET
dynamic result = scope.GetVariable("result");
string text = result.ToString();
int number = (int)result;
List<object> list = result.ToList();
```

#### .NET Interop
```python
# Python code accessing .NET
import clr
clr.AddReference("System.Net.Http")
from System.Net.Http import HttpClient

client = HttpClient()
response = client.GetAsync("https://api.example.com").Result
content = response.Content.ReadAsStringAsync().Result
```

## Implementation Examples

### Basic Script Execution
```csharp
public class PythonScriptBlock : BlockBase
{
    public string Script { get; set; }
    
    public override BlockResult Process(BotData data)
    {
        var engine = Python.CreateEngine();
        var scope = engine.CreateScope();
        
        // Pass bot data to Python
        scope.SetVariable("data", data);
        scope.SetVariable("variables", data.Variables);
        
        try
        {
            engine.Execute(Script, scope);
            
            // Retrieve results
            if (scope.ContainsVariable("result"))
            {
                data.Variables["PYTHON_RESULT"] = scope.GetVariable("result").ToString();
            }
            
            return BlockResult.Continue;
        }
        catch (Exception ex)
        {
            data.Log.Add($"Python error: {ex.Message}");
            return BlockResult.Error;
        }
    }
}
```

### Data Processing
```python
# Python script for data processing
import json
import re
from datetime import datetime

def process_response(response_text):
    """Extract and process data from response"""
    data = json.loads(response_text)
    
    # Extract specific fields
    user_id = data.get('user', {}).get('id')
    timestamp = datetime.fromisoformat(data.get('timestamp'))
    
    # Apply transformations
    formatted_date = timestamp.strftime('%Y-%m-%d')
    
    # Pattern matching
    tokens = re.findall(r'token=([a-zA-Z0-9]+)', response_text)
    
    return {
        'user_id': user_id,
        'date': formatted_date,
        'tokens': tokens
    }

# Execute with bot data
result = process_response(variables['RESPONSE'])
variables['PROCESSED_DATA'] = json.dumps(result)
```

### Custom Functions
```csharp
// Register Python functions for use in OpenBullet
public class PythonFunctions
{
    private ScriptEngine engine;
    private ScriptScope scope;
    
    public PythonFunctions()
    {
        engine = Python.CreateEngine();
        scope = engine.CreateScope();
        
        // Load Python functions
        engine.ExecuteFile("functions.py", scope);
    }
    
    public string ExecuteFunction(string funcName, params object[] args)
    {
        var func = scope.GetVariable(funcName);
        var result = engine.Operations.Invoke(func, args);
        return result?.ToString();
    }
}
```

### Advanced Parsing
```python
# Complex HTML parsing with BeautifulSoup (if available)
from bs4 import BeautifulSoup
import re

def parse_product_page(html):
    soup = BeautifulSoup(html, 'html.parser')
    
    products = []
    for item in soup.find_all('div', class_='product'):
        product = {
            'name': item.find('h2').text.strip(),
            'price': float(re.findall(r'[\d.]+', item.find('span', class_='price').text)[0]),
            'availability': 'in-stock' in item.get('class', []),
            'image': item.find('img')['src']
        }
        products.append(product)
    
    return products

# Use in bot
products = parse_product_page(variables['HTML_RESPONSE'])
variables['PRODUCTS'] = json.dumps(products)
```

## Module System

### Standard Library Access
```python
# Available Python standard library modules
import os
import sys
import json
import re
import datetime
import hashlib
import base64
import urllib
import random
import math
```

### Custom Module Loading
```csharp
// Configure module search paths
var paths = engine.GetSearchPaths();
paths.Add(@"C:\Python\Lib");
paths.Add(@"C:\CustomModules");
engine.SetSearchPaths(paths);
```

### Creating Custom Modules
```python
# custom_module.py
def encode_data(data, key):
    """Custom encoding function"""
    import hashlib
    combined = f"{data}:{key}"
    return hashlib.sha256(combined.encode()).hexdigest()

def decode_response(encoded):
    """Custom decoding function"""
    import base64
    return base64.b64decode(encoded).decode('utf-8')

# Utility functions
def extract_csrf_token(html):
    import re
    match = re.search(r'csrf_token["\']:\s*["\'](.*?)["\']', html)
    return match.group(1) if match else None
```

## Integration with OpenBullet

### Python Block Configuration
```csharp
public class PythonBlockConfig
{
    public string ScriptPath { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<string> RequiredModules { get; set; }
    public bool PassFullBotData { get; set; }
    
    public void Validate()
    {
        // Check required modules
        foreach (var module in RequiredModules)
        {
            if (!CheckModuleAvailable(module))
                throw new Exception($"Required module not available: {module}");
        }
    }
}
```

### Error Handling
```python
# Python error handling
try:
    # Risky operation
    result = process_data(variables['INPUT'])
    variables['OUTPUT'] = result
    status = 'SUCCESS'
except ValueError as e:
    variables['ERROR'] = str(e)
    status = 'RETRY'
except Exception as e:
    variables['ERROR'] = f"Unexpected error: {e}"
    status = 'FAIL'
finally:
    variables['STATUS'] = status
```

### Performance Optimization
```python
# Caching compiled regex patterns
import re

# Cache patterns at module level
PATTERNS = {
    'email': re.compile(r'^[\w\.-]+@[\w\.-]+\.\w+$'),
    'token': re.compile(r'token=([a-zA-Z0-9]+)'),
    'price': re.compile(r'\$?([\d,]+\.?\d*)')
}

def validate_email(email):
    return bool(PATTERNS['email'].match(email))

def extract_tokens(text):
    return PATTERNS['token'].findall(text)
```

## Advanced Features

### Async Operations
```python
# Async support in IronPython
import clr
clr.AddReference("System.Threading.Tasks")
from System.Threading.Tasks import Task

def async_operation():
    def work():
        import time
        time.sleep(1)
        return "Complete"
    
    task = Task.Run(work)
    return task.Result
```

### Dynamic Code Generation
```python
# Generate code dynamically
def create_parser(pattern):
    code = f"""
def parse(text):
    import re
    matches = re.findall(r'{pattern}', text)
    return matches
"""
    exec(code, globals())
    return parse

# Use generated function
parser = create_parser(r'\d{4}-\d{2}-\d{2}')
dates = parser(variables['TEXT'])
```

## Best Practices

### Memory Management
```python
# Clean up large objects
def process_large_data(data):
    result = analyze(data)
    del data  # Explicitly delete large object
    import gc
    gc.collect()  # Force garbage collection
    return result
```

### Security Considerations
```python
# Sanitize inputs
def safe_execute(user_code):
    # Restrict dangerous operations
    restricted_globals = {
        '__builtins__': {
            'len': len,
            'str': str,
            'int': int,
            # Limited set of safe functions
        }
    }
    
    exec(user_code, restricted_globals, {})
```

### Performance Tips
1. Pre-compile regular expressions
2. Cache frequently used calculations
3. Use list comprehensions over loops
4. Minimize .NET/Python boundary crossings
5. Batch operations when possible

## Limitations
- Python 2.7 syntax (not Python 3)
- Limited C extension support
- Some standard library modules unavailable
- Performance overhead compared to CPython
- Threading limitations with GIL

## Dependencies
- .NET Framework 4.5+
- Microsoft.Scripting.dll
- Microsoft.Dynamic.dll
- IronPython.Modules.dll (for standard library)

## Common Use Cases

### Data Validation
```python
def validate_account(username, email):
    errors = []
    
    if len(username) < 3:
        errors.append("Username too short")
    
    if '@' not in email:
        errors.append("Invalid email format")
    
    return len(errors) == 0, errors
```

### Response Parsing
```python
def parse_api_response(json_text):
    import json
    
    data = json.loads(json_text)
    
    return {
        'success': data.get('status') == 'ok',
        'data': data.get('result', {}),
        'error': data.get('error_message')
    }
```

### Cryptographic Operations
```python
import hashlib
import hmac

def generate_signature(data, secret):
    return hmac.new(
        secret.encode(),
        data.encode(),
        hashlib.sha256
    ).hexdigest()
```