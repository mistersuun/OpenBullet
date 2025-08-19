# Deep DLL Analysis - Discover Original OpenBullet Execution Logic
Write-Host "🔍 DEEP ANALYSIS: Original OpenBullet DLL Execution Logic" -ForegroundColor Cyan

$dllsToAnalyze = @(
    "RuriLib.dll",
    "Leaf.xNet.dll", 
    "Extreme.Net.dll",
    "AngleSharp.dll",
    "LiteDB.dll"
)

foreach ($dll in $dllsToAnalyze) {
    Write-Host "`n" + ("="*80) -ForegroundColor Yellow
    Write-Host "🔍 ANALYZING: $dll" -ForegroundColor Green
    Write-Host ("="*80) -ForegroundColor Yellow
    
    try {
        $assembly = [System.Reflection.Assembly]::LoadFrom("$PWD\libs\$dll")
        $types = $assembly.GetExportedTypes()
        
        Write-Host "📋 Total Types: $($types.Count)" -ForegroundColor Cyan
        
        # Look for LoliScript-related types
        $loliScriptTypes = $types | Where-Object { 
            $_.Name -like "*LoliScript*" -or 
            $_.Name -like "*Parser*" -or 
            $_.Name -like "*Interpreter*" -or
            $_.Name -like "*Block*" -or
            $_.Name -like "*Execute*" -or
            $_.Name -like "*Command*"
        }
        
        if ($loliScriptTypes) {
            Write-Host "🎯 LOLISCRIPT/EXECUTION TYPES FOUND:" -ForegroundColor Red
            foreach ($type in $loliScriptTypes) {
                Write-Host "   ✅ $($type.FullName)" -ForegroundColor Green
                
                # Analyze methods for execution patterns
                $methods = $type.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static) |
                           Where-Object { $_.DeclaringType -eq $type }
                
                foreach ($method in $methods) {
                    $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ", "
                    $static = if ($method.IsStatic) { "STATIC " } else { "" }
                    Write-Host "      📝 ${static}$($method.ReturnType.Name) $($method.Name)($params)" -ForegroundColor White
                }
            }
        }
        
        # Look for Runner/Execution types
        $runnerTypes = $types | Where-Object { 
            $_.Name -like "*Runner*" -or 
            $_.Name -like "*Bot*" -or
            $_.Name -like "*Execute*" -or
            $_.Name -like "*Process*"
        }
        
        if ($runnerTypes) {
            Write-Host "`n🏃 RUNNER/EXECUTION TYPES:" -ForegroundColor Magenta
            foreach ($type in $runnerTypes) {
                Write-Host "   ✅ $($type.FullName)" -ForegroundColor Green
                
                # Look for key execution methods
                $executeMethods = $type.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Static) |
                                  Where-Object { $_.Name -like "*Execute*" -or $_.Name -like "*Run*" -or $_.Name -like "*Process*" -or $_.Name -like "*Start*" }
                
                foreach ($method in $executeMethods) {
                    $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ", "
                    $static = if ($method.IsStatic) { "STATIC " } else { "" }
                    Write-Host "      🚀 ${static}$($method.ReturnType.Name) $($method.Name)($params)" -ForegroundColor Yellow
                }
            }
        }
        
        # Look for Config loading methods
        $configTypes = $types | Where-Object { $_.Name -like "*Config*" }
        
        if ($configTypes) {
            Write-Host "`n🔧 CONFIG TYPES:" -ForegroundColor Blue
            foreach ($type in $configTypes) {
                Write-Host "   ✅ $($type.FullName)" -ForegroundColor Green
                
                # Look for loading methods
                $loadMethods = $type.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static) |
                               Where-Object { $_.Name -like "*Load*" -or $_.Name -like "*Parse*" -or $_.Name -like "*From*" }
                
                foreach ($method in $loadMethods) {
                    $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ", "
                    Write-Host "      📁 STATIC $($method.ReturnType.Name) $($method.Name)($params)" -ForegroundColor Cyan
                }
            }
        }
        
    } catch {
        Write-Host "❌ Error analyzing $dll : $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n" + ("="*80) -ForegroundColor Yellow
Write-Host "🎯 SUMMARY: Key Findings for Original Logic Implementation" -ForegroundColor Green
Write-Host ("="*80) -ForegroundColor Yellow

