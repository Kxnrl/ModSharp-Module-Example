@echo off
title Building Sparkle
rd ".build/modules" /S /Q
rd ".build/gamedata" /S /Q
cls
dotnet publish src/Sparkle.csproj -f net9.0 -r linux-x64 --disable-build-servers --no-self-contained -c Release --output ".build/modules/Sparkle"
echo:
echo Build Sparkle Completed...
echo:
echo Copying GameData...
xcopy "gamedata\*" ".build/gamedata/" /E /I /Y
echo:
echo Copy Configs Completed...
echo:
pause