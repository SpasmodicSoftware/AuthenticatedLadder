#!/bin/bash
DOTNET_CLI_TELEMETRY_OPTOUT=1
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
set -euo pipefail
#Let's add minicover as a command
cd minicover
dotnet restore

#Let's build the project
cd ..
dotnet restore
dotnet build

#We now instrument minicover to create coverage data
cd minicover
dotnet minicover instrument --workdir ../ --assemblies */bin/**/*.dll --exclude-sources AuthenticatedLadder/Program.cs --exclude-sources AuthenticatedLadder/Startup.cs --sources AuthenticatedLadder/**/*.cs
cd ..
for project in *Tests/*.csproj; do dotnet test --no-build $project; done

#Let's push the results to coveralls
cd minicover
echo "Pushing to coveralls the coverage data"
dotnet minicover coverallsreport --workdir ../ --repo-token "$COVERALLS_TOKEN"
cd ..
