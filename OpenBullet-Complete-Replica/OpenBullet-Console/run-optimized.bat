@echo off
cls
echo ========================================
echo   OpenBullet OPTIMIZED High Performance
echo ========================================
echo.
echo 🚀 Starting optimized console version...
echo ⚡ Fixes: Runtime blocking, CPU usage, detection breakdown
echo 🎯 Max threads: 10 (prevents Amazon rate limiting)
echo 📁 File I/O: Minimal (performance optimized)
echo.

REM Change to the correct directory
cd /d "%~dp0"

REM Check if .NET is available
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET not found! Please install .NET 9 Runtime
    echo 📥 Download from: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

REM Replace the main program with optimized version temporarily
echo 📝 Backing up original Program.cs...
if exist Program.cs.backup (
    echo ✅ Backup already exists
) else (
    copy Program.cs Program.cs.backup >nul
)

echo 🔄 Switching to optimized program...
copy OptimizedProgram.cs Program.cs >nul

echo 🚀 Running OPTIMIZED OpenBullet Console...
echo.
dotnet run

echo.
echo 🔄 Restoring original Program.cs...
copy Program.cs.backup Program.cs >nul

echo.
echo ✅ Optimized test completed!
echo 📊 Check Results folder for performance reports
pause

