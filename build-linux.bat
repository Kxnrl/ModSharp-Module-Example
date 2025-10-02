@echo off
title Building Sparkle ^& Yunli
rd ".build/gamedata" /S /Q
rd ".build/modules" /S /Q
rd ".build/shared" /S /Q
cls
dotnet publish Kendama/Kendama.csproj -f net9.0 -r linux-x64 --disable-build-servers --no-self-contained -c Release --output ".build/shared/Sparkle.Kendama"
dotnet publish Sparkle/Sparkle.csproj -f net9.0 -r linux-x64 --disable-build-servers --no-self-contained -c Release --output ".build/modules/Sparkle"
dotnet publish YunLi/YunLi.csproj -f net9.0 -r linux-x64 --disable-build-servers --no-self-contained -c Release --output ".build/modules/YunLi"
echo:
echo Build Sparkle ^& Yunli Completed...
echo:
echo Copying GameData...
xcopy "gamedata\*" ".build/gamedata/" /E /I /Y
echo:
echo Copy Configs Completed...
echo:
pause