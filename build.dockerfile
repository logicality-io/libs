FROM ghcr.io/logicality-io/docker-images/build-dotnet:latest

ARG GitHubToken
ENV GITHUB_TOKEN=${GitHubToken}

RUN dotnet nuget add source --name logicality-github --username github \
    --password $GITHUB_TOKEN --store-password-in-clear-text \
    https://nuget.pkg.github.com/logicality-io/index.json

RUN echo $GITHUB_TOKEN | docker login ghcr.io -u github --password-stdin

COPY . ./repo/

WORKDIR /repo