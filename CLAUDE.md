# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

OpenBullet is an Amazon account validation tool that checks if phone numbers/emails have associated Amazon accounts. The codebase contains multiple implementations:

- **OpenBullet-Complete-Replica**: Modern .NET 9 implementation with console and GUI interfaces
- **OpenBullet-Modern**: Simplified .NET 9 version
- **OpenBullet-Replica**: .NET Framework 4.7.2 version for full DLL compatibility
- **Original Anomaly Version**: Reference implementation with DLL dependencies

## Essential Commands

### Build and Run

**Quick start (OpenBullet-Console):**
```bash
cd OpenBullet-Complete-Replica/OpenBullet-Console
dotnet run                    # Run with GUI interface
dotnet run --console         # Run in console mode
```

**Test with limited numbers:**
```bash
cd OpenBullet-Complete-Replica/OpenBullet-Console
dotnet run test-first-100.bat
```

**Build Windows executable:**
```bash
cd OpenBullet-Complete-Replica/OpenBullet-Console
./publish-windows.bat        # Creates single-file exe in WindowsRelease/
```

**Run optimized version:**
```bash
cd OpenBullet-Complete-Replica/OpenBullet-Console
./run-optimized.bat         # Uses OptimizedProgram.cs temporarily
```

### Development Commands

**Build solution:**
```bash
dotnet build
```

**Run tests (if available):**
```bash
dotnet test
```

**Clean build artifacts:**
```bash
dotnet clean
```

## High-Level Architecture

### Core Components

1. **Validation Engines**
   - `Program.cs:ValidatePhoneNumber()` - Primary HTTP-based validation
   - `CompleteRuriLibEngine.cs` - Loads original RuriLib.dll for compatibility
   - `SeleniumEngine.cs` - Browser automation approach (stub)
   - `OriginalEngine.cs` - Uses Leaf.xNet library via reflection

2. **Configuration System**
   - `.anom` files contain validation logic and patterns
   - Key file: `amazonChecker.anom` - Amazon-specific configuration
   - Parser extracts headers, POST data, success/failure patterns

3. **Validation Flow**
   ```
   Load Config → Parse Patterns → GET Amazon Login → Extract Tokens → 
   POST Phone/Email → Analyze Response → Determine Validity
   ```

4. **UI Architecture (Avalonia)**
   - `MainWindow.xaml` - Modern cross-platform UI
   - `MainWindowViewModel.cs` - MVVM pattern implementation
   - Real-time statistics and progress tracking

### Key Files and Responsibilities

- **Program.cs**: Entry point, main validation logic, console interface
- **CompleteRuriLibEngine.cs**: Attempts to use original OpenBullet DLLs
- **MainWindow.xaml.cs**: GUI initialization and event handling
- **amazonChecker.anom**: Configuration defining Amazon validation patterns

### Important Patterns

1. **Dual Interface**: Application supports both GUI and console modes
2. **Multi-Engine Approach**: Multiple validation strategies with fallbacks
3. **Configuration-Driven**: Behavior defined by .anom config files
4. **Concurrent Processing**: Configurable thread count for parallel validation

### Key Configuration Files

- `amazonChecker.anom`: Main validation configuration
- `sample_numbers.txt`: Test phone numbers for validation
- `Settings/`: JSON configuration templates
- `libs/`: Original OpenBullet DLL dependencies

### Security Considerations

- Application performs account existence checks on Amazon
- Uses anti-detection techniques (headers, cookies, timing)
- Results are saved as HTML files in Results/ directory
- Designed for bulk validation with concurrent threads

## Development Tips

1. **Testing**: Use `sample_numbers.txt` for quick validation tests
2. **Debugging**: HTML responses saved in Results/ folder for analysis
3. **Performance**: Default 100 threads, adjustable via UI or config
4. **Anti-Detection**: Maintain realistic request patterns and timing

## Common Tasks

### Adding New Validation Patterns
Edit the .anom config file and add patterns to [SCRIPT] section:
```
KEYCHECK BanOn "New failure pattern"
KEYCHECK SuccessOn "New success pattern"
```

### Adjusting Thread Count
- GUI: Use slider in settings
- Console: Modify `maxConcurrency` variable in Program.cs

### Debugging Failed Validations
1. Check Results/ folder for HTML responses
2. Look for pattern mismatches in console output
3. Verify config patterns match current Amazon responses