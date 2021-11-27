FROM ghcr.io/logicality-io/docker-images/build-dotnet:latest

COPY . ./repo/

WORKDIR /repo