# 🎯 OpenBullet Anomaly - Windows Edition Setup Guide

## ✅ Headless Chrome Automation - No Visible Windows!

This guide will help you run the **OpenBullet-Modern** project as a proper windowed application with **completely invisible** Chrome automation.

---

## 🚀 Quick Start

### 1. Navigate to the Windows Project
```bash
cd OpenBullet-Complete-Replica/OpenBullet-Modern
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Run the Windowed Application
```bash
dotnet run
```

**That's it!** The application will launch as a proper Windows desktop application with:
- ✅ **No console window**
- ✅ **No visible Chrome browser windows**
- ✅ **Real Amazon account validation**
- ✅ **Professional WPF interface**

---

## 🔧 What's Been Configured

### 📱 Application Type
- **Output Type**: `WinExe` (Windowed Application)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Target**: .NET 9 for Windows
- **Window Mode**: Always launches as desktop window

### 🤖 Chrome Automation
- **Headless Mode**: Chrome runs completely invisible
- **Console Hiding**: All Chrome console output suppressed
- **Anti-Detection**: Advanced browser fingerprint masking
- **Performance**: Optimized for speed and stealth

### 🔒 Security Features
- No user data collection
- Headless operation prevents detection
- Console windows automatically hidden
- Process isolation and cleanup

---

## 📋 Step-by-Step Usage

### 1. Launch Application
```bash
cd OpenBullet-Complete-Replica/OpenBullet-Modern
dotnet run
```

### 2. Load Configuration
- Click **"📂 Load amazonChecker.anom"**
- Select your Amazon validation config file
- Status will show: ✅ Amazon config loaded

### 3. Load Phone Numbers
- Click **"📋 Load Phone Numbers"**  
- Select a text file with phone numbers in format:
  ```
  16479971432:0000
  16479972941:0000
  16479979819:0000
  ```

### 4. Configure Settings
- **Concurrent Bots**: How many Chrome instances (default: 5)
- **Request Timeout**: Chrome timeout in seconds (default: 30)
- **Proxy Simulation**: Enable if desired (optional)

### 5. Start Validation
- Click **"▶️ START VALIDATION"**
- Watch the live statistics update
- All Chrome automation runs invisibly

### 6. Monitor Results
- Real-time statistics in right panel
- Detailed logs in main window
- Key detection breakdown for Amazon responses

---

## 🔍 Understanding the Output

### ✅ Valid Account Indicators
- `"Enter your password"` - Account exists, password prompt shown
- `"Sign-In"` - Amazon sign-in page indicates valid account
- `"ap_password"` - Password field present (account found)

### ❌ Invalid Account Indicators  
- `"We cannot find an account"` - No Amazon account for this number
- `"Incorrect phone number"` - Invalid phone format
- `"ap_ra_email_or_phone"` - Amazon's email/phone validation error

### ⚠️ Blocking/Rate Limiting
- `"AMAZON_BLOCKING_DETECTED"` - Too many requests, need proxies
- `"Robot Check"` - Captcha or verification required

---

## 🆚 Comparison: Console vs Windows Edition

| Feature | Console (Mac/Linux) | Windows Edition |
|---------|-------------------|----------------|
| **UI Type** | Terminal/Console | Desktop Window |
| **Platform** | Cross-platform | Windows Only |
| **Chrome Mode** | May show browser | Always headless |
| **Console Window** | Always visible | Hidden |
| **User Experience** | Text-based | Professional GUI |
| **Statistics** | Text output | Live visual charts |
| **Real-time Updates** | Manual refresh | Automatic |

---

## 🔧 Advanced Configuration

### Chrome Performance Tuning
The Chrome automation is pre-configured for optimal performance:

```csharp
// Already configured in ChromeAutomationService.cs
options.AddArgument("--headless=new");     // Invisible browser
options.AddArgument("--disable-gpu");       // No graphics
options.AddArgument("--disable-images");    // Faster loading  
options.AddArgument("--silent");            // No console output
options.AddArgument("--log-level=3");       // Minimal logging
```

### Concurrent Bot Scaling
- **1-5 bots**: Safe for testing
- **5-20 bots**: Moderate load
- **20+ bots**: High performance mode (use proxies)

### Memory Usage
Each Chrome bot uses ~50-100MB RAM:
- 5 bots ≈ 250-500MB
- 10 bots ≈ 500MB-1GB
- Scale according to your system

---

## 🐛 Troubleshooting

### "Chrome driver not found"
```bash
# Reinstall Chrome driver
dotnet add package Selenium.WebDriver.ChromeDriver --version 129.0.6668.5800
```

### "Console window still appears"  
- Ensure you're using `dotnet run` in the **OpenBullet-Modern** folder
- The project is configured as `WinExe` - this prevents console windows

### "Chrome browser windows are visible"
- This shouldn't happen - Chrome is configured with `--headless=new`
- Check antivirus settings (some block headless mode)

### "Amazon is blocking requests"
- Reduce concurrent bots (set to 1-3)
- Enable proxy simulation
- Add delays between requests

---

## 📁 Project Structure

```
OpenBullet-Complete-Replica/
├── OpenBullet-Console/          # Cross-platform (Mac/Linux)
├── OpenBullet-Modern/           # 🎯 Windows Edition (THIS ONE)
│   ├── MainWindow.xaml          # Desktop UI
│   ├── MainWindow.xaml.cs       # Main application logic  
│   ├── ChromeAutomationService.cs  # Headless Chrome engine
│   └── OpenBullet-Modern.csproj    # Project configuration
├── OpenBullet-Replica/          # Legacy .NET Framework
└── WINDOWS_SETUP_GUIDE.md       # This guide
```

---

## 🎉 Success!

You now have a **professional Windows desktop application** that:
- Runs with `dotnet run` like your Mac console version
- Provides a full GUI interface instead of console
- Performs real Amazon validation with headless Chrome
- **Never shows any browser or console windows**
- Gives you detailed statistics and logging

**No more console windows, no more visible Chrome browsers!** The automation runs completely in the background while you interact with the professional Windows interface.

---

## 🔗 Related Files
- `amazonChecker.anom` - Amazon validation configuration
- Sample wordlists in `OpenBullet-Complete-Replica/` directory  
- Settings saved in `Settings/` folder

**Happy validating! 🎯**
