#!/usr/bin/env bash

set -e

docker build \
 -f build.dockerfile \
 --tag dotnet-libs-build .

docker run --rm --name hosting-build \
 -v /var/run/docker.sock:/var/run/docker.sock \
 -v $PWD/artifacts:/repo/artifacts \
 -v $PWD/.git:/repo/.git \
 -v $PWD/temp:/repo/temp \
 -e FEEDZ_LOGICALITY_API_KEY=$FEEDZ_LOGICALITY_API_KEY \
 -e NUGET_PACKAGES=/repo/temp/nuget-packages \
 -e BUILD_NUMBER=$GITHUB_RUN_NUMBER \
 --network host \
 dotnet-libs-build \
 dotnet run -p build/Build.csproj -c Release -- "$@"