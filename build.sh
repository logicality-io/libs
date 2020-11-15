#!/usr/bin/env bash

set -e

docker run --rm --name hosting-build \
 -v /var/run/docker.sock:/var/run/docker.sock \
 -v $PWD:/repo \
 -w /repo \
 -e FEEDZ_LOGICALITY_API_KEY=$FEEDZ_LOGICALITY_API_KEY \
 -e BUILD_NUMBER=$GITHUB_RUN_NUMBER \
 --network host \
 damianh/dotnet-sdks:3 \
 dotnet run -p build/Build.csproj -c Release -- "$@"