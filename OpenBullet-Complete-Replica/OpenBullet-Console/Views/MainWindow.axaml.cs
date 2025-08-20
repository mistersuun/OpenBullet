using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using OpenBullet_Console.ViewModels;
using System;
using System.Collections.Generic;

namespace OpenBullet_Console.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        // Set reference for file dialogs in ViewModel
        ViewModels.MainWindowViewModel.MainWindow = this;
        
        // Apply default theme
        ApplyTheme("Dark");
    }
    
    public void OnThemeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            string selectedTheme = e.AddedItems[0]?.ToString() ?? "Dark";
            ApplyTheme(selectedTheme);
        }
    }
    
    private void ApplyTheme(string themeName)
    {
        var themes = new Dictionary<string, Dictionary<string, string>>
        {
            ["Dark"] = new()
            {
                // Backgrounds
                ["WindowBackground"] = "#1E1E1E",
                ["HeaderBackground"] = "#252526",
                ["CardBackground"] = "#2D2D30",
                ["InputBackground"] = "#3C3C3C",
                ["ButtonBackground"] = "#404040",
                
                // Text Colors
                ["TextPrimary"] = "#FFFFFF",
                ["TextSecondary"] = "#CCCCCC",
                ["TextMuted"] = "#999999",
                ["TextInverse"] = "#000000",
                
                // Theme Colors
                ["AccentColor"] = "#007ACC",
                ["AccentHover"] = "#005A99",
                ["SuccessColor"] = "#27AE60",
                ["SuccessHover"] = "#229954",
                ["DangerColor"] = "#E74C3C",
                ["DangerHover"] = "#C0392B",
                ["WarningColor"] = "#F39C12",
                ["WarningHover"] = "#D68910",
                ["InfoColor"] = "#3498DB",
                ["InfoHover"] = "#2980B9",
                
                // Borders & Separators
                ["BorderColor"] = "#555555",
                ["BorderLight"] = "#666666",
                ["SeparatorColor"] = "#444444"
            },
            ["Light"] = new()
            {
                // Backgrounds  
                ["WindowBackground"] = "#FFFFFF",
                ["HeaderBackground"] = "#F8F9FA",
                ["CardBackground"] = "#FFFFFF",
                ["InputBackground"] = "#F8F9FA",
                ["ButtonBackground"] = "#E9ECEF",
                
                // Text Colors
                ["TextPrimary"] = "#212529",
                ["TextSecondary"] = "#495057",
                ["TextMuted"] = "#6C757D",
                ["TextInverse"] = "#FFFFFF",
                
                // Theme Colors
                ["AccentColor"] = "#007BFF",
                ["AccentHover"] = "#0056B3",
                ["SuccessColor"] = "#28A745",
                ["SuccessHover"] = "#218838",
                ["DangerColor"] = "#DC3545",
                ["DangerHover"] = "#C82333",
                ["WarningColor"] = "#FFC107",
                ["WarningHover"] = "#E0A800",
                ["InfoColor"] = "#17A2B8",
                ["InfoHover"] = "#138496",
                
                // Borders & Separators
                ["BorderColor"] = "#DEE2E6",
                ["BorderLight"] = "#E9ECEF",
                ["SeparatorColor"] = "#CED4DA"
            },
            ["Matrix"] = new()
            {
                // Backgrounds
                ["WindowBackground"] = "#000000",
                ["HeaderBackground"] = "#001100",
                ["CardBackground"] = "#001A00",
                ["InputBackground"] = "#002200",
                ["ButtonBackground"] = "#003300",
                
                // Text Colors
                ["TextPrimary"] = "#00FF00",
                ["TextSecondary"] = "#00CC00",
                ["TextMuted"] = "#008800",
                ["TextInverse"] = "#000000",
                
                // Theme Colors
                ["AccentColor"] = "#00FF41",
                ["AccentHover"] = "#00DD33",
                ["SuccessColor"] = "#00FF00",
                ["SuccessHover"] = "#00DD00",
                ["DangerColor"] = "#FF0000",
                ["DangerHover"] = "#DD0000",
                ["WarningColor"] = "#FFFF00",
                ["WarningHover"] = "#DDDD00",
                ["InfoColor"] = "#00FFFF",
                ["InfoHover"] = "#00DDDD",
                
                // Borders & Separators
                ["BorderColor"] = "#004400",
                ["BorderLight"] = "#006600",
                ["SeparatorColor"] = "#002200"
            },
            ["Anomaly Classic"] = new()
            {
                // Backgrounds
                ["WindowBackground"] = "#2B2B2B",
                ["HeaderBackground"] = "#3C3C3C",
                ["CardBackground"] = "#404040",
                ["InputBackground"] = "#4A4A4A",
                ["ButtonBackground"] = "#555555",
                
                // Text Colors
                ["TextPrimary"] = "#FFFFFF",
                ["TextSecondary"] = "#E0E0E0",
                ["TextMuted"] = "#BBBBBB",
                ["TextInverse"] = "#000000",
                
                // Theme Colors (OpenBullet Classic Orange Theme)
                ["AccentColor"] = "#FF6B35",
                ["AccentHover"] = "#E55A2B",
                ["SuccessColor"] = "#4CAF50",
                ["SuccessHover"] = "#45A049",
                ["DangerColor"] = "#F44336",
                ["DangerHover"] = "#DA190B",
                ["WarningColor"] = "#FF9800",
                ["WarningHover"] = "#E68900",
                ["InfoColor"] = "#2196F3",
                ["InfoHover"] = "#0B7DDA",
                
                // Borders & Separators
                ["BorderColor"] = "#666666",
                ["BorderLight"] = "#777777",
                ["SeparatorColor"] = "#555555"
            },
            ["Neon Blue"] = new()
            {
                // Backgrounds
                ["WindowBackground"] = "#0A0A23",
                ["HeaderBackground"] = "#1A1A3A",
                ["CardBackground"] = "#2A2A5A",
                ["InputBackground"] = "#3A3A6A",
                ["ButtonBackground"] = "#4A4A7A",
                
                // Text Colors
                ["TextPrimary"] = "#FFFFFF",
                ["TextSecondary"] = "#E0E0FF",
                ["TextMuted"] = "#BBBBFF",
                ["TextInverse"] = "#000000",
                
                // Theme Colors (Neon Blue/Cyan Theme)
                ["AccentColor"] = "#00D4FF",
                ["AccentHover"] = "#00B8E6",
                ["SuccessColor"] = "#00FFD4",
                ["SuccessHover"] = "#00E6BB",
                ["DangerColor"] = "#FF0080",
                ["DangerHover"] = "#E6006B",
                ["WarningColor"] = "#FFD400",
                ["WarningHover"] = "#E6BB00",
                ["InfoColor"] = "#8A2BE2",
                ["InfoHover"] = "#7B1FA2",
                
                // Borders & Separators
                ["BorderColor"] = "#5A5A8A",
                ["BorderLight"] = "#6A6A9A",
                ["SeparatorColor"] = "#4A4A7A"
            }
        };
        
        if (themes.TryGetValue(themeName, out var themeColors))
        {
            // Apply theme colors to the window
            Background = Brush.Parse(themeColors["WindowBackground"]);
            
            // Update ALL dynamic resources for comprehensive theming
            try 
            {
                // Background Resources
                Resources["WindowBackgroundBrush"] = Brush.Parse(themeColors["WindowBackground"]);
                Resources["HeaderBackgroundBrush"] = Brush.Parse(themeColors["HeaderBackground"]);
                Resources["CardBackgroundBrush"] = Brush.Parse(themeColors["CardBackground"]);
                Resources["InputBackgroundBrush"] = Brush.Parse(themeColors["InputBackground"]);
                Resources["ButtonBackgroundBrush"] = Brush.Parse(themeColors["ButtonBackground"]);
                
                // Text Resources
                Resources["TextPrimaryBrush"] = Brush.Parse(themeColors["TextPrimary"]);
                Resources["TextSecondaryBrush"] = Brush.Parse(themeColors["TextSecondary"]);
                Resources["TextMutedBrush"] = Brush.Parse(themeColors["TextMuted"]);
                Resources["TextInverseBrush"] = Brush.Parse(themeColors["TextInverse"]);
                
                // Theme Color Resources
                Resources["AccentColorBrush"] = Brush.Parse(themeColors["AccentColor"]);
                Resources["AccentHoverBrush"] = Brush.Parse(themeColors["AccentHover"]);
                Resources["SuccessColorBrush"] = Brush.Parse(themeColors["SuccessColor"]);
                Resources["SuccessHoverBrush"] = Brush.Parse(themeColors["SuccessHover"]);
                Resources["DangerColorBrush"] = Brush.Parse(themeColors["DangerColor"]);
                Resources["DangerHoverBrush"] = Brush.Parse(themeColors["DangerHover"]);
                Resources["WarningColorBrush"] = Brush.Parse(themeColors["WarningColor"]);
                Resources["WarningHoverBrush"] = Brush.Parse(themeColors["WarningHover"]);
                Resources["InfoColorBrush"] = Brush.Parse(themeColors["InfoColor"]);
                Resources["InfoHoverBrush"] = Brush.Parse(themeColors["InfoHover"]);
                
                // Border & Separator Resources
                Resources["BorderColorBrush"] = Brush.Parse(themeColors["BorderColor"]);
                Resources["BorderLightBrush"] = Brush.Parse(themeColors["BorderLight"]);
                Resources["SeparatorColorBrush"] = Brush.Parse(themeColors["SeparatorColor"]);
                
                // Log comprehensive theme change
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.AddLog($"🎨 Theme changed to: {themeName}");
                    viewModel.AddLog($"  🖌️ Applied {themeColors.Count} comprehensive color definitions");
                    viewModel.AddLog($"  🎯 Updated buttons, text, backgrounds, borders, and all UI elements");
                    
                    switch (themeName)
                    {
                        case "Light":
                            viewModel.AddLog("  ☀️ Clean white interface with professional blue accents");
                            break;
                        case "Matrix":
                            viewModel.AddLog("  💚 Terminal green-on-black hacker aesthetic");
                            break;
                        case "Anomaly Classic":
                            viewModel.AddLog("  🔥 Original OpenBullet orange theme with classic styling");
                            break;
                        case "Neon Blue":
                            viewModel.AddLog("  💙 Futuristic neon blue/cyan color scheme");
                            break;
                        default:
                            viewModel.AddLog("  🌙 Professional dark theme with blue accents");
                            break;
                    }
                    
                    // Update the ViewModel's SelectedTheme if it's different
                    if (viewModel.SelectedTheme != themeName)
                    {
                        viewModel.SelectedTheme = themeName;
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback if resource updating fails
                Console.WriteLine($"Theme update error: {ex.Message}");
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.AddLog($"❌ Theme update error: {ex.Message}");
                }
            }
        }
    }
}

