using System;
using System.Reflection;
using System.Linq;

namespace OpenBullet_Console
{
    /// <summary>
    /// Tool to examine the original OpenBullet DLL files and understand their anti-detection capabilities
    /// </summary>
    public static class DllAnalyzer
    {
        public static void AnalyzeRuriLib()
        {
            try
            {
                Console.WriteLine("🔍 ===== ANALYZING ORIGINAL OPENBULLET DLL FILES =====");
                
                // Load RuriLib.dll
                var ruriLibPath = "../libs/RuriLib.dll";
                Console.WriteLine($"📚 Loading: {ruriLibPath}");
                
                var assembly = Assembly.LoadFrom(ruriLibPath);
                Console.WriteLine($"✅ Assembly loaded: {assembly.FullName}");
                
                // Get all types
                var types = assembly.GetTypes();
                Console.WriteLine($"📋 Found {types.Length} types in RuriLib");
                
                Console.WriteLine("\n🎯 KEY TYPES FOR HTTP/COOKIE HANDLING:");
                foreach (var type in types.Where(t => 
                    t.Name.Contains("Http") || 
                    t.Name.Contains("Cookie") || 
                    t.Name.Contains("Client") ||
                    t.Name.Contains("Request") ||
                    t.Name.Contains("Bot") ||
                    t.Name.Contains("Data")))
                {
                    Console.WriteLine($"   📄 {type.FullName}");
                    
                    // Show key methods
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .Where(m => !m.IsSpecialName && m.DeclaringType == type)
                        .Take(5);
                    
                    foreach (var method in methods)
                    {
                        Console.WriteLine($"      🔧 {method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name))})");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error analyzing RuriLib: {ex.Message}");
            }
        }
        
        public static void AnalyzeLeafXNet()
        {
            try
            {
                Console.WriteLine("\n🔍 ===== ANALYZING LEAF.XNET.DLL =====");
                
                var leafPath = "../libs/Leaf.xNet.dll";
                Console.WriteLine($"📚 Loading: {leafPath}");
                
                var assembly = Assembly.LoadFrom(leafPath);
                Console.WriteLine($"✅ Assembly loaded: {assembly.FullName}");
                
                var types = assembly.GetTypes();
                Console.WriteLine($"📋 Found {types.Length} types in Leaf.xNet");
                
                Console.WriteLine("\n🎯 KEY TYPES FOR ANTI-DETECTION:");
                foreach (var type in types.Where(t => 
                    t.Name.Contains("Http") || 
                    t.Name.Contains("Cookie") || 
                    t.Name.Contains("Browser") ||
                    t.Name.Contains("Client") ||
                    t.Name.Contains("Proxy")))
                {
                    Console.WriteLine($"   📄 {type.FullName}");
                    
                    // Show key methods
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .Where(m => !m.IsSpecialName && m.DeclaringType == type)
                        .Take(3);
                    
                    foreach (var method in methods)
                    {
                        Console.WriteLine($"      🔧 {method.Name}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error analyzing Leaf.xNet: {ex.Message}");
            }
        }
    }
}

