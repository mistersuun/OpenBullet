# OpenBullet Anomaly - Windows EXE Publisher (PowerShell)
# This script builds a single-file Windows executable with error handling

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " OpenBullet Anomaly - Windows EXE Build" -ForegroundColor Cyan  
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    # Clean previous builds
    Write-Host "🗑️ Cleaning previous builds..." -ForegroundColor Yellow
    if (Test-Path "bin\Release\net9.0\win-x64\publish") {
        Remove-Item "bin\Release\net9.0\win-x64\publish" -Recurse -Force
    }
    if (Test-Path "WindowsRelease") {
        Remove-Item "WindowsRelease" -Recurse -Force
    }

    # Restore packages
    Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
    $restoreResult = dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "Package restore failed"
    }

    # Build and publish
    Write-Host "🔨 Building Windows executable..." -ForegroundColor Green
    $publishArgs = @(
        "publish",
        "-c", "Release",
        "-r", "win-x64",
        "--self-contained", "true",
        "-p:PublishSingleFile=true",
        "-p:IncludeNativeLibrariesForSelfExtract=true", 
        "-p:IncludeAllContentForSelfExtract=true",
        "-p:EnableCompressionInSingleFile=true",
        "-p:PublishReadyToRun=true",
        "-p:PublishTrimmed=false",
        "-p:DebugType=none",
        "-p:DebugSymbols=false",
        "--output", "WindowsRelease"
    )
    
    $buildResult = & dotnet @publishArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }

    # Copy essential files
    Write-Host "📁 Copying essential files..." -ForegroundColor Yellow
    
    # Create directories
    New-Item -ItemType Directory -Force -Path "WindowsRelease\libs" | Out-Null
    New-Item -ItemType Directory -Force -Path "WindowsRelease\x64" | Out-Null
    New-Item -ItemType Directory -Force -Path "WindowsRelease\x86" | Out-Null

    # Copy DLL files
    if (Test-Path "..\libs\*.dll") {
        Copy-Item "..\libs\*.dll" "WindowsRelease\libs\" -Force
    }
    if (Test-Path "..\x64\*.dll") {
        Copy-Item "..\x64\*.dll" "WindowsRelease\x64\" -Force
    }
    if (Test-Path "..\x86\*.dll") {
        Copy-Item "..\x86\*.dll" "WindowsRelease\x86\" -Force
    }

    # Copy sample files
    if (Test-Path "..\amazonChecker.anom") {
        Copy-Item "..\amazonChecker.anom" "WindowsRelease\" -Force
    }
    if (Test-Path "..\sample_numbers.txt") {
        Copy-Item "..\sample_numbers.txt" "WindowsRelease\" -Force
    }

    # Create helper scripts
    Write-Host "📝 Creating helper scripts..." -ForegroundColor Yellow

    # Normal run script
    @"
@echo off
echo Starting OpenBullet Anomaly - Windows Edition...
OpenBullet-Console.exe
pause
"@ | Out-File -FilePath "WindowsRelease\Run-OpenBullet.bat" -Encoding ASCII

    # Console mode script  
    @"
@echo off
echo Starting OpenBullet Anomaly - Console Mode...
OpenBullet-Console.exe --console
pause
"@ | Out-File -FilePath "WindowsRelease\Run-Console-Mode.bat" -Encoding ASCII

    # Create comprehensive README
    Write-Host "📋 Creating README file..." -ForegroundColor Yellow
    $readme = @'
OpenBullet Anomaly - Windows Edition
=====================================

🎯 QUICK START:
1. Double-click OpenBullet-Console.exe
2. The application will open with the professional UI
3. Load amazonChecker.anom config file
4. Load phone numbers from sample_numbers.txt
5. Start validation!

⚡ ALTERNATIVE MODES:
- Double-click Run-OpenBullet.bat for guided startup
- Double-click Run-Console-Mode.bat for console mode  
- Run with --console flag for command-line interface

🔍 ERROR LOGGING:
If the app fails to start, error details are logged to:
%LOCALAPPDATA%\OpenBullet-Anomaly\

🖥️ FEATURES:
✅ Professional Avalonia UI with dark theme
✅ Real Amazon account validation
✅ Selenium WebDriver automation (headless)
✅ Multi-threaded concurrent validation
✅ Real-time statistics and progress tracking
✅ Export results to files
✅ Original OpenBullet DLL integration
✅ Advanced proxy support
✅ Comprehensive error logging

📁 FILES INCLUDED:
- OpenBullet-Console.exe: Main application (self-contained executable)
- amazonChecker.anom: Amazon validation configuration
- sample_numbers.txt: Sample phone numbers for testing
- libs\: Required DLL files for advanced features
- x64\, x86\: Native libraries for different architectures
- Run-*.bat: Helper scripts for different modes

🔧 SYSTEM REQUIREMENTS:
- Windows 10/11 (x64)
- No additional software required (self-contained)
- Internet connection for Amazon validation
- Chrome/Chromium for Selenium automation (optional)

🛡️ SECURITY:
- No data collection or external communication
- All validation data stays on your machine
- Error logs contain no sensitive information
- Open source and transparent

💡 TROUBLESHOOTING:
- If Chrome automation fails: Install Google Chrome
- If permission errors: Run as Administrator
- If startup fails: Check error logs in %LOCALAPPDATA%\OpenBullet-Anomaly\
- If validation blocked: Use proxy settings in application
- For console mode: Use --console flag or Run-Console-Mode.bat

📞 SUPPORT:
Check error logs first, then review the technical details in the Debug tab
of the application for detailed validation information.

Built: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
Version: 2.0.0.0
Platform: Windows x64
Runtime: .NET 9
'@
    $readme | Out-File -FilePath "WindowsRelease\README.txt" -Encoding UTF8

    # Success summary
    Write-Host ""
    Write-Host "✅ BUILD COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📁 Windows executable location:" -ForegroundColor Cyan
    Write-Host "   WindowsRelease\OpenBullet-Console.exe"
    Write-Host ""
    
    # File size info
    $exeFile = Get-Item "WindowsRelease\OpenBullet-Console.exe"
    Write-Host "📊 File size:" -ForegroundColor Cyan
    Write-Host "   $($exeFile.Length) bytes ($([math]::Round($exeFile.Length/1MB, 1)) MB)"
    Write-Host ""
    
    Write-Host "🎯 You can now:" -ForegroundColor Green
    Write-Host "   1. Double-click OpenBullet-Console.exe to run"
    Write-Host "   2. Copy the entire WindowsRelease folder to any Windows PC"  
    Write-Host "   3. No installation required - just run the EXE!"
    Write-Host ""
    
    # List all files created
    Write-Host "📋 Files created:" -ForegroundColor Yellow
    Get-ChildItem "WindowsRelease" -Recurse | ForEach-Object {
        if ($_.PSIsContainer) {
            Write-Host "   📁 $($_.FullName.Replace((Get-Location), '.'))" -ForegroundColor Blue
        } else {
            Write-Host "   📄 $($_.FullName.Replace((Get-Location), '.')) ($($_.Length) bytes)" -ForegroundColor Gray
        }
    }

    Write-Host ""
    Write-Host "🚀 Ready to distribute!" -ForegroundColor Green

} catch {
    Write-Host ""
    Write-Host "❌ BUILD FAILED!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "🔍 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure .NET 9 SDK is installed"
    Write-Host "2. Check if all packages can be restored"
    Write-Host "3. Verify you have write permissions"
    Write-Host "4. Try running PowerShell as Administrator"
    exit 1
}

Write-Host ""
Write-Host "Press any key to continue..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") | Out-Null
