@ECHO OFF

:: Builds the build environment image.
docker build ^
 -f build.dockerfile ^
 --tag dotnet-libs-build .

if errorlevel 1 (
   echo Docker build failed: Exit code is %errorlevel%
   exit /b %errorlevel%
)

docker run --rm -it --name dotnet-libs-build ^
 -v /var/run/docker.sock:/var/run/docker.sock ^
 -v %cd%/artifacts:/repo/artifacts ^
 -v %cd%/.git:/repo/.git ^
 -v %userprofile%\.nuget\packages:/repo/temp/nuget-packages ^
 -e NUGET_PACKAGES=/repo/temp/nuget-packages ^
 -e FEEDZ_LOGICALITY_API_KEY=%FEEDZ_LOGICALITY_API_KEY% ^
 -e BUILD_NUMBER=%GITHUB_RUN_NUMBER% ^
 --network host ^
 dotnet-libs-build ^
 dotnet run -p build/Build.csproj -c Release -- %*

if errorlevel 1 (
  echo Docker build failed: Exit code is %errorlevel%
  exit /b %errorlevel%
)