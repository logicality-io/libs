#!/usr/bin/env bash

set -e

docker run --rm --name proxykit-build \
 -v $PWD:/repo \
 -w /repo \
 -e FEEDZ_LOGICALITY_API_KEY=$FEEDZ_LOGICALITY_API_KEY \
 -e BUILD_NUMBER=$GITHUB_RUN_NUMBER \
 damianh/dotnet-sdks:2 \
 dotnet run -p build/build.csproj -c Release -- "$@"