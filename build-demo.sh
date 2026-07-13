#!/usr/bin/bash
set -e
dotnet publish ./src/QtChungus.Demo/QtChungus.Demo.csproj -o ./
./QtChungus.Demo
rm ./QtChungus.Demo
rm ./QtChungus.Demo.dbg
rm ./QtChungus.Demo.pdb