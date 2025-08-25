@echo off
set LOCALPLAY_ARGS_P1=--width 1240 --height 600
set LOCALPLAY_ARGS_P2=--width 1240 --height 600
set UNITY_ARGS=-screen-fullscreen 0
set BEPINEX="C:/Users/NKvD Officer Walthze/AppData/Roaming/Thunderstore Mod Manager/DataFolder/MageArena/profiles/DEVELOPMENT/BepInEx/core/BepInEx.Preloader.dll"
start "A" /B "N:\SteamLibrary\steamapps\common\Mage Arena\MageArena.exe" %UNITY_ARGS% %LOCALPLAY_ARGS_P1% --doorstop-enable true --doorstop-target %BEPINEX%
::timeout /T 3
::start "B" /B "N:\SteamLibrary\steamapps\common\Mage Arena\MageArena.exe" %UNITY_ARGS% %LOCALPLAY_ARGS_P2% --doorstop-enable true --doorstop-target %BEPINEX%
