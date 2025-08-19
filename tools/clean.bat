:: This is to clean up your DLL build files. usually not something that would be necessary.
:: has to be called from the primary folder.
@echo off
call settings.bat
:: Ensure a clean slate
del %MOD_NAME%-%MOD_VERSION%.zip
rmdir /s /q build
cd src
rmdir /s /q bin
rmdir /s /q obj

