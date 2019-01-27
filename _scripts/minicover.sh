#!/bin/bash
cd minicover
dotnet restore
cd ..
dotnet restore
dotnet build
cd minicover
dotnet minicover instrument --workdir ../ --assemblies */bin/**/*.dll --sources AuthenticatedLadder/**/*.cs || exit 1
cd ..
for project in *Tests/*.csproj; do dotnet test --no-build $project || exit 2; done
cd minicover
dotnet minicover coverallsreport --workdir ../ || exit 3
cd ..