#!/usr/bin/bash
set -e
rm -rf ./bld
cmake -S QtGlue -B bld
cd ./QtGlue
cmake --build ../bld
cd ../QtChungus
dotnet publish -o ../bld -v:diag
cd ../bld
./QtChungus