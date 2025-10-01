@echo off
title Building Sparkle & Yunli
rd ".build/modules" /S /Q
rd ".build/gamedata" /S /Q
cls
dotnet publish Sparkle/Sparkle.csproj -f net9.0 -r linux-x64 --disable-build-servers --no-self-contained -c Release --output ".build/modules/Sparkle"
dotnet publish YunLi/YunLi.csproj -f net9.0 -r linux-x64 --disable-build-servers --no-self-contained -c Release --output ".build/modules/YunLi"
echo:
echo Build Sparkle & Yunli Completed...
echo:
echo Copying GameData...
xcopy "gamedata\*" ".build/gamedata/" /E /I /Y
echo:
echo Copy Configs Completed...
echo:
pause