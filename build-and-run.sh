#!/usr/bin/bash
set -e
dotnet publish ./QtChungus -o ./bld -v:diag -p:_QtGlueShared=false
./bld/QtChungus