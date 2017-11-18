@echo off
setlocal

cd "%~dp0"

if '%Configuration%' == '' if not '%1' == '' set Configuration=%1
if '%Configuration%' == '' set Configuration=Debug

if exist build\coverage rd /s /q build\coverage
md build\coverage

packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -output:build\coverage\results.xml -target:packages\xunit.runner.console.2.3.1\tools\net452\xunit.console.exe -targetargs:"JeremyAnsel.Media.WavefrontObj.Tests\bin\%Configuration%\JeremyAnsel.Media.WavefrontObj.Tests.dll -noshadow" "-filter:+[JeremyAnsel.Media.WavefrontObj]* -[*.Tests]*" -hideskipped:File;Filter;Attribute -returntargetcode
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%

packages\ReportGenerator.3.0.2\tools\ReportGenerator.exe -reports:build\coverage\results.xml -reporttypes:Html;Badges -targetdir:build\coverage -verbosity:Info
