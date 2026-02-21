@echo off
setlocal

cd "%~dp0"

echo Test
echo Configuration
echo %Configuration%
echo CONFIGURATION
echo %CONFIGURATION%

if '%Configuration%' == '' if not '%1' == '' set Configuration=%1
if '%Configuration%' == '' set Configuration=Debug

dotnet tool update dotnet-reportgenerator-globaltool --tool-path packages

if exist bld\coverage rd /s /q bld\coverage
md bld\coverage

if exist bld\TestResults rd /s /q bld\TestResults

dotnet test --no-build --coverage --coverage-output-format cobertura --results-directory bld\TestResults
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%

setlocal EnableDelayedExpansion
cd "%~dp0"
packages\reportgenerator -reports:"bld\TestResults\*cobertura.xml" -reporttypes:Html;Badges -targetdir:bld\coverage -verbosity:Info
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%
