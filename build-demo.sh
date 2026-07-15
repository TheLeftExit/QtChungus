#!/usr/bin/bash
set -e
dotnet publish ./src/QtChungus.Demo/QtChungus.Demo.csproj -p:DebugType=None -p:DebugSymbols=false -p:CopyOutputSymbolsToPublishDirectory=false -o ./
./QtChungus.Demo
rm ./QtChungus.Demo