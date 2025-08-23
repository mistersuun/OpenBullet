# OpenBullet-Copy Project Structure

## 📁 Organized File Structure (Following Original TestBot Pattern)

### 🎯 **Main Executable Location:**
```
OpenBullet-Copy/bin/Release/net472/OpenBullet-Copy.exe
```

### 📂 **Folder Organization:**

```
OpenBullet-Copy/
├── 🏗️  PROJECT ROOT
├── bin/Release/net472/           # Built application
│   ├── OpenBullet-Copy.exe       # Main executable
│   ├── *.dll                     # All OpenBullet DLLs
│   ├── amazonChecker.anom        # Configuration file
│   │
│   ├── 🔊 Sounds/                # Sound effects (like Anomaly)
│   │   ├── rifle_reload.wav      # Start validation sound
│   │   └── rifle_hit.wav         # Complete validation sound
│   │
│   ├── 📄 Wordlists/             # Test phone numbers
│   │   ├── mixed_test_numbers.txt
│   │   ├── test_valid_numbers.txt
│   │   └── test_invalid_numbers.txt
│   │
│   ├── 💻 x64/                   # Native libraries (64-bit)
│   │   ├── liblept*.dll
│   │   ├── libtesseract*.dll
│   │   └── *.so, *.dylib (Linux/Mac)
│   │
│   ├── 🖥️  x86/                   # Native libraries (32-bit)
│   │   ├── liblept*.dll
│   │   └── libtesseract*.dll
│   │
│   └── 📊 Results/               # Auto-created during validation
│       └── OpenBullet_Results_YYYYMMDD_HHMMSS/
│           ├── HTML_Responses/   # Saved HTML files
│           ├── test.log          # Validation log
│           └── results_summary.txt
│
├── 🛠️  SOURCE CODE
├── TestBotEngine.cs              # EXACT TestBot validation logic
├── SimpleForm.cs                 # Modern UI interface
├── Program.cs                    # Application entry point
├── OpenBullet-Copy.csproj        # Project configuration
│
├── ⚙️  CONFIGURATION
├── Settings/
│   └── UISettings.json           # UI preferences
│
├── 📁 SOURCE ASSETS (copied to bin during build)
├── Sounds/                       # Source sound files
├── Wordlists/                    # Source wordlists
├── x64/ & x86/                   # Source native libraries
├── Configs/                      # Future configs storage
├── Screenshots/                  # Future screenshots storage
│
└── 📖 DOCUMENTATION
    ├── README.md                 # Main documentation
    └── STRUCTURE.md              # This file
```

## 🚀 **How Files Are Organized:**

### 1. **Sounds** (Following Anomaly Pattern)
- **Source**: `TestBot-UI/Sounds/`
- **Runtime**: `bin/Release/net472/Sounds/`
- **Files**: `rifle_reload.wav`, `rifle_hit.wav`

### 2. **Wordlists** (Organized for Easy Access)
- **Source**: `TestBot-UI/Wordlists/`
- **Runtime**: `bin/Release/net472/Wordlists/`
- **File Picker**: Opens to Wordlists folder by default

### 3. **Native Libraries** (Platform-Specific)
- **x64**: For 64-bit systems (Windows, Linux, Mac)
- **x86**: For 32-bit systems (Windows only)
- **Auto-copied**: During build process

### 4. **Results** (Auto-Generated)
- **Location**: `bin/Release/net472/Results/`
- **Created**: When validation starts
- **Contains**: HTML responses, logs, summaries

## 🔧 **Build Process:**

The `.csproj` file automatically:
1. **Copies DLLs** from Anomaly version `bin/` folder
2. **Organizes sounds** into `Sounds/` subfolder
3. **Organizes wordlists** into `Wordlists/` subfolder
4. **Copies native libs** to `x64/` and `x86/` subfolders
5. **Maintains structure** identical to original OpenBullet

## ✅ **Benefits of This Structure:**

1. **Familiar Layout**: Matches original TestBot and Anomaly versions
2. **Clean Organization**: Files grouped by purpose
3. **Easy Navigation**: Default paths point to correct folders
4. **Portable**: Can copy entire `bin/Release/net472/` folder
5. **Maintainable**: Clear separation of concerns

## 🎯 **Usage:**

```bash
# Run the application
cd C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-Copy\bin\Release\net472
OpenBullet-Copy.exe

# Or run with dotnet (RECOMMENDED)
cd C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-Copy
dotnet run

# Load wordlists (auto-opens Wordlists folder)
Click "📁 Load Wordlist" → Wordlists folder opens automatically

# Sound effects (auto-loaded from Sounds folder)
rifle_reload.wav plays on start
rifle_hit.wav plays on completion

# Results (auto-saved to Results folder)
HTML responses and logs saved automatically
```

This structure ensures OpenBullet Copy behaves exactly like the original TestBot while providing a clean, organized interface!