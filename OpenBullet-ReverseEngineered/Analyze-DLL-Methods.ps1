# Simple DLL method analysis using .NET reflection (instead of ildasm)
Write-Host "🔍 DLL METHOD ANALYSIS - Alternative to ildasm" -ForegroundColor Cyan

$dllPath = "$PWD\libs\RuriLib.dll"

try {
    Write-Host "📦 Loading: $dllPath" -ForegroundColor Green
    $assembly = [System.Reflection.Assembly]::LoadFrom($dllPath)
    
    Write-Host "Assembly: $($assembly.FullName)" -ForegroundColor Yellow
    Write-Host "Location: $($assembly.Location)" -ForegroundColor Yellow
    
    $types = $assembly.GetExportedTypes()
    Write-Host "Total Types: $($types.Count)" -ForegroundColor Cyan
    
    # Focus on the key types we discovered
    $keyTypes = @(
        "RuriLib.LS.BlockParser",
        "RuriLib.BlockRequest", 
        "RuriLib.BlockKeycheck",
        "RuriLib.BotData",
        "RuriLib.Runner.RunnerViewModel"
    )
    
    foreach ($typeName in $keyTypes) {
        Write-Host "`n" + ("="*60) -ForegroundColor Yellow
        Write-Host "🎯 ANALYZING: $typeName" -ForegroundColor Red
        Write-Host ("="*60) -ForegroundColor Yellow
        
        $type = $types | Where-Object { $_.FullName -eq $typeName }
        
        if ($type) {
            Write-Host "✅ FOUND: $($type.FullName)" -ForegroundColor Green
            
            # Show all methods
            $methods = $type.GetMethods()
            Write-Host "📋 Methods ($($methods.Count)):" -ForegroundColor Cyan
            
            foreach ($method in $methods | Sort-Object Name) {
                $params = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ", "
                $static = if ($method.IsStatic) { "STATIC " } else { "" }
                $visibility = if ($method.IsPublic) { "PUBLIC" } else { "PRIVATE" }
                
                Write-Host "   $visibility ${static}$($method.ReturnType.Name) $($method.Name)($params)" -ForegroundColor White
            }
            
            # Show properties
            $properties = $type.GetProperties()
            if ($properties.Count -gt 0) {
                Write-Host "📋 Properties ($($properties.Count)):" -ForegroundColor Cyan
                foreach ($prop in $properties | Sort-Object Name) {
                    $getter = if ($prop.CanRead) { "get;" } else { "" }
                    $setter = if ($prop.CanWrite) { "set;" } else { "" }
                    Write-Host "   $($prop.PropertyType.Name) $($prop.Name) { $getter $setter }" -ForegroundColor White
                }
            }
            
        } else {
            Write-Host "❌ NOT FOUND: $typeName" -ForegroundColor Red
        }
    }
    
    Write-Host "`n🎯 ANALYSIS COMPLETE!" -ForegroundColor Green
    Write-Host "Now we can use the exact original methods!" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

