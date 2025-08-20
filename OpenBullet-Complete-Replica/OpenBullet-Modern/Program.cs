using System;
using System.Threading;
using System.Windows;

namespace OpenBullet_Modern
{
    /// <summary>
    /// Main entry point for .NET 9 version
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("🎯 OpenBullet Anomaly Replica - .NET 9 Edition");
                Console.WriteLine("=================================================");
                Console.WriteLine();
                
                // Check for original DLL compatibility
                CheckDllCompatibility();
                
                var app = new Application();
                app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Application startup error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
        
        private static void CheckDllCompatibility()
        {
            Console.WriteLine("🔍 Checking DLL compatibility...");
            
            var requiredDlls = new[]
            {
                "../libs/RuriLib.dll",
                "../libs/Leaf.xNet.dll", 
                "../libs/LiteDB.dll",
                "../libs/Newtonsoft.Json.dll"
            };
            
            bool allFound = true;
            foreach (var dll in requiredDlls)
            {
                if (File.Exists(dll))
                {
                    Console.WriteLine($"✅ Found: {Path.GetFileName(dll)}");
                }
                else
                {
                    Console.WriteLine($"❌ Missing: {Path.GetFileName(dll)}");
                    allFound = false;
                }
            }
            
            if (!allFound)
            {
                Console.WriteLine();
                Console.WriteLine("⚠️  Some original DLLs are missing. The application will run in simulation mode.");
                Console.WriteLine("📋 To get full functionality, copy DLLs from the original OpenBullet installation.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("✅ All required DLLs found!");
                Console.WriteLine("⚠️  Note: .NET 9 may not be fully compatible with .NET Framework DLLs.");
                Console.WriteLine("📋 For best compatibility, use the .NET Framework 4.7.2 version.");
            }
            
            Console.WriteLine();
        }
    }
}

