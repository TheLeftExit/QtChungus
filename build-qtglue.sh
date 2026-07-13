#!/usr/bin/bash
set -e

cd ./src/QtGlue

rm -rf ./bin
mkdir ./bin

rm -rf ./build
cmake -S ./ -B ./build -D QTGLUE_SHARED=OFF
cmake --build ./build
mv ./build/libQtGlue.a ./bin/libQtGlue.a

rm -rf ./build
cmake -S ./ -B ./build -D QTGLUE_SHARED=ON
cmake --build ./build
mv ./build/libQtGlue.so ./bin/libQtGlue.so