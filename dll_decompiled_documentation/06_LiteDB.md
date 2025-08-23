# LiteDB.dll Documentation

## Overview
LiteDB is a lightweight, serverless, embedded NoSQL database written in C#. It provides a simple API similar to MongoDB, storing data in BSON format within a single file, making it perfect for desktop applications and embedded scenarios.

## Purpose in OpenBullet
- Store bot configurations and settings
- Manage wordlists and combo lists
- Cache proxy lists and validation results
- Store hit results and statistics
- Maintain session data and cookies
- Log bot execution history

## Key Components

### Core Database Classes

#### `LiteDatabase`
- **Purpose**: Main database connection and operations
- **Key Methods**:
  - `GetCollection<T>()` - Access collection
  - `BeginTrans()` - Start transaction
  - `Commit()` - Commit transaction
  - `Rollback()` - Rollback changes
  - `Checkpoint()` - Flush data to disk
  - `Shrink()` - Reduce file size
- **Connection String**:
```csharp
// File mode
var db = new LiteDatabase(@"C:\Data\OpenBullet.db");

// Connection string with options
var db = new LiteDatabase("Filename=data.db;Password=secret;Mode=Exclusive");

// In-memory database
var db = new LiteDatabase(":memory:");
```

#### `LiteCollection<T>`
- **Purpose**: Collection of documents (like MongoDB collection)
- **Key Methods**:
  - `Insert()` - Add new document
  - `Update()` - Update existing document
  - `Delete()` - Remove document
  - `Find()` - Query documents
  - `FindById()` - Get by ID
  - `FindOne()` - Get single document
  - `Count()` - Count documents
  - `EnsureIndex()` - Create index

### Data Models

#### Document Structure
```csharp
public class HitData
{
    [BsonId]
    public int Id { get; set; }
    
    public string Combo { get; set; }
    public string Proxy { get; set; }
    public DateTime Timestamp { get; set; }
    public string Config { get; set; }
    public Dictionary<string, string> CapturedData { get; set; }
    
    [BsonIgnore]
    public string TempData { get; set; } // Not stored
}

public class ProxyData
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    public string Address { get; set; }
    public int Port { get; set; }
    public ProxyType Type { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime LastChecked { get; set; }
    public bool IsWorking { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
}
```

## Implementation Examples

### Database Initialization
```csharp
public class DatabaseManager
{
    private LiteDatabase db;
    
    public DatabaseManager(string dbPath)
    {
        db = new LiteDatabase(dbPath);
        
        // Create indexes for better performance
        var hits = db.GetCollection<HitData>("hits");
        hits.EnsureIndex(x => x.Combo);
        hits.EnsureIndex(x => x.Timestamp);
        
        var proxies = db.GetCollection<ProxyData>("proxies");
        proxies.EnsureIndex(x => x.Address);
        proxies.EnsureIndex(x => x.IsWorking);
    }
    
    public void Dispose()
    {
        db?.Dispose();
    }
}
```

### Storing Bot Results
```csharp
public class ResultStorage
{
    private readonly ILiteCollection<HitData> collection;
    
    public ResultStorage(LiteDatabase db)
    {
        collection = db.GetCollection<HitData>("hits");
    }
    
    public void SaveHit(BotData data)
    {
        var hit = new HitData
        {
            Combo = $"{data.Variables["USER"]}:{data.Variables["PASS"]}",
            Proxy = data.Proxy?.ToString(),
            Timestamp = DateTime.UtcNow,
            Config = data.ConfigName,
            CapturedData = data.Variables
                .Where(v => v.Key.StartsWith("CAP_"))
                .ToDictionary(v => v.Key, v => v.Value.ToString())
        };
        
        collection.Insert(hit);
    }
    
    public List<HitData> GetRecentHits(int count = 100)
    {
        return collection.Query()
            .OrderByDescending(x => x.Timestamp)
            .Limit(count)
            .ToList();
    }
    
    public List<HitData> SearchHits(string keyword)
    {
        return collection.Find(x => x.Combo.Contains(keyword))
            .ToList();
    }
}
```

### Proxy Management
```csharp
public class ProxyManager
{
    private readonly ILiteCollection<ProxyData> proxies;
    
    public ProxyManager(LiteDatabase db)
    {
        proxies = db.GetCollection<ProxyData>("proxies");
    }
    
    public void ImportProxies(List<string> proxyList)
    {
        var proxyData = proxyList.Select(p => ParseProxy(p)).ToList();
        proxies.InsertBulk(proxyData);
    }
    
    public ProxyData GetWorkingProxy()
    {
        // Get least used working proxy
        return proxies.Query()
            .Where(x => x.IsWorking)
            .OrderBy(x => x.SuccessCount + x.FailCount)
            .FirstOrDefault();
    }
    
    public void UpdateProxyStatus(string address, bool success)
    {
        var proxy = proxies.FindOne(x => x.Address == address);
        if (proxy != null)
        {
            if (success)
                proxy.SuccessCount++;
            else
                proxy.FailCount++;
            
            proxy.LastChecked = DateTime.UtcNow;
            proxy.IsWorking = proxy.FailCount < 5; // Mark as not working after 5 fails
            
            proxies.Update(proxy);
        }
    }
    
    public void CleanupOldProxies(int daysOld = 7)
    {
        var cutoff = DateTime.UtcNow.AddDays(-daysOld);
        proxies.DeleteMany(x => x.LastChecked < cutoff && !x.IsWorking);
    }
}
```

### Configuration Storage
```csharp
public class ConfigStorage
{
    private readonly ILiteCollection<BsonDocument> configs;
    
    public ConfigStorage(LiteDatabase db)
    {
        configs = db.GetCollection("configs");
    }
    
    public void SaveConfig(string name, Config config)
    {
        var doc = BsonMapper.Global.ToDocument(config);
        doc["_id"] = name;
        doc["lastModified"] = DateTime.UtcNow;
        
        configs.Upsert(doc);
    }
    
    public Config LoadConfig(string name)
    {
        var doc = configs.FindById(name);
        if (doc == null) return null;
        
        return BsonMapper.Global.ToObject<Config>(doc);
    }
    
    public List<string> GetConfigNames()
    {
        return configs.Query()
            .Select(x => x["_id"].AsString)
            .ToList();
    }
}
```

### Wordlist Management
```csharp
public class WordlistDatabase
{
    private readonly LiteDatabase db;
    
    public WordlistDatabase(string dbPath)
    {
        db = new LiteDatabase(dbPath);
    }
    
    public void ImportWordlist(string name, string path, string type)
    {
        var collection = db.GetCollection("wordlists");
        var lines = File.ReadAllLines(path);
        
        var wordlist = new
        {
            _id = name,
            Type = type,
            TotalLines = lines.Length,
            ImportDate = DateTime.UtcNow,
            Path = path
        };
        
        collection.Insert(wordlist);
        
        // Store actual data in separate collection
        var dataCollection = db.GetCollection($"wordlist_{name}");
        var documents = lines.Select((line, index) => new
        {
            _id = index,
            Data = line
        });
        
        dataCollection.InsertBulk(documents);
    }
    
    public IEnumerable<string> GetWordlistData(string name, int skip = 0, int take = -1)
    {
        var collection = db.GetCollection($"wordlist_{name}");
        
        var query = collection.Query()
            .OrderBy("_id");
            
        if (skip > 0)
            query = query.Skip(skip);
            
        if (take > 0)
            query = query.Limit(take);
            
        return query.Select(x => x["Data"].AsString).ToEnumerable();
    }
}
```

### Statistics Tracking
```csharp
public class StatsTracker
{
    private readonly ILiteCollection<BsonDocument> stats;
    
    public StatsTracker(LiteDatabase db)
    {
        stats = db.GetCollection("statistics");
    }
    
    public void RecordRun(string config, int tested, int hits, int fails, TimeSpan duration)
    {
        var stat = new
        {
            Config = config,
            Timestamp = DateTime.UtcNow,
            Tested = tested,
            Hits = hits,
            Fails = fails,
            Duration = duration.TotalSeconds,
            CPM = tested / duration.TotalMinutes
        };
        
        stats.Insert(stat);
    }
    
    public object GetStatsSummary(string config = null)
    {
        var query = stats.Query();
        
        if (!string.IsNullOrEmpty(config))
            query = query.Where(x => x["Config"] == config);
            
        var results = query.ToList();
        
        return new
        {
            TotalRuns = results.Count,
            TotalTested = results.Sum(x => x["Tested"].AsInt32),
            TotalHits = results.Sum(x => x["Hits"].AsInt32),
            AverageCPM = results.Average(x => x["CPM"].AsDouble),
            LastRun = results.Max(x => x["Timestamp"].AsDateTime)
        };
    }
}
```

## Advanced Features

### Transactions
```csharp
public void TransferHits(string fromConfig, string toConfig)
{
    using (var trans = db.BeginTrans())
    {
        try
        {
            var hits = db.GetCollection<HitData>("hits");
            
            var toTransfer = hits.Find(x => x.Config == fromConfig).ToList();
            
            foreach (var hit in toTransfer)
            {
                hit.Config = toConfig;
                hits.Update(hit);
            }
            
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }
}
```

### File Storage
```csharp
public class FileStorage
{
    private readonly LiteDatabase db;
    
    public FileStorage(LiteDatabase database)
    {
        db = database;
    }
    
    public void StoreFile(string name, byte[] data)
    {
        var fs = db.FileStorage;
        
        using (var stream = new MemoryStream(data))
        {
            fs.Upload(name, name, stream);
        }
    }
    
    public byte[] RetrieveFile(string name)
    {
        var fs = db.FileStorage;
        var file = fs.FindById(name);
        
        if (file == null) return null;
        
        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            return stream.ToArray();
        }
    }
}
```

### Query Examples
```csharp
// Complex queries
var results = collection.Query()
    .Where(x => x.Timestamp > DateTime.UtcNow.AddDays(-7))
    .Where(x => x.CapturedData.ContainsKey("BALANCE"))
    .OrderByDescending(x => x.Timestamp)
    .Select(x => new { x.Combo, x.Timestamp, Balance = x.CapturedData["BALANCE"] })
    .Limit(100)
    .ToList();

// Using expressions
var highValueHits = collection.Find(x => 
    x.CapturedData.ContainsKey("BALANCE") && 
    Convert.ToDecimal(x.CapturedData["BALANCE"]) > 100);

// Full-text search
collection.EnsureIndex(x => x.Combo, "LOWER($.Combo)");
var searchResults = collection.Find(Query.Contains("Combo", "gmail"));
```

## Performance Optimization

### Indexing Strategy
```csharp
// Create indexes for frequently queried fields
collection.EnsureIndex(x => x.Timestamp);
collection.EnsureIndex(x => x.Config);
collection.EnsureIndex(x => x.Combo, true); // Unique index

// Compound index
collection.EnsureIndex("idx_config_time", 
    BsonExpression.Create("$.Config, $.Timestamp"));
```

### Bulk Operations
```csharp
// Bulk insert for better performance
var documents = Enumerable.Range(0, 10000)
    .Select(i => new HitData { Combo = $"user{i}:pass{i}" });

collection.InsertBulk(documents);

// Bulk update
collection.UpdateMany(
    x => x.Config == "OldConfig",
    x => new HitData { Config = "NewConfig" });
```

### Connection Pooling
```csharp
public class DatabasePool
{
    private readonly string connectionString;
    private readonly Stack<LiteDatabase> pool = new Stack<LiteDatabase>();
    
    public LiteDatabase GetDatabase()
    {
        lock (pool)
        {
            return pool.Count > 0 
                ? pool.Pop() 
                : new LiteDatabase(connectionString);
        }
    }
    
    public void ReturnDatabase(LiteDatabase db)
    {
        lock (pool)
        {
            pool.Push(db);
        }
    }
}
```

## Best Practices
1. Always dispose database connections
2. Use indexes for frequently queried fields
3. Batch operations when possible
4. Use transactions for related operations
5. Implement proper error handling
6. Regular database maintenance (shrink)
7. Backup important data regularly

## Limitations
- 8TB maximum database size
- Single-file database (no sharding)
- Limited concurrent write access
- No server/client architecture
- Basic query capabilities compared to SQL

## Security Considerations
- Encrypt sensitive data before storage
- Use database password for file encryption
- Implement access control in application layer
- Regular backups for data recovery
- Validate input to prevent injection
- Audit database access and modifications