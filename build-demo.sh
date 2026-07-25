#!/usr/bin/bash
set -e
dotnet clean ./src/QtChungus.Demo/QtChungus.Demo.csproj
dotnet run --project ./src/QtChungus.Demo/QtChungus.Demo.csproj
dotnet publish ./src/QtChungus.Demo/QtChungus.Demo.csproj -o ./bin
./bin/QtChungus.Demo
rm -rf ./bin