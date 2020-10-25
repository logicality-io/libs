@ECHO OFF

docker run --rm -it --name hosting-build ^
 -v /var/run/docker.sock:/var/run/docker.sock ^
 -v %cd%:/repo ^
 -w /repo ^
 -e FEEDZ_LOGICALITY_API_KEY=%FEEDZ_LOGICALITY_API_KEY% ^
 --network host ^
 -e BUILD_NUMBER=%GITHUB_RUN_NUMBER% ^
 damianh/dotnet-sdks:2 ^
 dotnet run -p build/build.csproj -c Release -- %*

if errorlevel 1 (
  echo Docker build failed: Exit code is %errorlevel%
  exit /b %errorlevel%
)