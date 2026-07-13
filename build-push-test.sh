#!/usr/bin/bash
set -e
dotnet nuget delete --source git.theleftexit.net --non-interactive QtChungus 0.0.1

dotnet pack ./src/QtChungus/QtChungus.csproj -o ./ -v:diag
dotnet nuget push QtChungus.0.0.1.nupkg --source git.theleftexit.net
rm QtChungus.0.0.1.nupkg

dotnet nuget locals all --clear
dotnet run --project ./src/QtChungus.Demo/QtChungus.Demo.csproj -p:UsePackage=true

dotnet publish ./src/QtChungus.Demo/QtChungus.Demo.csproj -p:UsePackage=true -o ./
./QtChungus.Demo
rm ./QtChungus.Demo
rm ./QtChungus.Demo.dbg