@echo off
setlocal enabledelayedexpansion

set "MOD_NAME=MyCustomRolesMod"
set "VERSION=4.1"
set "DIST_DIR=dist"
set "ZIP_NAME=%MOD_NAME%-v%VERSION%-Distribution.zip"

echo === Build Script for %MOD_NAME% v%VERSION% ===
echo.

:: Step 1: Check for .NET SDK
echo Step 1: Checking for .NET SDK...
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK is not installed!
    echo Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo [OK] .NET SDK found.
echo.

:: Step 2: Clean up old artifacts
echo Step 2: Cleaning up old artifacts...
if exist "%DIST_DIR%" rmdir /s /q "%DIST_DIR%"
if exist "%ZIP_NAME%" del /f "%ZIP_NAME%"
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
mkdir "%DIST_DIR%"
echo [OK] Cleanup complete.
echo.

:: Step 3: Verify Game Assemblies
echo Step 3: Verifying game assemblies...
set "GAME_ASSEMBLY_PATH="
set "DOTNET_BUILD_CMD=dotnet build MyCustomRolesMod.csproj --configuration Release"

if defined AMONG_US_GAME_PATH (
    echo --^> Using AMONG_US_GAME_PATH environment variable: "%AMONG_US_GAME_PATH%"
    if not exist "%AMONG_US_GAME_PATH%" (
        echo ERROR: AMONG_US_GAME_PATH is not a valid directory.
        pause
        exit /b 1
    )
    set "GAME_ASSEMBLY_PATH=%AMONG_US_GAME_PATH%\Among Us_Data\Managed"
    set "DOTNET_BUILD_CMD=%DOTNET_BUILD_CMD% -p:AmongUsGamePath="%AMONG_US_GAME_PATH%""
) else (
    echo --^> AMONG_US_GAME_PATH not set. Checking for game assemblies in BepInEx managed folder...
    set "GAME_ASSEMBLY_PATH=bepinex_files\BepInEx\unity-libs"

    if not exist "!GAME_ASSEMBLY_PATH!" (
        echo --^> BepInEx unity-libs not found. Trying interop folder...
        set "GAME_ASSEMBLY_PATH=bepinex_files\BepInEx\interop"
    )

    if not exist "!GAME_ASSEMBLY_PATH!" (
        echo ERROR: Could not find game assemblies in BepInEx folders.
        echo.
        echo Please either:
        echo   1. Set AMONG_US_GAME_PATH environment variable to your Among Us installation, OR
        echo   2. Ensure bepinex_files contains the full BepInEx 6.0.0-pre.1 IL2CPP installation
        pause
        exit /b 1
    )

    if not exist "lib\AmongUs" mkdir "lib\AmongUs"
    set "DOTNET_BUILD_CMD=%DOTNET_BUILD_CMD% -p:AmongUsGamePath=lib\AmongUs"
)

if not exist "!GAME_ASSEMBLY_PATH!" (
    echo ERROR: Assembly directory not found: "!GAME_ASSEMBLY_PATH!"
    pause
    exit /b 1
)

:: Check for required assemblies
set "ALL_FILES_FOUND=true"
set "REQUIRED_ASSEMBLIES=Assembly-CSharp.dll Il2Cppmscorlib.dll Hazel.dll UnityEngine.CoreModule.dll UnityEngine.UI.dll UnityEngine.TextRenderingModule.dll"

for %%F in (%REQUIRED_ASSEMBLIES%) do (
    set "FOUND=false"

    if exist "!GAME_ASSEMBLY_PATH!\%%~F" (
        set "FOUND=true"
        if not defined AMONG_US_GAME_PATH (
            if not exist "lib\AmongUs\%%~F" (
                copy "!GAME_ASSEMBLY_PATH!\%%~F" "lib\AmongUs\" >nul 2>&1
            )
        )
    )

    if "!FOUND!"=="false" (
        echo ERROR: Missing required game assembly: %%~F
        set "ALL_FILES_FOUND=false"
    )
)

if "!ALL_FILES_FOUND!"=="false" (
    echo.
    echo Build failed. Could not find all required assemblies.
    echo Please ensure you have the complete BepInEx installation or set AMONG_US_GAME_PATH.
    pause
    exit /b 1
)
echo [OK] All game assemblies found.
echo.

:: Step 4: Verify BepInEx Files
echo Step 4: Verifying BepInEx files...
set "BEPINEX_DIR=bepinex_files"
if not exist "%BEPINEX_DIR%" (
    echo ERROR: The '%BEPINEX_DIR%' directory is missing.
    echo Please download BepInEx 6.0.0-pre.1 IL2CPP from:
    echo https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.1
    echo Then, extract it to a folder named '%BEPINEX_DIR%' in this directory.
    pause
    exit /b 1
)
echo [OK] BepInEx directory found.
echo.

:: Step 5: Build the mod
echo Step 5: Building the mod...
%DOTNET_BUILD_CMD%
set "DLL_PATH=bin\Release\net6.0\%MOD_NAME%.dll"
if not exist "%DLL_PATH%" (
    echo ERROR: DLL was not created. Build failed!
    pause
    exit /b 1
)
for %%A in ("%DLL_PATH%") do set "DLL_SIZE=%%~zA"
echo [OK] Mod built successfully (!DLL_SIZE! bytes).
echo.

:: Step 6: Assemble distribution
echo Step 6: Assembling distribution package...
mkdir "%DIST_DIR%\BepInEx\plugins" >nul 2>&1
xcopy /s /e /i /q "%BEPINEX_DIR%\*.*" "%DIST_DIR%"
copy "%DLL_PATH%" "%DIST_DIR%\BepInEx\plugins\" >nul
echo [OK] Distribution package assembled in '%DIST_DIR%'.
echo.

:: Step 7: Create ZIP
echo Step 7: Creating distribution ZIP...
where powershell >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    powershell -NoProfile -Command "Compress-Archive -Path '%DIST_DIR%\*' -DestinationPath '%ZIP_NAME%' -Force"
    echo [OK] Created %ZIP_NAME%
) else (
    echo WARNING: PowerShell not found. ZIP creation skipped.
    echo You can manually zip the contents of the '%DIST_DIR%' folder.
)
echo.

echo === BUILD COMPLETE ===
echo.
echo The final distribution is available in: %ZIP_NAME%
echo The mod DLL is also available at: %DLL_PATH%
echo.
pause
