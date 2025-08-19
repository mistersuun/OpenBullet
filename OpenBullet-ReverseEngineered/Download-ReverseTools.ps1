# Download .NET Reverse Engineering Tools
Write-Host "📥 Downloading .NET Reverse Engineering Tools" -ForegroundColor Cyan

$toolsDir = "$PWD\ReverseTools"
New-Item -ItemType Directory -Path $toolsDir -Force | Out-Null

# Download ILSpy
Write-Host "`n📥 Downloading ILSpy..." -ForegroundColor Green
try {
    $ilspyUrl = "https://github.com/icsharpcode/ILSpy/releases/download/v8.2.1.7535/ILSpy_binaries_8.2.1.7535.zip"
    $ilspyZip = "$toolsDir\ILSpy.zip"
    
    # Use TLS 1.2 for GitHub downloads
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    
    Invoke-WebRequest -Uri $ilspyUrl -OutFile $ilspyZip -UseBasicParsing
    Expand-Archive -Path $ilspyZip -DestinationPath "$toolsDir\ILSpy" -Force
    Remove-Item $ilspyZip
    Write-Host "✅ ILSpy downloaded to: $toolsDir\ILSpy" -ForegroundColor Green
} catch {
    Write-Host "❌ ILSpy download failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Download dnSpy
Write-Host "`n📥 Downloading dnSpy..." -ForegroundColor Green  
try {
    $dnspyUrl = "https://github.com/dnSpy/dnSpy/releases/download/v6.1.8/dnSpy-net-win32.zip"
    $dnspyZip = "$toolsDir\dnSpy.zip"
    
    Invoke-WebRequest -Uri $dnspyUrl -OutFile $dnspyZip -UseBasicParsing
    Expand-Archive -Path $dnspyZip -DestinationPath "$toolsDir\dnSpy" -Force
    Remove-Item $dnspyZip
    Write-Host "✅ dnSpy downloaded to: $toolsDir\dnSpy" -ForegroundColor Green
} catch {
    Write-Host "❌ dnSpy download failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🔍 Creating DLL analysis script..." -ForegroundColor Yellow

# Create ILSpy analysis script
$analysisScript = @"
@echo off
echo 🔍 Analyzing RuriLib.dll with ILSpy...
echo.

set ILSPY_PATH="$toolsDir\ILSpy\ILSpy.exe"
set DLL_PATH="$PWD\libs\RuriLib.dll"

echo Starting ILSpy GUI for interactive analysis...
start `"`" %ILSPY_PATH% %DLL_PATH%

echo.
echo 📋 Instructions:
echo 1. ILSpy should open with RuriLib.dll loaded
echo 2. Expand the namespaces to find:
echo    - RuriLib.LS.BlockParser (Parse method)
echo    - RuriLib.BotData (constructor parameters)
echo    - RuriLib.BlockRequest (Process method)
echo    - RuriLib.BlockKeycheck (Process method)
echo 3. Right-click classes and select "Export to Project" for source code
echo.
pause
"@

$analysisScript | Out-File "$toolsDir\Analyze-RuriLib.bat" -Encoding ASCII

Write-Host "✅ Analysis script created: $toolsDir\Analyze-RuriLib.bat" -ForegroundColor Green

Write-Host "`n🎯 REVERSE ENGINEERING TOOLS READY!" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host "📁 Tools location: $toolsDir" -ForegroundColor White
Write-Host "🔧 Run analysis: $toolsDir\Analyze-RuriLib.bat" -ForegroundColor Yellow
Write-Host "📊 ILSpy GUI: $toolsDir\ILSpy\ILSpy.exe" -ForegroundColor Yellow
Write-Host "🔍 dnSpy: $toolsDir\dnSpy\dnSpy.exe" -ForegroundColor Yellow

