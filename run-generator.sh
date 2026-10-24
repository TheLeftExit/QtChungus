#!/usr/bin/bash
set -e
cd ./src
dotnet run --project ./QtChungus.Generator/QtChungus.Generator.csproj ./QtChungus/Widgets/Classes ./QtChungus.Cxx/Methods.g.cxx
#dotnet format ./QtChungus
