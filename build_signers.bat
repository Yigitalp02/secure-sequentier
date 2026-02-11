@echo off
REM Build script for SignerApp and AdvancedSignerApp
REM Update the SUBSYSTEMS_PATH variable below to match your setup

set SUBSYSTEMS_PATH=C:\Users\%USERNAME%\Subsystems

echo Building SignerApp as self-contained executable...
cd /d "%SUBSYSTEMS_PATH%\SignerApp"
if errorlevel 1 (
    echo ERROR: SignerApp directory not found at %SUBSYSTEMS_PATH%\SignerApp
    echo Please update SUBSYSTEMS_PATH in this script or create the directory structure.
    pause
    exit /b 1
)

dotnet publish SignerApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false

echo.
echo Building AdvancedSignerApp as self-contained executable...
cd /d "%SUBSYSTEMS_PATH%\AdvancedSignerApp"
if errorlevel 1 (
    echo ERROR: AdvancedSignerApp directory not found at %SUBSYSTEMS_PATH%\AdvancedSignerApp
    echo Please update SUBSYSTEMS_PATH in this script or create the directory structure.
    pause
    exit /b 1
)

dotnet publish AdvancedSignerApp.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false

echo.
echo Copying executables to main directories...
copy "%SUBSYSTEMS_PATH%\SignerApp\bin\Release\net8.0\win-x64\publish\SignerApp.exe" "%SUBSYSTEMS_PATH%\SignerApp\SignerApp.exe"
copy "%SUBSYSTEMS_PATH%\AdvancedSignerApp\bin\Release\net8.0\win-x64\publish\AdvancedSignerApp.exe" "%SUBSYSTEMS_PATH%\AdvancedSignerApp\AdvancedSignerApp.exe"

echo.
echo Build complete! Self-contained executables are ready.
echo.
pause
