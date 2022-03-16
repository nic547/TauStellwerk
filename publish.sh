#!/bin/bash
set -e

dotnet=${1:-dotnet}

customargs="-c Release --self-contained /p:DebugSymbols=false /p:DebugType=None  /p:EnableCompressionInSingleFile=true /p:PublishTrimmed=true /p:TrimMode=CopyUsed /p:TreatWarningsAsErrors=false /nowarn:IL2026,IL2111,IL2104,IL2110,IL2091"
rids=("linux-x64" "linux-musl-x64" "linux-arm64" "linux-musl-arm64" "win-x64" "win-arm64")

BLUE='\033[1;34m'
NC='\033[0m' # No Color

rm -rf ./publish
$dotnet clean /p:PublishSingleFile=false -c Release

for rid in ${rids[@]}

do

rm -rf ./src/*/bin
rm -rf ./src/*/obj

echo -e "$BLUE --- Building $rid --- $NC"
$dotnet publish ./src/TauStellwerk.Server/ -r $rid -o ./publish/$rid $customargs

rm ./publish/$rid/*.xml
rm ./publish/$rid/appsettings.Development.json
rm -r ./publish/$rid/BlazorDebugProxy
rm -f ./publish/$rid/web.config

echo -e "$BLUE ---Finsihed building $rid --- $NC"

done

echo -e "$BLUE --- All builds completed! --- $NC"

