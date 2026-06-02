#!/usr/bin/bash
set -e
cd ./QtGlue
cmake --build ../bld
cd ../QtChungus
dotnet publish -o ../bld -v:diag
cd ../bld
./QtChungus