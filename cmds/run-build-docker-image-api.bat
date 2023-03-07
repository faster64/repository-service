@echo off

:: Enter build version (tag docker image)
set /p BUILD_VERSION="Enter build version (ex 4.0.1): "

set REPO_DOCKER=docker.citigo.com.vn/kvdev/kv-timesheet-service:%BUILD_VERSION%

CD ../

:: Get Git branch, commitId
for /f "tokens=2" %%I in ('git.exe branch 2^> NUL ^| findstr /b "* "') do set GIT_BRANCH=%%I
for /f %%i in ('git rev-parse --short HEAD') do set GIT_COMMIT=%%i

:: Get date
for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set NOW=%%j
set NOW=%NOW:~0,4%-%NOW:~4,2%-%NOW:~6,2% %NOW:~8,2%:%NOW:~10,2%:%NOW:~12,6%

:: Write file BuidVersion.txt
::echo VERSION: %BUILD_VERSION% - DATE: %NOW% - BRANCH: %GIT_BRANCH% - COMMITID: %GIT_COMMIT%> "./src/User Interface/Api/KiotVietTimeSheet.Api/BuildVersion.txt"

set DOCKER_IMAGE_BUILD_ARG="VERSION: %BUILD_VERSION% - DATE: %NOW% - BRANCH: %GIT_BRANCH% - COMMITID: %GIT_COMMIT%"

:: Build image
docker build -f ".\docker\api\Dockerfile" -t %REPO_DOCKER% --build-arg DOCKER_IMAGE_BUILD_ARG=%DOCKER_IMAGE_BUILD_ARG% .

:: Confirm push image
setlocal
echo ===========Build done===========
echo   + Image: %REPO_DOCKER%
echo   + Build Version: %BUILD_VERSION%
echo   + Branch: %GIT_BRANCH% - CommitId: %GIT_COMMIT%
:PROMPT
set /P AREYOUSURE=Do you want push to server (Y/[N])?
if /I "%AREYOUSURE%" NEQ "Y" goto END
docker push %REPO_DOCKER%
:END
endlocal

pause