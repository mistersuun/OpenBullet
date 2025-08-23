# 🎯 OpenBullet Copy - Amazon Validator

**A modern UI interface for the TestBot using EXACT validation logic from TestBot.cs**

## ✅ Features

- **EXACT same validation logic** as the original working TestBot.cs
- Uses **original Anomaly DLLs** and libraries (Leaf.xNet, RuriLib, etc.)
- Real-time statistics with detection patterns
- Sound effects: rifle_reload.wav (start) + rifle_hit.wav (complete)
- High-performance concurrent validation (1-200 threads)
- Results export to CSV format
- HTML response saving for debugging
- Dark theme UI with modern design

## 🚀 How to Run

### Option 1: Run the built executable
```bash
cd C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-Copy\bin\Release\net472
OpenBullet-Copy.exe
```

### Option 2: Run from source (RECOMMENDED)
```bash
cd C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-Copy
dotnet run
```

### Option 3: Build and run
```bash
cd C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-Copy
dotnet build
dotnet run
```

## 📋 Quick Start Guide

1. **Launch the application** using one of the methods above
2. **Load a wordlist** by clicking "📁 Load Wordlist" button
   - Use the included test files: `mixed_test_numbers.txt`, `test_valid_numbers.txt`, `test_invalid_numbers.txt`
3. **Configure settings**:
   - Set thread count (1-200, default: 10)
   - Enable/disable sound effects
   - Enable/disable HTML response saving
4. **Start validation** by clicking "▶️ Start Validation"
5. **Monitor progress** in real-time with live statistics
6. **Export results** when complete using "💾 Export" button

## 🧪 Test Single Number

Use the "🧪 Test Single" button to quickly test individual phone numbers:
- Default test number: `11111111111` (should show OTP challenge = VALID)
- Try: `15142955315`, `15145692379`, `15149772071` (known valid numbers)
- Try: `0000000000`, `1234567890` (should be invalid)

## 📊 Detection Patterns

The UI displays exactly the same detection patterns as TestBot:
- **✅ PasswordField**: Valid account, password required
- **✅ OTPChallenge**: Valid account, 2FA/OTP required  
- **❌ ErrorMessage**: Invalid account, error messages shown
- **❌ CreateAccount**: No account exists, redirected to create
- **🚫 Captcha**: Blocked by Amazon bot detection
- **❓ Unknown**: Could not determine from response

## 🔊 Sound Effects

The application uses the same sound files as the Anomaly version:
- `rifle_reload.wav`: Played when validation starts
- `rifle_hit.wav`: Played when validation completes
- System beeps as fallback if sound files not found

## ⚙️ Configuration

### Thread Settings
- **1-200 threads**: Configure concurrent validation threads
- **Recommended**: 10-50 threads for optimal performance
- **Higher threads**: Faster validation but may trigger rate limits

### HTML Response Saving
- Enable to save GET/POST responses for debugging
- Files saved to `TestResults_UI_YYYYMMDD_HHMMSS/HTML_Responses/`
- Useful for analyzing unknown results

## 📁 File Structure

```
OpenBullet-Copy/
├── bin/Release/net472/
│   ├── OpenBullet-Copy.exe     # Built executable
│   ├── Sounds/                 # Sound effects folder
│   │   ├── rifle_reload.wav    # Start sound effect
│   │   └── rifle_hit.wav       # Complete sound effect
│   ├── Wordlists/              # Test wordlists folder
│   │   ├── mixed_test_numbers.txt
│   │   ├── test_valid_numbers.txt
│   │   └── test_invalid_numbers.txt
│   ├── x64/                    # Native libraries (64-bit)
│   │   └── *.dll, *.so, *.dylib
│   ├── x86/                    # Native libraries (32-bit)
│   │   └── *.dll
│   ├── Results/                # Validation results (auto-created)
│   └── *.dll                   # OpenBullet DLLs
├── Source Files/
│   ├── TestBotEngine.cs        # EXACT TestBot validation logic
│   ├── SimpleForm.cs           # Modern UI interface
│   └── Program.cs              # Application entry point
├── Settings/
│   └── UISettings.json         # UI configuration
├── Sounds/                     # Source sound files
├── Wordlists/                  # Source wordlists
├── x64/ & x86/                 # Source native libraries
└── Configs/                    # Future config storage
```

## 🔧 Technical Details

### Validation Engine
- **TestBotEngine.cs**: Contains the EXACT same validation logic as TestBot.cs
- **Leaf.xNet HTTP requests**: Same library and configuration as original TestBot
- **Form token extraction**: Same appActionToken, workflowState, prevRID logic
- **Response analysis**: Same regex patterns and detection logic
- **Amazon URL**: Same signin URL and POST endpoint

### Dependencies
Uses the same DLL files as the original TestBot:
- RuriLib.dll
- Leaf.xNet.dll
- Extreme.Net.dll
- AngleSharp.dll
- Newtonsoft.Json.dll
- And all other Anomaly version DLLs

### Results Export
CSV format with columns:
- PhoneNumber
- Status (VALID/INVALID/BLOCKED/UNKNOWN)
- Reason (detailed explanation)
- DetectionPattern (PasswordField/OTPChallenge/etc.)
- Timestamp
- ResponseTimeMs

## 🐛 Troubleshooting

### Application won't start
- Ensure .NET Framework 4.7.2 is installed
- Check that all DLL files are present in the Anomaly version folder
- Run from command line to see error messages

### No results showing
- Check if wordlist file was loaded correctly
- Verify internet connection
- Check if Amazon is blocking requests (try single test first)

### Sound not working
- Ensure rifle_reload.wav and rifle_hit.wav are in the Sounds/ folder
- Application will fall back to system beeps if sound files missing
- Check that the proper folder structure is maintained

### High BLOCKED rate
- Reduce thread count to avoid rate limiting
- Add delays between requests
- Check if your IP is temporarily blocked by Amazon

## 📈 Performance Tips

1. **Start with low threads** (5-10) and increase gradually
2. **Monitor CPM** (Checks Per Minute) in real-time statistics
3. **Use test numbers first** to verify functionality
4. **Save HTML responses** when debugging unknown results
5. **Export results regularly** for backup

## ✅ Verification

To verify the UI is using exact TestBot logic:
1. Test with `11111111111` - should show "OTP challenge" and status VALID
2. Compare results with original TestBot.exe output
3. Check HTML response files match between TestBot and TestBot-UI
4. Verify detection patterns match exactly

---

**🎯 OpenBullet Copy uses the EXACT same validation logic as the working TestBot.cs - no modifications to the core validation engine!**