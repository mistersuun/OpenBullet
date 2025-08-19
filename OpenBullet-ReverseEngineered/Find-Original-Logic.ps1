# Find Original OpenBullet Execution Logic
Write-Host "🔍 DISCOVERING ORIGINAL OPENBULLET EXECUTION LOGIC" -ForegroundColor Cyan

try {
    $assembly = [System.Reflection.Assembly]::LoadFrom("$PWD\libs\RuriLib.dll")
    $types = $assembly.GetExportedTypes()
    
    Write-Host "`n🎯 SEARCHING FOR EXECUTION ENGINE..." -ForegroundColor Yellow
    
    # Look for Block-based execution (suggested by BlocksAmount = 1)
    $blockTypes = $types | Where-Object { $_.Name -like "*Block*" }
    Write-Host "`n📦 BLOCK TYPES (Block-based execution):" -ForegroundColor Green
    foreach ($type in $blockTypes) {
        Write-Host "   ✅ $($type.FullName)" -ForegroundColor White
    }
    
    # Look for LoliScript execution engine
    $loliTypes = $types | Where-Object { 
        $_.Name -like "*LoliScript*" -or 
        $_.Name -like "*Script*" -or 
        $_.Name -like "*Parser*" -or
        $_.Name -like "*Interpreter*" -or
        $_.Name -like "*Execute*"
    }
    Write-Host "`n📜 LOLISCRIPT/EXECUTION TYPES:" -ForegroundColor Green
    foreach ($type in $loliTypes) {
        Write-Host "   ✅ $($type.FullName)" -ForegroundColor White
        
        # Check for execution methods
        $methods = $type.GetMethods() | Where-Object { 
            $_.Name -like "*Execute*" -or 
            $_.Name -like "*Process*" -or 
            $_.Name -like "*Run*" -or
            $_.Name -like "*Parse*"
        }
        
        foreach ($method in $methods) {
            $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name)" }) -join ", "
            $static = if ($method.IsStatic) { "STATIC " } else { "" }
            Write-Host "      🚀 ${static}$($method.Name)($params)" -ForegroundColor Yellow
        }
    }
    
    # Look for Runner execution methods in detail
    $runnerTypes = $types | Where-Object { $_.Name -like "*Runner*" }
    Write-Host "`n🏃 RUNNER EXECUTION METHODS:" -ForegroundColor Green
    foreach ($type in $runnerTypes) {
        Write-Host "   ✅ $($type.FullName)" -ForegroundColor White
        
        $methods = $type.GetMethods() | Where-Object { 
            $_.Name -like "*Execute*" -or 
            $_.Name -like "*Process*" -or 
            $_.Name -like "*Run*" -or
            $_.Name -like "*Start*"
        }
        
        foreach ($method in $methods) {
            $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name)" }) -join ", "
            $static = if ($method.IsStatic) { "STATIC " } else { "" }
            Write-Host "      🚀 ${static}$($method.Name)($params)" -ForegroundColor Yellow
        }
    }
    
    # Look for BotData execution methods
    $botDataType = $types | Where-Object { $_.Name -eq "BotData" }
    if ($botDataType) {
        Write-Host "`n🤖 BOTDATA EXECUTION METHODS:" -ForegroundColor Green
        Write-Host "   ✅ $($botDataType.FullName)" -ForegroundColor White
        
        $methods = $botDataType.GetMethods() | Where-Object { 
            $_.Name -like "*Execute*" -or 
            $_.Name -like "*Process*" -or 
            $_.Name -like "*Run*"
        }
        
        foreach ($method in $methods) {
            $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name)" }) -join ", "
            Write-Host "      🚀 $($method.Name)($params)" -ForegroundColor Yellow
        }
    }
    
    # Check for any ProcessScript or similar methods across all types
    Write-Host "`n🔍 SEARCHING FOR SCRIPT PROCESSING METHODS..." -ForegroundColor Cyan
    foreach ($type in $types) {
        $processMethods = $type.GetMethods() | Where-Object { 
            $_.Name -like "*ProcessScript*" -or 
            $_.Name -like "*ExecuteScript*" -or
            $_.Name -like "*RunScript*" -or
            $_.Name -like "*ProcessBlock*"
        }
        
        if ($processMethods.Count -gt 0) {
            Write-Host "   ✅ $($type.FullName)" -ForegroundColor White
            foreach ($method in $processMethods) {
                $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name)" }) -join ", "
                $static = if ($method.IsStatic) { "STATIC " } else { "" }
                Write-Host "      🚀 ${static}$($method.Name)($params)" -ForegroundColor Green
            }
        }
    }
    
    Write-Host "`n🎯 CONCLUSION:" -ForegroundColor Magenta
    Write-Host "Based on BlocksAmount=1, original likely uses BLOCK-based execution" -ForegroundColor Yellow
    Write-Host "Need to find Block classes and execution methods in RuriLib.dll" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

