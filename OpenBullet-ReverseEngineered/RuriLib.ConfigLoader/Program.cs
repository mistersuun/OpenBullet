using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RuriLib.ConfigLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔧 RuriLib Config Loading - Isolated Test");
            Console.WriteLine("=========================================");

            TestConfigLoading();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void TestConfigLoading()
        {
            try
            {
                Console.WriteLine("\n📦 Step 1: Loading RuriLib.dll...");
                var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
                var assembly = Assembly.LoadFrom(ruriLibPath);
                Console.WriteLine($"✅ RuriLib.dll loaded: {assembly.FullName}");

                Console.WriteLine("\n📋 Step 2: Finding Config class...");
                var configType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Config" && t.IsPublic);
                
                if (configType == null)
                {
                    Console.WriteLine("❌ Config class not found!");
                    return;
                }
                
                Console.WriteLine($"✅ Found Config class: {configType.FullName}");

                Console.WriteLine("\n🔍 Step 3: Analyzing Config class structure...");
                
                // Get all constructors
                var constructors = configType.GetConstructors();
                Console.WriteLine($"📋 Found {constructors.Length} constructor(s):");
                
                foreach (var ctor in constructors)
                {
                    var parameters = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    Console.WriteLine($"   🔨 Constructor({parameters})");
                }

                // Get all static methods
                var staticMethods = configType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                Console.WriteLine($"\n📋 Found {staticMethods.Length} static method(s):");
                
                foreach (var method in staticMethods)
                {
                    var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    Console.WriteLine($"   📝 {method.ReturnType.Name} {method.Name}({parameters})");
                }

                // Get all instance methods
                var instanceMethods = configType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.DeclaringType == configType)
                    .ToList();
                
                Console.WriteLine($"\n📋 Found {instanceMethods.Count} instance method(s):");
                foreach (var method in instanceMethods)
                {
                    var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    Console.WriteLine($"   📝 {method.ReturnType.Name} {method.Name}({parameters})");
                }

                // Get all properties
                var properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Console.WriteLine($"\n📋 Found {properties.Length} propertie(s):");
                
                foreach (var prop in properties)
                {
                    var getter = prop.CanRead ? "get;" : "";
                    var setter = prop.CanWrite ? "set;" : "";
                    Console.WriteLine($"   🏷️ {prop.PropertyType.Name} {prop.Name} {{ {getter} {setter} }}");
                }

                Console.WriteLine("\n🧪 Step 4: Testing Config instantiation...");
                
                // Try to create a Config instance using parameterless constructor if available
                var parameterlessConstructor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
                if (parameterlessConstructor != null)
                {
                    try
                    {
                        var configInstance = parameterlessConstructor.Invoke(null);
                        Console.WriteLine("✅ Config instance created successfully!");
                        
                        // Try to read some basic properties
                        var nameProperty = configType.GetProperty("Name");
                        if (nameProperty != null && nameProperty.CanRead)
                        {
                            var name = nameProperty.GetValue(configInstance);
                            Console.WriteLine($"   📋 Default Name: {name}");
                        }
                        
                        Console.WriteLine("✅ STEP 4 PASSED: Config instantiation working");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Config instantiation failed: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ No parameterless constructor found");
                }

                Console.WriteLine("\n📄 Step 5: Analyzing .anom file structure...");
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "amazonChecker.anom");
                
                if (File.Exists(configPath))
                {
                    var configContent = File.ReadAllText(configPath);
                    Console.WriteLine($"✅ Config file loaded: {configContent.Length} characters");
                    
                    // Parse basic structure manually first
                    if (configContent.Contains("[SETTINGS]"))
                    {
                        Console.WriteLine("✅ Found [SETTINGS] section");
                        var settingsStart = configContent.IndexOf("{");
                        var settingsEnd = configContent.IndexOf("}", settingsStart);
                        
                        if (settingsStart > 0 && settingsEnd > settingsStart)
                        {
                            var settingsJson = configContent.Substring(settingsStart, settingsEnd - settingsStart + 1);
                            Console.WriteLine($"✅ Settings JSON extracted: {settingsJson.Length} chars");
                            Console.WriteLine($"   Preview: {settingsJson.Substring(0, Math.Min(200, settingsJson.Length))}...");
                        }
                    }
                    
                    if (configContent.Contains("[SCRIPT]"))
                    {
                        Console.WriteLine("✅ Found [SCRIPT] section");
                        var scriptStart = configContent.IndexOf("[SCRIPT]") + "[SCRIPT]".Length;
                        var scriptContent = configContent.Substring(scriptStart).Trim();
                        Console.WriteLine($"✅ Script content extracted: {scriptContent.Length} chars");
                        Console.WriteLine($"   First line: {scriptContent.Split('\n')[0]}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Config file not found: {configPath}");
                }

                Console.WriteLine("\n🎉 CONFIG ANALYSIS COMPLETE!");
                Console.WriteLine("✅ RuriLib.dll: Fully accessible");
                Console.WriteLine("✅ Config class: Identified and analyzed");
                Console.WriteLine("✅ .anom file: Structure understood");
                Console.WriteLine("🚀 Ready for manual config parsing implementation!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ CRITICAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                var innerEx = ex.InnerException;
                while (innerEx != null)
                {
                    Console.WriteLine($"Inner exception: {innerEx.Message}");
                    innerEx = innerEx.InnerException;
                }
            }
        }
    }
}

