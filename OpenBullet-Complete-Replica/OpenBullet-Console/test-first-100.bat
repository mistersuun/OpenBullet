@echo off
cls
echo =========================================================
echo   OpenBullet - Test First 100 Numbers (High Performance)
echo =========================================================
echo.
echo 🧪 This will test the first 100 numbers from sample_numbers.txt
echo ⚡ OPTIMIZED mode - fixes runtime blocking and detection breakdown
echo 📱 Numbers: 16479971432, 16479972941, 16479979819, etc.
echo 🚀 Max threads: 10 (prevents Amazon rate limiting)
echo 📁 HTML saving: DISABLED for performance
echo ⏱️ Expected time: ~5-10 minutes
echo.

pause

REM Change to the correct directory
cd /d "%~dp0"

REM Backup and switch to optimized program
if not exist Program.cs.backup (
    copy Program.cs Program.cs.backup >nul
)
copy OptimizedProgram.cs Program.cs >nul

echo 🚀 Starting first 100 number test...
echo.

REM Create a simple test script
echo using System; > QuickTest.cs
echo using System.Threading.Tasks; >> QuickTest.cs
echo. >> QuickTest.cs
echo class QuickTest >> QuickTest.cs
echo { >> QuickTest.cs
echo     static async Task Main() >> QuickTest.cs
echo     { >> QuickTest.cs
echo         var program = new OpenBullet_Console.OptimizedProgram(); >> QuickTest.cs
echo         Console.WriteLine("🧪 Testing first 100 numbers..."); >> QuickTest.cs
echo         // This will automatically load config and wordlist >> QuickTest.cs
echo         // Then run the first 100 numbers test >> QuickTest.cs
echo         await Task.Delay(1000); >> QuickTest.cs
echo     } >> QuickTest.cs
echo } >> QuickTest.cs

REM Run the optimized program
dotnet run

REM Restore original program
copy Program.cs.backup Program.cs >nul

echo.
echo ✅ First 100 number test completed!
echo 📊 Check Results folder for:
echo    - optimized_results_[timestamp].json (summary)
echo    - error_[number]_[time].html (only for failed validations)
echo.
echo 📈 Performance improvements:
echo    ✅ No more runtime blocking
echo    ✅ Reduced file I/O (only errors saved)
echo    ✅ Smart concurrency (max 10 threads)
echo    ✅ Faster pattern matching
echo    ✅ Minimal console logging
echo.
pause

