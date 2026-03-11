@echo off

echo ============================
echo Building LobbyHUD Mod
echo ============================

dotnet build -c Release

if %errorlevel% neq 0 (
    echo.
    echo BUILD FAILED
    pause
    exit
)

echo.
echo Build successful!

set DLL=bin\Release\net472\LobbyHUD.dll
set DEST="C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Mods\LobbyHUD.dll"

echo Copying DLL...
copy /Y %DLL% %DEST%

echo.
echo Starting Gorilla Tag...
start "" "C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag.exe"

pause