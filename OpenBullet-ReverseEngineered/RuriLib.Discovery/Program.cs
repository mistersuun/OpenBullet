using System.Reflection;

Console.WriteLine("🔍 DISCOVERING ORIGINAL OPENBULLET EXECUTION LOGIC");
Console.WriteLine("=================================================");

await DiscoverRuriLibAPIs();
DiscoverOriginalExecutionEngine();

static async Task DiscoverRuriLibAPIs()
{
    try
    {
        Console.WriteLine("\n📦 Loading RuriLib.dll...");
        
        // Load the assembly
        var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
        var assembly = Assembly.LoadFrom(ruriLibPath);
        
        Console.WriteLine($"✅ Assembly loaded: {assembly.FullName}");
        Console.WriteLine($"📍 Location: {assembly.Location}");
        
        // Get all public types
        var publicTypes = assembly.GetExportedTypes();
        Console.WriteLine($"\n📋 Found {publicTypes.Length} public types");
        
        // Group types by namespace
        var typesByNamespace = publicTypes
            .GroupBy(t => t.Namespace ?? "<no namespace>")
            .OrderBy(g => g.Key);
        
        Console.WriteLine("\n🗂️  NAMESPACE STRUCTURE:");
        Console.WriteLine("========================");
        
        foreach (var nsGroup in typesByNamespace)
        {
            Console.WriteLine($"\n📁 {nsGroup.Key}");
            
            foreach (var type in nsGroup.OrderBy(t => t.Name))
            {
                var typeKind = GetTypeKind(type);
                Console.WriteLine($"   {typeKind} {type.Name}");
                
                // Show constructors for classes
                if (type.IsClass && !type.IsAbstract)
                {
                    var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var ctor in constructors)
                    {
                        var parameters = string.Join(", ", 
                            ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        Console.WriteLine($"      🔨 Constructor({parameters})");
                    }
                }
            }
        }
        
        // Focus on key types we expect based on analysis
        Console.WriteLine("\n🎯 SEARCHING FOR KEY API CLASSES:");
        Console.WriteLine("==================================");
        
        await AnalyzeKeyTypes(assembly);
        
        // Try to test configuration loading if we find the right APIs
        await TestConfigurationLoading(assembly);
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

static string GetTypeKind(Type type)
{
    if (type.IsInterface) return "🔌 INTERFACE";
    if (type.IsEnum) return "📋 ENUM";
    if (type.IsValueType) return "📦 STRUCT";
    if (type.IsAbstract && type.IsSealed) return "🔧 STATIC CLASS";
    if (type.IsAbstract) return "🏗️  ABSTRACT CLASS";
    return "🏛️  CLASS";
}

static async Task AnalyzeKeyTypes(Assembly assembly)
{
    // Look for Config-related types
    var configTypes = assembly.GetTypes()
        .Where(t => t.IsPublic && (t.Name.Contains("Config") || t.Name.Equals("Config", StringComparison.OrdinalIgnoreCase)))
        .ToList();
    
    Console.WriteLine($"\n🔧 CONFIG TYPES FOUND ({configTypes.Count}):");
    foreach (var type in configTypes)
    {
        Console.WriteLine($"   ✅ {type.FullName}");
        
        // Analyze methods
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
            .Where(m => m.DeclaringType == type)
            .ToList();
        
        foreach (var method in methods)
        {
            var parameters = string.Join(", ", 
                method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            var isStatic = method.IsStatic ? "STATIC " : "";
            Console.WriteLine($"      📝 {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
        }
        
        // Analyze properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.DeclaringType == type)
            .ToList();
        
        foreach (var prop in properties)
        {
            var getter = prop.CanRead ? "get;" : "";
            var setter = prop.CanWrite ? "set;" : "";
            Console.WriteLine($"      🏷️  {prop.PropertyType.Name} {prop.Name} {{ {getter} {setter} }}");
        }
    }
    
    // Look for BotData types
    var botDataTypes = assembly.GetTypes()
        .Where(t => t.IsPublic && (t.Name.Contains("BotData") || t.Name.Contains("Bot")))
        .ToList();
    
    Console.WriteLine($"\n🤖 BOTDATA TYPES FOUND ({botDataTypes.Count}):");
    foreach (var type in botDataTypes)
    {
        Console.WriteLine($"   ✅ {type.FullName}");
    }
    
    // Look for Runner types
    var runnerTypes = assembly.GetTypes()
        .Where(t => t.IsPublic && t.Name.Contains("Runner"))
        .ToList();
    
    Console.WriteLine($"\n🏃 RUNNER TYPES FOUND ({runnerTypes.Count}):");
    foreach (var type in runnerTypes)
    {
        Console.WriteLine($"   ✅ {type.FullName}");
    }
    
    // Look for LoliScript-related types
    var loliScriptTypes = assembly.GetTypes()
        .Where(t => t.IsPublic && (t.Name.Contains("LoliScript") || t.Name.Contains("Script") || t.Name.Contains("Parser")))
        .ToList();
    
    Console.WriteLine($"\n📜 LOLISCRIPT TYPES FOUND ({loliScriptTypes.Count}):");
    foreach (var type in loliScriptTypes)
    {
        Console.WriteLine($"   ✅ {type.FullName}");
    }
}

static async Task TestConfigurationLoading(Assembly assembly)
{
    Console.WriteLine("\n🧪 TESTING CONFIGURATION LOADING:");
    Console.WriteLine("==================================");
    
    try
    {
        // Try to find the Config class
        var configType = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == "Config" && t.IsPublic);
        
        if (configType == null)
        {
            Console.WriteLine("❌ Config class not found");
            return;
        }
        
        Console.WriteLine($"✅ Found Config class: {configType.FullName}");
        
        // Look for LoadFromFile or similar static methods
        var loadMethods = configType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name.Contains("Load") || m.Name.Contains("From"))
            .ToList();
        
        Console.WriteLine($"📋 Found {loadMethods.Count} potential load methods:");
        foreach (var method in loadMethods)
        {
            var parameters = string.Join(", ", 
                method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            Console.WriteLine($"   🔄 {method.ReturnType.Name} {method.Name}({parameters})");
        }
        
        // Try to test config loading with a sample .anom file
        var sampleConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "amazonChecker.anom");
        if (File.Exists(sampleConfigPath))
        {
            Console.WriteLine($"📄 Found sample config: {sampleConfigPath}");
            
            foreach (var loadMethod in loadMethods)
            {
                try
                {
                    var parameters = loadMethod.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                    {
                        Console.WriteLine($"🧪 Testing {loadMethod.Name} with sample config...");
                        var result = loadMethod.Invoke(null, new object[] { sampleConfigPath });
                        
                        if (result != null)
                        {
                            Console.WriteLine($"✅ SUCCESS! Config loaded using {loadMethod.Name}");
                            Console.WriteLine($"   Result type: {result.GetType().FullName}");
                            
                            // Try to extract properties from the loaded config
                            await AnalyzeLoadedConfig(result);
                        }
                        else
                        {
                            Console.WriteLine($"❌ {loadMethod.Name} returned null");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ {loadMethod.Name} failed: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine($"❌ Sample config not found at: {sampleConfigPath}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Configuration loading test failed: {ex.Message}");
    }
}

static async Task AnalyzeLoadedConfig(object config)
{
    try
    {
        var configType = config.GetType();
        Console.WriteLine($"\n📊 ANALYZING LOADED CONFIG ({configType.Name}):");
        
        var properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(config);
                var valueStr = value?.ToString() ?? "<null>";
                
                // Truncate long values
                if (valueStr.Length > 100)
                {
                    valueStr = valueStr.Substring(0, 97) + "...";
                }
                
                Console.WriteLine($"   🏷️  {prop.Name} ({prop.PropertyType.Name}): {valueStr}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   🏷️  {prop.Name} ({prop.PropertyType.Name}): <error reading: {ex.Message}>");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Config analysis failed: {ex.Message}");
    }
}

static void DiscoverOriginalExecutionEngine()
{
    Console.WriteLine("\n" + new string('=', 80));
    Console.WriteLine("🔥 DISCOVERING ORIGINAL EXECUTION ENGINE");
    Console.WriteLine(new string('=', 80));
    
    try
    {
        var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
        var assembly = Assembly.LoadFrom(ruriLibPath);
        var types = assembly.GetExportedTypes();
        
        // 1. Look for Block-based execution (suggested by BlocksAmount = 1)
        Console.WriteLine("\n📦 BLOCK TYPES (Block-based execution):");
        var blockTypes = types.Where(t => t.Name.Contains("Block")).ToList();
        
        foreach (var type in blockTypes)
        {
            Console.WriteLine($"   ✅ {type.FullName}");
            
            // Check for Execute methods in blocks
            var executeMethods = type.GetMethods()
                .Where(m => m.Name.Contains("Execute") || m.Name.Contains("Process"))
                .ToList();
            
            foreach (var method in executeMethods)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                var isStatic = method.IsStatic ? "STATIC " : "";
                Console.WriteLine($"      🚀 {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
            }
        }
        
        // 2. Look for LoliScript execution engine
        Console.WriteLine("\n📜 LOLISCRIPT/EXECUTION TYPES:");
        var loliTypes = types.Where(t => 
            t.Name.Contains("LoliScript") || 
            t.Name.Contains("Script") || 
            t.Name.Contains("Parser") ||
            t.Name.Contains("Interpreter") ||
            t.Name.Contains("Execute")).ToList();
        
        foreach (var type in loliTypes)
        {
            Console.WriteLine($"   📜 {type.FullName}");
            
            var methods = type.GetMethods()
                .Where(m => m.Name.Contains("Execute") || 
                           m.Name.Contains("Process") || 
                           m.Name.Contains("Run") ||
                           m.Name.Contains("Parse"))
                .ToList();
            
            foreach (var method in methods)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                var isStatic = method.IsStatic ? "STATIC " : "";
                Console.WriteLine($"      🚀 {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
            }
        }
        
        // 3. Check for ProcessScript methods across all types
        Console.WriteLine("\n🔍 SCRIPT PROCESSING METHODS:");
        foreach (var type in types)
        {
            var scriptMethods = type.GetMethods()
                .Where(m => m.Name.Contains("ProcessScript") || 
                           m.Name.Contains("ExecuteScript") ||
                           m.Name.Contains("RunScript") ||
                           m.Name.Contains("ProcessBlock"))
                .ToList();
            
            if (scriptMethods.Any())
            {
                Console.WriteLine($"   ✅ {type.FullName}");
                foreach (var method in scriptMethods)
                {
                    var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                    var isStatic = method.IsStatic ? "STATIC " : "";
                    Console.WriteLine($"      🔥 {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
                }
            }
        }
        
        Console.WriteLine("\n🎯 ORIGINAL EXECUTION ANALYSIS COMPLETE!");
        Console.WriteLine("Now we know exactly what methods the original OpenBullet uses!");
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}
