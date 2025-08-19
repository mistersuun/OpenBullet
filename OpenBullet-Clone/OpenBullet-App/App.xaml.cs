using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace OpenBullet
{
    public partial class App : Application
    {
        public static Models.Settings.OBSettings OBSettings { get; set; }
        public static Models.Settings.RLSettings RLSettings { get; set; }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Load settings
            LoadSettings();
            
            // Initialize database
            InitializeDatabase();
            
            // Setup exception handling
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        
        private void LoadSettings()
        {
            try
            {
                // Load OBSettings
                var obSettingsPath = "Settings/OBSettings.json";
                if (File.Exists(obSettingsPath))
                {
                    var obJson = File.ReadAllText(obSettingsPath);
                    OBSettings = JsonConvert.DeserializeObject<Models.Settings.OBSettings>(obJson);
                }
                else
                {
                    OBSettings = new Models.Settings.OBSettings(); // Default settings
                }
                
                // Load RLSettings  
                var rlSettingsPath = "Settings/RLSettings.json";
                if (File.Exists(rlSettingsPath))
                {
                    var rlJson = File.ReadAllText(rlSettingsPath);
                    RLSettings = JsonConvert.DeserializeObject<Models.Settings.RLSettings>(rlJson);
                }
                else
                {
                    RLSettings = new Models.Settings.RLSettings(); // Default settings
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void InitializeDatabase()
        {
            try
            {
                // Create DB directory if it doesn't exist
                Directory.CreateDirectory("DB");
                
                // Initialize LiteDB database
                var dbPath = "DB/OpenBullet.db";
                if (!File.Exists(dbPath))
                {
                    // Create empty database
                    using (var db = new LiteDB.LiteDatabase(dbPath))
                    {
                        // Database will be created automatically
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled exception: {e.Exception.Message}\n\nStackTrace:\n{e.Exception.StackTrace}", 
                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}


