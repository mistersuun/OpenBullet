# Newtonsoft.Json.dll Documentation

## Overview
Newtonsoft.Json (Json.NET) is the most popular JSON framework for .NET, providing high-performance JSON parsing, serialization, and manipulation. It's the de facto standard for JSON handling in .NET applications.

## Purpose in OpenBullet
- Parse API responses in JSON format
- Serialize/deserialize configuration files
- Extract data from JSON responses
- Transform and manipulate JSON data
- Handle complex nested JSON structures
- Convert between JSON and .NET objects

## Key Components

### Core Classes

#### `JsonConvert`
- **Purpose**: Static methods for quick JSON operations
- **Key Methods**:
  - `SerializeObject()` - Object to JSON string
  - `DeserializeObject()` - JSON string to object
  - `DeserializeObject<T>()` - JSON to typed object
  - `PopulateObject()` - Update existing object
- **Usage**:
```csharp
// Serialize
string json = JsonConvert.SerializeObject(data, Formatting.Indented);

// Deserialize
var result = JsonConvert.DeserializeObject<ResponseModel>(jsonString);

// With settings
var settings = new JsonSerializerSettings
{
    NullValueHandling = NullValueHandling.Ignore,
    DateFormatString = "yyyy-MM-dd"
};
var json = JsonConvert.SerializeObject(data, settings);
```

#### `JObject`
- **Purpose**: Dynamic JSON object manipulation
- **Key Features**:
  - Dynamic property access
  - LINQ to JSON queries
  - JSON path queries
  - Merge and combine JSON
- **Methods**:
  - `Parse()` - Parse JSON string
  - `SelectToken()` - Query with JSON path
  - `Add()` - Add properties
  - `Remove()` - Remove properties
  - `Merge()` - Merge JSON objects

#### `JArray`
- **Purpose**: JSON array manipulation
- **Key Methods**:
  - `Add()` - Add items
  - `Remove()` - Remove items
  - `Where()` - LINQ filtering
  - `Select()` - LINQ projection
- **Usage**:
```csharp
JArray array = JArray.Parse(jsonArray);
var filtered = array.Where(x => x["price"].Value<decimal>() > 100);
```

## Implementation Examples

### API Response Parsing
```csharp
public class ApiResponseParser
{
    public T ParseResponse<T>(string jsonResponse)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (JsonException ex)
        {
            throw new Exception($"Failed to parse response: {ex.Message}");
        }
    }
    
    public dynamic ParseDynamic(string jsonResponse)
    {
        return JObject.Parse(jsonResponse);
    }
    
    public Dictionary<string, object> ParseToDictionary(string jsonResponse)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
    }
}

// Usage with typed model
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public DataModel Data { get; set; }
    
    [JsonProperty("error_code")]
    public int? ErrorCode { get; set; }
    
    [JsonIgnore]
    public DateTime ParsedAt { get; set; } = DateTime.Now;
}
```

### Dynamic JSON Manipulation
```csharp
public class JsonDataExtractor
{
    private JObject jsonObject;
    
    public JsonDataExtractor(string json)
    {
        jsonObject = JObject.Parse(json);
    }
    
    public string ExtractValue(string path)
    {
        var token = jsonObject.SelectToken(path);
        return token?.ToString();
    }
    
    public List<string> ExtractArray(string path)
    {
        var array = jsonObject.SelectToken(path) as JArray;
        return array?.Select(x => x.ToString()).ToList() ?? new List<string>();
    }
    
    public Dictionary<string, string> ExtractObject(string path)
    {
        var obj = jsonObject.SelectToken(path) as JObject;
        return obj?.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
    }
    
    public bool CheckCondition(string path, string value)
    {
        var token = jsonObject.SelectToken(path);
        return token?.ToString() == value;
    }
}

// Usage
var extractor = new JsonDataExtractor(responseJson);
var userId = extractor.ExtractValue("user.id");
var permissions = extractor.ExtractArray("user.permissions");
var metadata = extractor.ExtractObject("user.metadata");
```

### JSON Path Queries
```csharp
public class JsonPathQueries
{
    public static void QueryExamples(string json)
    {
        var obj = JObject.Parse(json);
        
        // Basic path
        var name = obj.SelectToken("$.user.name");
        
        // Array index
        var firstItem = obj.SelectToken("$.items[0]");
        
        // Array slice
        var firstThree = obj.SelectTokens("$.items[0:3]");
        
        // Wildcard
        var allPrices = obj.SelectTokens("$.items[*].price");
        
        // Recursive descent
        var allIds = obj.SelectTokens("$..id");
        
        // Filter expression
        var expensive = obj.SelectTokens("$.items[?(@.price > 100)]");
        
        // Multiple conditions
        var filtered = obj.SelectTokens("$.items[?(@.price > 50 && @.inStock == true)]");
    }
}
```

### Custom Converters
```csharp
public class UnixTimestampConverter : JsonConverter<DateTime>
{
    public override DateTime ReadJson(JsonReader reader, Type objectType, 
        DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var timestamp = (long)reader.Value;
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }
    
    public override void WriteJson(JsonWriter writer, DateTime value, 
        JsonSerializer serializer)
    {
        var timestamp = new DateTimeOffset(value).ToUnixTimeSeconds();
        writer.WriteValue(timestamp);
    }
}

// Usage
var settings = new JsonSerializerSettings();
settings.Converters.Add(new UnixTimestampConverter());
var result = JsonConvert.DeserializeObject<Model>(json, settings);
```

### Configuration Management
```csharp
public class JsonConfigManager
{
    private readonly string configPath;
    private JObject config;
    
    public JsonConfigManager(string path)
    {
        configPath = path;
        LoadConfig();
    }
    
    private void LoadConfig()
    {
        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            config = JObject.Parse(json);
        }
        else
        {
            config = new JObject();
        }
    }
    
    public T GetValue<T>(string key, T defaultValue = default)
    {
        var token = config.SelectToken(key);
        return token != null ? token.ToObject<T>() : defaultValue;
    }
    
    public void SetValue(string key, object value)
    {
        var parts = key.Split('.');
        JToken current = config;
        
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (current[parts[i]] == null)
                current[parts[i]] = new JObject();
            current = current[parts[i]];
        }
        
        current[parts.Last()] = JToken.FromObject(value);
        SaveConfig();
    }
    
    private void SaveConfig()
    {
        File.WriteAllText(configPath, config.ToString(Formatting.Indented));
    }
}
```

### Response Validation
```csharp
public class JsonValidator
{
    public bool ValidateSchema(string json, string schemaJson)
    {
        var schema = JSchema.Parse(schemaJson);
        var data = JObject.Parse(json);
        
        return data.IsValid(schema, out IList<string> errors);
    }
    
    public bool ValidateStructure(string json, Dictionary<string, Type> expectedFields)
    {
        try
        {
            var obj = JObject.Parse(json);
            
            foreach (var field in expectedFields)
            {
                var token = obj.SelectToken(field.Key);
                if (token == null)
                    return false;
                    
                try
                {
                    token.ToObject(field.Value);
                }
                catch
                {
                    return false;
                }
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

## Advanced Features

### LINQ to JSON
```csharp
public class LinqToJson
{
    public void QueryExamples(string json)
    {
        var obj = JObject.Parse(json);
        
        // Filter and project
        var products = obj["products"]
            .Where(p => p["price"].Value<decimal>() > 50)
            .Select(p => new
            {
                Name = p["name"].Value<string>(),
                Price = p["price"].Value<decimal>(),
                InStock = p["quantity"].Value<int>() > 0
            });
        
        // Aggregation
        var totalPrice = obj["items"]
            .Sum(item => item["price"].Value<decimal>());
        
        // Grouping
        var grouped = obj["orders"]
            .GroupBy(o => o["status"].Value<string>())
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                Total = g.Sum(o => o["amount"].Value<decimal>())
            });
    }
}
```

### JSON Merge and Patch
```csharp
public class JsonMerge
{
    public JObject MergeJson(JObject original, JObject updates)
    {
        original.Merge(updates, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union,
            MergeNullValueHandling = MergeNullValueHandling.Ignore
        });
        
        return original;
    }
    
    public JObject ApplyJsonPatch(JObject document, JArray patchOperations)
    {
        foreach (var operation in patchOperations)
        {
            var op = operation["op"].Value<string>();
            var path = operation["path"].Value<string>();
            
            switch (op)
            {
                case "add":
                    var addValue = operation["value"];
                    document.SelectToken(path).Parent.Add(addValue);
                    break;
                    
                case "remove":
                    document.SelectToken(path).Remove();
                    break;
                    
                case "replace":
                    var replaceValue = operation["value"];
                    document.SelectToken(path).Replace(replaceValue);
                    break;
            }
        }
        
        return document;
    }
}
```

### Performance Optimization
```csharp
public class JsonPerformance
{
    // Use JsonTextReader for large files
    public void ProcessLargeJson(string filePath)
    {
        using (var file = File.OpenText(filePath))
        using (var reader = new JsonTextReader(file))
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && 
                    reader.Value.ToString() == "targetProperty")
                {
                    reader.Read();
                    ProcessValue(reader.Value);
                }
            }
        }
    }
    
    // Reuse JsonSerializer
    private static readonly JsonSerializer serializer = new JsonSerializer
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None
    };
    
    public T DeserializeEfficient<T>(string json)
    {
        using (var reader = new StringReader(json))
        using (var jsonReader = new JsonTextReader(reader))
        {
            return serializer.Deserialize<T>(jsonReader);
        }
    }
}
```

## Integration with OpenBullet

### JSON Response Analysis
```csharp
public class JsonKeycheck
{
    public BotStatus CheckResponse(string json, List<KeycheckRule> rules)
    {
        try
        {
            var obj = JObject.Parse(json);
            
            foreach (var rule in rules)
            {
                var value = obj.SelectToken(rule.JsonPath)?.ToString();
                
                switch (rule.Condition)
                {
                    case "equals":
                        if (value == rule.Value)
                            return rule.Status;
                        break;
                        
                    case "contains":
                        if (value?.Contains(rule.Value) == true)
                            return rule.Status;
                        break;
                        
                    case "exists":
                        if (value != null)
                            return rule.Status;
                        break;
                }
            }
            
            return BotStatus.Fail;
        }
        catch
        {
            return BotStatus.Error;
        }
    }
}
```

## Best Practices
1. Use strongly typed models when possible
2. Handle parsing exceptions properly
3. Validate JSON structure before processing
4. Use JsonTextReader for large files
5. Cache JsonSerializer instances
6. Use appropriate null handling settings
7. Consider using JSON Schema validation

## Common Patterns

### Safe Value Extraction
```csharp
public static class JsonExtensions
{
    public static T SafeGet<T>(this JObject obj, string path, T defaultValue = default)
    {
        try
        {
            var token = obj.SelectToken(path);
            return token != null ? token.ToObject<T>() : defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
}
```

### Nested Object Creation
```csharp
var json = new JObject
{
    ["user"] = new JObject
    {
        ["id"] = 123,
        ["name"] = "John",
        ["settings"] = new JObject
        {
            ["theme"] = "dark",
            ["notifications"] = true
        }
    }
};
```

## Performance Considerations
- Use JsonTextReader for streaming
- Avoid unnecessary conversions
- Reuse JsonSerializer instances
- Use StringBuilders for large concatenations
- Consider binary formats for large data