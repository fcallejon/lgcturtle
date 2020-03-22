#!/bin/bash

echo "Restoring"
dotnet restore
echo "Bulding"
dotnet build -c release
echo "Publishing"
dotnet publish -r linux-x64 -c release  -o ./publish

echo "To The Exit! ->"
./publish/turtle ./small.txt ./toExit.txt
echo "----------"

echo "To a bomb! ->"
./publish/turtle ./small.txt ./toABomb.txt
echo "----------"

echo "Out of bounds! ->"
./publish/turtle ./small.txt ./toOutBounds.txt
echo "----------"

echo "Still in danger! ->"
./publish/turtle ./small.txt ./notOut.txt
echo "----------"

echo "Circle! ->"
./publish/turtle ./small.txt ./circle.txt
echo "----------"

echo "No Edge Start Win! ->"
./publish/turtle ./small-NoEdgeStart.txt ./noEdgeStart-win.txt
echo "----------"

rm -rf ./obj
rm -rf ./bin
rm -rf ./publish
