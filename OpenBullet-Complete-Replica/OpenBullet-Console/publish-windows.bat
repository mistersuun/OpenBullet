@echo off
REM OpenBullet Anomaly - Windows EXE Publisher
REM This script builds a single-file Windows executable

echo ========================================
echo  OpenBullet Anomaly - Windows EXE Build
echo ========================================
echo.

REM Clean previous builds
echo 🗑️ Cleaning previous builds...
if exist "bin\Release\net9.0\win-x64\publish" rmdir /s /q "bin\Release\net9.0\win-x64\publish"
if exist "WindowsRelease" rmdir /s /q "WindowsRelease"

REM Restore packages
echo 📦 Restoring packages...
dotnet restore
if errorlevel 1 (
    echo ❌ Package restore failed!
    pause
    exit /b 1
)

REM Build and publish
echo 🔨 Building Windows executable...
dotnet publish -c Release -r win-x64 --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:IncludeAllContentForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:PublishReadyToRun=true ^
    -p:PublishTrimmed=false ^
    -p:DebugType=none ^
    -p:DebugSymbols=false ^
    --output WindowsRelease

if errorlevel 1 (
    echo ❌ Build failed!
    pause
    exit /b 1
)

REM Copy essential files
echo 📁 Copying essential files...
mkdir "WindowsRelease\libs" 2>nul
copy "..\libs\*.dll" "WindowsRelease\libs\" >nul 2>&1
mkdir "WindowsRelease\x64" 2>nul
copy "..\x64\*.dll" "WindowsRelease\x64\" >nul 2>&1
mkdir "WindowsRelease\x86" 2>nul
copy "..\x86\*.dll" "WindowsRelease\x86\" >nul 2>&1

REM Copy sample files
copy "..\amazonChecker.anom" "WindowsRelease\" >nul 2>&1
copy "..\sample_numbers.txt" "WindowsRelease\" >nul 2>&1

REM Create run scripts
echo Creating helper scripts...

REM Normal run script
echo @echo off > "WindowsRelease\Run-OpenBullet.bat"
echo echo Starting OpenBullet Anomaly - Windows Edition... >> "WindowsRelease\Run-OpenBullet.bat"
echo OpenBullet-Console.exe >> "WindowsRelease\Run-OpenBullet.bat"
echo pause >> "WindowsRelease\Run-OpenBullet.bat"

REM Console mode script
echo @echo off > "WindowsRelease\Run-Console-Mode.bat"
echo echo Starting OpenBullet Anomaly - Console Mode... >> "WindowsRelease\Run-Console-Mode.bat"
echo OpenBullet-Console.exe --console >> "WindowsRelease\Run-Console-Mode.bat"
echo pause >> "WindowsRelease\Run-Console-Mode.bat"

REM Create README
echo Creating README file...
echo OpenBullet Anomaly - Windows Edition > "WindowsRelease\README.txt"
echo ===================================== >> "WindowsRelease\README.txt"
echo. >> "WindowsRelease\README.txt"
echo 🎯 QUICK START: >> "WindowsRelease\README.txt"
echo 1. Double-click OpenBullet-Console.exe >> "WindowsRelease\README.txt"
echo 2. The application will open with the professional UI >> "WindowsRelease\README.txt"
echo 3. Load amazonChecker.anom config file >> "WindowsRelease\README.txt"
echo 4. Load phone numbers from sample_numbers.txt >> "WindowsRelease\README.txt"
echo 5. Start validation! >> "WindowsRelease\README.txt"
echo. >> "WindowsRelease\README.txt"
echo ⚡ ALTERNATIVE MODES: >> "WindowsRelease\README.txt"
echo - Double-click Run-OpenBullet.bat for guided startup >> "WindowsRelease\README.txt"
echo - Double-click Run-Console-Mode.bat for console mode >> "WindowsRelease\README.txt"
echo - Run with --console flag for command-line interface >> "WindowsRelease\README.txt"
echo. >> "WindowsRelease\README.txt"
echo 🔍 ERROR LOGGING: >> "WindowsRelease\README.txt"
echo If the app fails to start, error details are logged to: >> "WindowsRelease\README.txt"
echo %LOCALAPPDATA%\OpenBullet-Anomaly\ >> "WindowsRelease\README.txt"
echo. >> "WindowsRelease\README.txt"
echo 📁 Files: >> "WindowsRelease\README.txt"
echo - OpenBullet-Console.exe: Main application >> "WindowsRelease\README.txt"
echo - amazonChecker.anom: Amazon validation config >> "WindowsRelease\README.txt"
echo - sample_numbers.txt: Sample phone numbers >> "WindowsRelease\README.txt"
echo - libs\: Required DLL files >> "WindowsRelease\README.txt"
echo - x64\, x86\: Native libraries >> "WindowsRelease\README.txt"

echo.
echo ✅ BUILD COMPLETED SUCCESSFULLY!
echo.
echo 📁 Windows executable location:
echo    WindowsRelease\OpenBullet-Console.exe
echo.
echo 📊 File size:
for %%I in ("WindowsRelease\OpenBullet-Console.exe") do echo    %%~zI bytes (%%~zI bytes)
echo.
echo 🎯 You can now:
echo    1. Double-click OpenBullet-Console.exe to run
echo    2. Copy the entire WindowsRelease folder to any Windows PC
echo    3. No installation required - just run the EXE!
echo.
pause
