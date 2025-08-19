using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RuriLib.Discovery
{
    public class OriginalLogicFinder
    {
        public static void AnalyzeOriginalExecution()
        {
            Console.WriteLine("🔍 DISCOVERING ORIGINAL OPENBULLET EXECUTION LOGIC");
            Console.WriteLine("=================================================");
            
            try
            {
                var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
                var assembly = Assembly.LoadFrom(ruriLibPath);
                var types = assembly.GetExportedTypes();
                
                Console.WriteLine($"📋 Total types in RuriLib: {types.Length}");
                
                // 1. Look for Block-based execution (suggested by BlocksAmount = 1)
                Console.WriteLine("\n🔥 SEARCHING FOR BLOCK-BASED EXECUTION:");
                var blockTypes = types.Where(t => t.Name.Contains("Block")).ToList();
                
                foreach (var type in blockTypes)
                {
                    Console.WriteLine($"   📦 BLOCK TYPE: {type.FullName}");
                    
                    // Check for Execute methods
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
                Console.WriteLine("\n📜 SEARCHING FOR LOLISCRIPT EXECUTION:");
                var loliScriptTypes = types.Where(t => 
                    t.Name.Contains("LoliScript") || 
                    t.Name.Contains("Script") || 
                    t.Name.Contains("Parser") ||
                    t.Name.Contains("Interpreter")).ToList();
                
                foreach (var type in loliScriptTypes)
                {
                    Console.WriteLine($"   📜 LOLISCRIPT TYPE: {type.FullName}");
                    
                    var methods = type.GetMethods()
                        .Where(m => m.Name.Contains("Execute") || 
                                   m.Name.Contains("Process") || 
                                   m.Name.Contains("Parse") ||
                                   m.Name.Contains("Run"))
                        .ToList();
                    
                    foreach (var method in methods)
                    {
                        var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                        var isStatic = method.IsStatic ? "STATIC " : "";
                        Console.WriteLine($"      🚀 {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
                    }
                }
                
                // 3. Check BotData for execution methods
                Console.WriteLine("\n🤖 BOTDATA EXECUTION METHODS:");
                var botDataType = types.FirstOrDefault(t => t.Name == "BotData");
                if (botDataType != null)
                {
                    var executeMethods = botDataType.GetMethods()
                        .Where(m => m.Name.Contains("Execute") || 
                                   m.Name.Contains("Process") || 
                                   m.Name.Contains("Run"))
                        .ToList();
                    
                    foreach (var method in executeMethods)
                    {
                        var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                        Console.WriteLine($"   🚀 {method.ReturnType.Name} {method.Name}({parameters})");
                    }
                }
                
                // 4. Check Runner classes for the main execution flow
                Console.WriteLine("\n🏃 RUNNER EXECUTION FLOW:");
                var runnerTypes = types.Where(t => t.Name.Contains("Runner")).ToList();
                
                foreach (var type in runnerTypes)
                {
                    Console.WriteLine($"   🏃 RUNNER TYPE: {type.FullName}");
                    
                    var methods = type.GetMethods()
                        .Where(m => m.Name.Contains("Start") || 
                                   m.Name.Contains("Run") || 
                                   m.Name.Contains("Execute") ||
                                   m.Name.Contains("Process"))
                        .ToList();
                    
                    foreach (var method in methods)
                    {
                        var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                        var isStatic = method.IsStatic ? "STATIC " : "";
                        Console.WriteLine($"      🚀 {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
                    }
                }
                
                // 5. Look for any ProcessScript methods across all types
                Console.WriteLine("\n🔍 SEARCHING FOR SCRIPT PROCESSING:");
                foreach (var type in types)
                {
                    var scriptMethods = type.GetMethods()
                        .Where(m => m.Name.Contains("ProcessScript") || 
                                   m.Name.Contains("ExecuteScript") ||
                                   m.Name.Contains("RunScript"))
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
                
                Console.WriteLine("\n🎯 CONCLUSION:");
                Console.WriteLine("Need to use RuriLib's original execution engine, not custom parsing!");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}

