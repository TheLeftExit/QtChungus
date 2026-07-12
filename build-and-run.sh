#!/usr/bin/bash
set -e
dotnet pack -o ./ -v:diag -p:QtGlueShared=false
dotnet pack -o ./ -v:diag -p:QtGlueShared=true
dotnet nuget push QtChungus.Shared*.nupkg --source git.theleftexit.net
dotnet nuget push QtChungus.Static*.nupkg --source git.theleftexit.net
rm QtChungus.*.nupkg
dotnet nuget locals all --clear
dotnet run ./src/QtChungus.Demo