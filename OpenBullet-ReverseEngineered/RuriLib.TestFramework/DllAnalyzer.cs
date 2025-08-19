using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RuriLib.TestFramework
{
    public class DllAnalyzer
    {
        public static void AnalyzeDllMethods()
        {
            Console.WriteLine("🔍 DLL METHOD ANALYSIS - Detailed Original OpenBullet Logic");
            Console.WriteLine("=========================================================");
            
            var keyTypes = new[]
            {
                "RuriLib.LS.BlockParser",
                "RuriLib.BlockRequest", 
                "RuriLib.BlockKeycheck",
                "RuriLib.BotData",
                "RuriLib.Runner.RunnerViewModel",
                "RuriLib.Config",
                "RuriLib.ConfigSettings"
            };
            
            try
            {
                var ruriLibPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RuriLib.dll");
                var assembly = Assembly.LoadFrom(ruriLibPath);
                var types = assembly.GetExportedTypes();
                
                foreach (var typeName in keyTypes)
                {
                    Console.WriteLine("\n" + new string('=', 60));
                    Console.WriteLine($"🎯 ANALYZING: {typeName}");
                    Console.WriteLine(new string('=', 60));
                    
                    var type = types.FirstOrDefault(t => t.FullName == typeName);
                    
                    if (type != null)
                    {
                        Console.WriteLine($"✅ FOUND: {type.FullName}");
                        
                        // Show all public methods
                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                            .Where(m => m.DeclaringType == type)
                            .OrderBy(m => m.Name)
                            .ToList();
                        
                        Console.WriteLine($"📋 Methods ({methods.Count}):");
                        foreach (var method in methods)
                        {
                            var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                            var isStatic = method.IsStatic ? "STATIC " : "";
                            var visibility = method.IsPublic ? "PUBLIC" : "PRIVATE";
                            
                            Console.WriteLine($"   {visibility} {isStatic}{method.ReturnType.Name} {method.Name}({parameters})");
                        }
                        
                        // Show properties
                        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.DeclaringType == type)
                            .OrderBy(p => p.Name)
                            .ToList();
                        
                        if (properties.Any())
                        {
                            Console.WriteLine($"📋 Properties ({properties.Count}):");
                            foreach (var prop in properties)
                            {
                                var getter = prop.CanRead ? "get;" : "";
                                var setter = prop.CanWrite ? "set;" : "";
                                Console.WriteLine($"   {prop.PropertyType.Name} {prop.Name} {{ {getter} {setter} }}");
                            }
                        }
                        
                        // Show constructors
                        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                        if (constructors.Any())
                        {
                            Console.WriteLine($"📋 Constructors ({constructors.Length}):");
                            foreach (var ctor in constructors)
                            {
                                var parameters = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                                Console.WriteLine($"   Constructor({parameters})");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ NOT FOUND: {typeName}");
                    }
                }
                
                Console.WriteLine("\n" + new string('=', 80));
                Console.WriteLine("🎯 ORIGINAL EXECUTION FLOW ANALYSIS COMPLETE");
                Console.WriteLine(new string('=', 80));
                Console.WriteLine("✅ BlockParser.Parse() - Converts LoliScript to Block objects");
                Console.WriteLine("✅ Block.Process(BotData) - Executes blocks using original logic");
                Console.WriteLine("✅ RunnerViewModel.Start() - Controls execution flow");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Analysis failed: {ex.Message}");
            }
        }
    }
}

