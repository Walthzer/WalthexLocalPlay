:: This script will do the initial setup for your mod.
@echo off
IF NOT EXIST "settings.bat" (
    exit 1
)
echo %MOD_NAME% | findstr /r /i ^^[a-z][a-z0-9_]*$ > nul && (
    echo MOD_NAME has invalid characters. 
    exit 1
)

call settings.bat
:: setup the thunderstore manifest.json
echo Setup packageFiles\manifest.json
powershell -Command "((gc 'packageFiles\manifest.json') -replace 'MOD_NAME', '%MOD_NAME%') -replace 'MOD_AUTHOR', '%MOD_AUTHOR%' | Out-File -encoding ASCII 'packageFiles\manifest.json'"

::setup the project.csproj
echo Setup src\%MOD_NAME%.csproj
powershell -Command "(gc 'src\MageArenaModTemplate.csproj') -replace '(<AssemblyName>|<Product>)(.*?)(</AssemblyName>|</Product>)', '$1%MOD_NAME%$3' | Out-File -encoding ASCII 'src\%MOD_NAME%.csproj'"
del src\MageArenaModTemplate.csproj

echo %MOD_NAME% has been setup succesfully.