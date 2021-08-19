#!/bin/sh

rm -rf ./publish


customargs="-c Release /property:PublishTrimmed=True /p:DebugSymbols=false /p:DebugType=None"
dotnet publish ./src/TauStellwerk.Server/ -r linux-x64 -o ./publish/linux-x64 $customargs
dotnet publish ./src/TauStellwerk.Server/ -r linux-arm64 -o ./publish/linux-arm64 $customargs
dotnet publish ./src/TauStellwerk.Server/ -r win-x64 -o ./publish/windows-x64 $customargs
dotnet publish ./src/TauStellwerk.Server/ -r win-arm64 -o ./publish/windows-arm64 $customargs

rm ./publish/*/*.xml
rm ./publish/*/appsettings.Development.json
rm -r ./publish/*/BlazorDebugProxy
rm ./publish/*/web.config