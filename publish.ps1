param($dotnet='dotnet')

$customargs="-c Release --self-contained /p:DebugSymbols=false /p:DebugType=None  /p:EnableCompressionInSingleFile=true"
$rids="linux-x64","linux-musl-x64","linux-arm64","linux-musl-arm64","win-x64","win-arm64"

if (Test-Path ./publish){
    Remove-Item ./publish -Recurse -Force
}
Invoke-Expression "$dotnet clean /p:PublishSingleFile=false -c Release"

foreach ($rid in $rids){
    Write-Host "--- Building $rid ---" -ForegroundColor Blue
    Invoke-Expression "$dotnet publish ./src/TauStellwerk.Server/ -r $rid -o ./publish/$rid $customargs"

    Remove-Item ./publish/$rid/*.xml
    Remove-Item ./publish/$rid/appsettings.Development.json
    Remove-Item  ./publish/$rid/BlazorDebugProxy -Recurse
    Remove-Item ./publish/$rid/web.config -ea ig

    Write-Host "---Finsihed building $rid ---" -ForegroundColor Blue
}

Write-Host "--- All builds completed! ---" -ForegroundColor Blue